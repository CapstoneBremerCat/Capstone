using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Weapon : MonoBehaviour
    {
        [Header("Projectile")] //inspector�� ǥ��, �ش� Data���� ���ó�� �˷��ش�.
        private List<GameObject> projectilePool; // ������Ʈ Ǯ ����Ʈ
        [SerializeField] private int poolSize = 10; // �ʱ� Ǯ ũ��
        [SerializeField] private GameObject projectilePrefab; // ������ ������
        [SerializeField] private Transform firePos; // ����ü �߻� ��ġ.
        [SerializeField] private int _weaponId;  // ���� ID
        public int weaponId { get { return _weaponId; }}  // ���� ID.

        [Header("Projectile Attribute")] // [Range(a,b)] : ���� ������ ����(a~b).
        [SerializeField] private float hitRange = 100f;  // �����Ÿ�.
        [SerializeField] private float timeBetFire = 2.0f;    // ����ü �߻� ����.
        private float lastFireTime; // ���������� �߻��� ����.

        //[SerializeField] private float reloadTime = 0.9f;   // ������ �ҿ� �ð�.

        [SerializeField] private float damage = 25;   // ������ ���ݷ�
        private float totalDamage;   // �� ���ݷ�

        [Header("SFX")]
        [SerializeField] private ParticleSystem muzzleFlashEffect;  // �߻�ü �Ա� ȿ��.
        [SerializeField] private AudioClip shootSound;  // �߻� ȿ����.
        [SerializeField] private AudioClip reloadSound; // źâ ������ ȿ����.
        private AudioSource audioSource;

        private Vector3 originPos;  // ���� ��ġ
        [SerializeField] private float retroActionForce;  // �ݵ� ����

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
                GameObject projectileObj = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
                projectileObj.transform.SetParent(null);
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
            // ù ��ġ ���
            originPos = position;
        }

        public void Fire()
        {
            if (Time.time >= lastFireTime + timeBetFire)    // ���� ���� �� �� �ִ� �������� Ȯ��.
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
/*                RaycastHit hit; // Physics.RayCast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
                Vector3 hitPos = firePos.position + firePos.forward * hitRange; // �Ѿ��� ���� ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.
                if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
                {
                    Debug.DrawRay(firePos.position, firePos.forward * hit.distance, Color.red);
                    Debug.DrawRay(firePos.position, hitPos, Color.blue);
                    // TODO : Enemy(Zombie) Damageable Code �߰�
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    if (null != target) target.OnDamage(totalDamage, hit.point, hit.normal);
                    hitPos = hit.point; // ���� �Ѿ��� ���� �������� ����.
                }*/
                // �ѱ� �ݵ� �ڷ�ƾ ����
                //StopAllCoroutines();
                //StartCoroutine(ShotEffect(hitPos)); // �Ѿ� �߻� ����Ʈ.

                // ������Ʈ Ǯ���� ������Ʈ�� ������ Ȱ��ȭ��Ű�� ��ġ, ȸ������ �����Ѵ�.
                GameObject projectileObj = GetProjectileFromPool();
                projectileObj.GetComponent<Projectile>().InitProjectile(firePos, totalDamage);
                projectileObj.SetActive(true);
            }
        }

        private IEnumerator ShotEffect(Vector3 hitPosition)
        {
            if (muzzleFlashEffect) muzzleFlashEffect.Play();
            if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

            yield return new WaitForSeconds(0.03f); // 0.03�� ���� ��� ó���� ���.
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

            // Ǯ�� �� ������ ������ ���ο� ������Ʈ�� �����Ͽ� �߰��Ѵ�.
            GameObject projectileObj = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            projectileObj.transform.SetParent(null);
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