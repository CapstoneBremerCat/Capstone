using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public enum State
    {
        Ready,  // �߻� �غ��
        Empty,  // źâ�� ��
        Reloading   // ������ ��
    }

    public class Weapon : MonoBehaviour
    {
        private State state = State.Ready;

        [Header("Projectile")] //inspector�� ǥ��, �ش� Data���� ���ó�� �˷��ش�.
        [SerializeField] private Projectile Projectile; // ����ü
        [SerializeField] private Transform firePos; // ����ü �߻� ��ġ.
        [SerializeField] private int _weaponId;  // ���� ID
        public int weaponId { get { return _weaponId; }}  // ���� ID.

        [Header("Projectile Attribute")] // [Range(a,b)] : ���� ������ ����(a~b).
        [SerializeField] private float hitRange = 100f;  // �����Ÿ�.
        [SerializeField] private float timeBetFire = 0.12f;    // ����ü �߻� ����.
        private float lastFireTime; // ���������� �߻��� ����.

        [SerializeField] private readonly int magCapacity = 50;  // źâ �뷮.
        [SerializeField] private int ammoRemain = 900; // �����ϰ� �ִ� ����ü�� ��.
        private int magAmmo;    // ���� źâ�� �����ִ� ����ü�� ��.

        [SerializeField] private float reloadTime = 0.9f;   // ������ �ҿ� �ð�.

        [SerializeField] private float damage = 25;   // ������ ���ݷ�

        [Header("SFX")]
        [SerializeField] private ParticleSystem muzzleFlashEffect;  // �߻�ü �Ա� ȿ��.
        [SerializeField] private AudioClip shootSound;  // �߻� ȿ����.
        [SerializeField] private AudioClip reloadSound; // źâ ������ ȿ����.
        private AudioSource audioSource;

        private Vector3 originPos;  // ���� ��ġ
        [SerializeField] private float retroActionForce;  // �ݵ� ����

        public Transform FirePos { get { return firePos; } }
        public State GetState { get { return state; } } // Ŀ����.
        public int AmmoRemain { get { return ammoRemain; } }
        public int MagAmmo { get { return magAmmo; } }
        public float HitRange { get { return hitRange; } }
        public bool IsFire { get; private set; }


        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            //pivot = transform.parent;
        }

        public void Init()
        {
            magAmmo = magCapacity;  // �ʱ� źâ�� �Ѿ��� �ִ�ġ��.
            state = State.Ready;
            lastFireTime = Time.time - timeBetFire;
            SetPos(transform.position);
        }

        public void SetPos(Vector3 position)
        {
            // ù ��ġ ���
            originPos = position;
        }

        public void Fire()
        {
            if (state.Equals(State.Ready) && Time.time >= lastFireTime + timeBetFire)    // ���� ���� �� �� �ִ� �������� Ȯ��.
            {
                lastFireTime = Time.time;   // �߻� ���� ����.
                Shot(); //�߻� ó��
                IsFire = true;
            }
            IsFire = false;
        }

        private void Shot()
        {
            if (firePos)
            {
                RaycastHit hit; // Physics.RayCast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
                Vector3 hitPos = firePos.position + firePos.forward * hitRange; // �Ѿ��� ���� ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
                {
                    Debug.DrawRay(firePos.position, firePos.forward * hit.distance, Color.red);
                    Debug.DrawRay(firePos.position, hitPos, Color.blue);
                    // TODO : Enemy(Zombie) Damageable Code �߰�
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    if (null != target) target.OnDamage(damage, hit.point, hit.normal);
                    hitPos = hit.point; // ���� �Ѿ��� ���� �������� ����.
                }
                // �ѱ� �ݵ� �ڷ�ƾ ����
                StopAllCoroutines();
                StartCoroutine(RetroActionCoroutine());
                StartCoroutine(ShotEffect(hitPos)); // �Ѿ� �߻� ����Ʈ.

                Instantiate(Projectile.gameObject, firePos.transform.position, firePos.transform.rotation);   // �Ѿ� �߻�

                magAmmo--;  // źâ�� �Ѿ��� ����.
                if (0 >= magAmmo) state = State.Empty;  // ���� �Ѿ� �� Ȯ��. �Ѿ��� ���ٸ�, ���� ���¸� Empty�� ����.
            }
        }

        IEnumerator RetroActionCoroutine()
        {
            Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -retroActionForce);     // ������ �� ���� ���� �ִ� �ݵ�
                                                                                               //Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);  // ������ ���� ���� �ִ� �ݵ�

            //if (!isFineSightMode)  // �������� �ƴ� ����
            {
                transform.localPosition = originPos;

                // �ݵ� ����
                while (transform.localPosition.z >= -retroActionForce)
                {
                    transform.localPosition = Vector3.Lerp(transform.localPosition, recoilBack, 0.4f);
                    yield return null;
                }

                // ����ġ
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

            yield return new WaitForSeconds(0.03f); // 0.03�� ���� ��� ó���� ���.
        }


        public bool Reload()
        {
            // ������ ���̰ų� ���� ź���� ���ų�,
            // ź���� �������� ���̻� �߰��� �� ���� ���� ������ �Ұ���.
            if (state.Equals(State.Reloading) || 0 >= ammoRemain || magCapacity <= magAmmo) return false;
            StartCoroutine(ReloadRoutine()); // ������ ���·� ����.
            return true;
        }

        private IEnumerator ReloadRoutine()
        {
            state = State.Reloading; // ���� ���¸� ������ �� ���·� ��ȯ.

            if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);

            yield return new WaitForSeconds(reloadTime); // ������ �ҿ� �ð� ��ŭ ó���� ����.

            // źâ�� ä�� �Ѿ��� ���.
            // �����ϰ� �ִ� �Ѿ˰� źâ�� �� ���� ���� Ȯ���Ͽ� ���� ���� ���.
            int ammoTofill = Mathf.Min(magCapacity - magAmmo, ammoRemain);
            ammoRemain -= ammoTofill; // �����ϰ� �ִ� �Ѿ��� ���� ����.
            magAmmo += ammoTofill; // ������ �Ѿ��� ����ŭ źâ�� �Ѿ��� �߰�.

            state = State.Ready; // ���� ���� ���¸� �߻� �غ�� ���·� ����.
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