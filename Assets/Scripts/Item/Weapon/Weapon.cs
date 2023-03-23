using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Weapon : MonoBehaviour
    {
        [Header("Projectile")] //inspector에 표시, 해당 Data들의 사용처를 알려준다.
        private List<GameObject> projectilePool; // 오브젝트 풀 리스트
        [SerializeField] private int poolSize = 10; // 초기 풀 크기
        [SerializeField] private GameObject projectilePrefab; // 생성할 프리팹
        [SerializeField] private Transform firePos; // 투사체 발사 위치.
        [SerializeField] private int _weaponId;  // 무기 ID
        public int weaponId { get { return _weaponId; }}  // 무기 ID.

        [Header("Projectile Attribute")] // [Range(a,b)] : 값의 범위를 제한(a~b).
        [SerializeField] private float hitRange = 100f;  // 사정거리.
        [SerializeField] private float timeBetFire = 2.0f;    // 투사체 발사 간격.
        private float lastFireTime; // 마지막으로 발사한 시점.

        [SerializeField] private float reloadTime = 0.9f;   // 재장전 소요 시간.

        [SerializeField] private float damage = 25;   // 무기의 공격력
        private float totalDamage;   // 총 공격력

        [Header("SFX")]
        [SerializeField] private ParticleSystem muzzleFlashEffect;  // 발사체 입구 효과.
        [SerializeField] private AudioClip shootSound;  // 발사 효과음.
        [SerializeField] private AudioClip reloadSound; // 탄창 재장전 효과음.
        private AudioSource audioSource;

        private Vector3 originPos;  // 원래 위치
        [SerializeField] private float retroActionForce;  // 반동 세기

        public Transform FirePos { get { return firePos; } }
        public float HitRange { get { return hitRange; } }
        public float Damage { get { return damage; } }
        public bool IsFire { get; private set; }


        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public void Init(Transform parentTransform)
        {
            lastFireTime = Time.time - timeBetFire;
            totalDamage = damage;
            transform.position = parentTransform.TransformPoint(transform.localPosition);
            transform.rotation = Quaternion.LookRotation(
    parentTransform.TransformDirection(transform.localPosition), parentTransform.up);
            InitProjectilePool();
        }
        private void InitProjectilePool()
        {
            projectilePool = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectileObj = Instantiate(projectilePrefab, transform);
                projectileObj.SetActive(false);
                projectilePool.Add(projectileObj);
            }
        }

        public void UpdateWeaponStats(Status status)
        {
            totalDamage = status.totalDamage;
            timeBetFire = Mathf.Max(0.1f, timeBetFire * 100 / (100 + status.attackSpeed));
        }

        public void SetPos(Vector3 position)
        {
            // 첫 위치 등록
            originPos = position;
        }

        public void Fire()
        {
            if (Time.time >= lastFireTime + timeBetFire)    // 현재 총을 쏠 수 있는 상태인지 확인.
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
/*                RaycastHit hit; // Physics.RayCast()를 이용하여 충돌 지점 정보를 알아온다.
                Vector3 hitPos = firePos.position + firePos.forward * hitRange; // 총알이 맞은 위치를 저장, 최대 거리 위치를 기본 값으로 가진다.
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
                {
                    Debug.DrawRay(firePos.position, firePos.forward * hit.distance, Color.red);
                    Debug.DrawRay(firePos.position, hitPos, Color.blue);
                    // TODO : Enemy(Zombie) Damageable Code 추가
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    if (null != target) target.OnDamage(totalDamage, hit.point, hit.normal);
                    hitPos = hit.point; // 실제 총알이 맞은 지점으로 갱신.
                }*/
                // 총기 반동 코루틴 실행
                //StopAllCoroutines();
                //StartCoroutine(ShotEffect(hitPos)); // 총알 발사 이펙트.

                // 오브젝트 풀에서 오브젝트를 가져와 활성화시키고 위치, 회전값을 설정한다.
                GameObject projectileObj = GetProjectileFromPool();
                projectileObj.GetComponent<Projectile>().InitProjectile(firePos, totalDamage);
                projectileObj.SetActive(true);
            }
        }

        private IEnumerator ShotEffect(Vector3 hitPosition)
        {
            if (muzzleFlashEffect) muzzleFlashEffect.Play();
            if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

            yield return new WaitForSeconds(0.03f); // 0.03초 동안 잠시 처리를 대기.
        }
        private GameObject GetProjectileFromPool()
        {
            for (int i = 0; i < projectilePool.Count; i++)
            {
                if (!projectilePool[i].activeInHierarchy)
                {
                    return projectilePool[i];
                }
            }

            // 풀에 빈 공간이 없으면 새로운 오브젝트를 생성하여 추가한다.
            GameObject projectileObj = Instantiate(projectilePrefab, transform);
            projectileObj.SetActive(false);
            projectilePool.Add(projectileObj);
            return projectileObj;
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}