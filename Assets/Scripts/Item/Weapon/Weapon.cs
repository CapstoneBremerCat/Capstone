using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public enum State
    {
        Ready,  // 발사 준비됨
        Empty,  // 탄창이 빔
        Reloading   // 재장전 중
    }

    public class Weapon : MonoBehaviour
    {
        private Transform pivot;
        private State state = State.Ready;

        [Header("Projectile")] //inspector에 표시, 해당 Data들의 사용처를 알려준다.
        [SerializeField] private Projectile Projectile; // 투사체
        [SerializeField] private Transform firePos; // 투사체 발사 위치.
        [SerializeField] private string weaponName;  // 무기 이름.

        [Header("Projectile Attribute")] // [Range(a,b)] : 값의 범위를 제한(a~b).
        [SerializeField] private float hitRange = 100f;  // 사정거리.
        [SerializeField] private float timeBetFire = 0.12f;    // 투사체 발사 간격.
        private float lastFireTime; // 마지막으로 발사한 시점.

        [SerializeField] private readonly int magCapacity = 50;  // 탄창 용량.
        [SerializeField] private int ammoRemain = 900; // 소지하고 있는 투사체의 수.
        private int magAmmo;    // 현재 탄창에 남아있는 투사체의 수.

        [SerializeField] private float reloadTime = 0.9f;   // 재장전 소요 시간.

        [SerializeField] private float damage = 25;   // 무기의 공격력

        [Header("SFX")]
        [SerializeField] private ParticleSystem muzzleFlashEffect;  // 발사체 입구 효과.
        [SerializeField] private AudioClip shootSound;  // 발사 효과음.
        [SerializeField] private AudioClip reloadSound; // 탄창 재장전 효과음.
        private AudioSource audioSource;

        private Vector3 originPos;  // 원래 위치
        [SerializeField] private float retroActionForce;  // 반동 세기

        public Transform FirePos { get { return firePos; } }
        public Vector3 Pivot { get { return (pivot) ? pivot.position : Vector3.zero; } set { if (pivot) pivot.position = value; } }
        public State GetState { get { return state; } } // 커스텀.
        public int AmmoRemain { get { return ammoRemain; } }
        public int MagAmmo { get { return magAmmo; } }
        public float HitRange { get { return hitRange; } }
        public string WeaponName { get { return weaponName; } }

        public bool IsFire { get; private set; }


        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            //pivot = transform.parent;
        }

        private void OnEnable()
        {
            magAmmo = magCapacity;  // 초기 탄창의 총알을 최대치로.
            state = State.Ready;
            lastFireTime = Time.time - timeBetFire;

        }

        public void SetPos(Vector3 position)
        {
            // 첫 위치 등록
            originPos = position;
        }

        public void SetFirePos(Transform transform)
        {
            firePos = transform;
        }

        public void Fire()
        {
            if (state.Equals(State.Ready) && Time.time >= lastFireTime + timeBetFire)    // 현재 총을 쏠 수 있는 상태인지 확인.
            {
                lastFireTime = Time.time;   // 발사 시점 갱신.
                Shot(); //발사 처리
                IsFire = true;
            }
            IsFire = false;
        }

        private void Shot()
        {
            if (firePos)
            {
                RaycastHit hit; // Physics.RayCast()를 이용하여 충돌 지점 정보를 알아온다.
                Vector3 hitPos = firePos.position + firePos.forward * hitRange; // 총알이 맞은 위치를 저장, 최대 거리 위치를 기본 값으로 가진다.
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
                {
                    Debug.DrawRay(firePos.position, firePos.forward * hit.distance, Color.red);
                    Debug.DrawRay(firePos.position, hitPos, Color.blue);
                    // TODO : Enemy(Zombie) Damageable Code 추가
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    if (null != target) target.OnDamage(damage, hit.point, hit.normal);
                    hitPos = hit.point; // 실제 총알이 맞은 지점으로 갱신.
                }
                // 총기 반동 코루틴 실행
                StopAllCoroutines();
                StartCoroutine(RetroActionCoroutine());
                StartCoroutine(ShotEffect(hitPos)); // 총알 발사 이펙트.

                Instantiate(Projectile.gameObject, firePos.transform.position, firePos.transform.rotation);   // 총알 발사

                magAmmo--;  // 탄창의 총알을 감소.
                if (0 >= magAmmo) state = State.Empty;  // 남은 총알 수 확인. 총알이 없다면, 총의 상태를 Empty로 변경.
            }
        }

        IEnumerator RetroActionCoroutine()
        {
            Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -retroActionForce);     // 정조준 안 했을 때의 최대 반동
                                                                                               //Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);  // 정조준 했을 때의 최대 반동

            //if (!isFineSightMode)  // 정조준이 아닌 상태
            {
                transform.localPosition = originPos;

                // 반동 시작
                while (transform.localPosition.z >= -retroActionForce)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, recoilBack, 0.4f);
                    yield return null;
                }

                // 원위치
                while (transform.localPosition != originPos)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, originPos, 0.1f);
                    yield return null;
                }
            }
        }

        private IEnumerator ShotEffect(Vector3 hitPosition)
        {
            if (muzzleFlashEffect) muzzleFlashEffect.Play();
            if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

            yield return new WaitForSeconds(0.03f); // 0.03초 동안 잠시 처리를 대기.
        }


        public bool Reload()
        {
            // 재장전 중이거나 남은 탄약이 없거나,
            // 탄알이 가득차서 더이상 추가할 수 없는 경우는 재장전 불가능.
            if (state.Equals(State.Reloading) || 0 >= ammoRemain || magCapacity <= magAmmo) return false;
            StartCoroutine(ReloadRoutine()); // 재장전 상태로 변경.
            return true;
        }

        private IEnumerator ReloadRoutine()
        {
            state = State.Reloading; // 현재 상태를 재장전 중 상태로 전환.

            if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);

            yield return new WaitForSeconds(reloadTime); // 재장전 소요 시간 만큼 처리를 쉬기.

            // 탄창에 채울 총알을 계산.
            // 소지하고 있는 총알과 탄창의 빈 공간 수를 확인하여 작은 값을 사용.
            int ammoTofill = Mathf.Min(magCapacity - magAmmo, ammoRemain);
            ammoRemain -= ammoTofill; // 소지하고 있는 총알의 수를 감소.
            magAmmo += ammoTofill; // 감소한 총알의 수만큼 탄창에 총알을 추가.

            state = State.Ready; // 총의 현재 상태를 발사 준비된 상태로 변경.
        }

        public void AddAmmo(int value)
        {
            ammoRemain += value;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}