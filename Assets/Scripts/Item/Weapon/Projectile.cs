using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigid;   // ����ü�� rigidbody
        [SerializeField] private float force = 100.0f; // �������� ��
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] private GameObject[] projectiles;
        public float damage { get; private set; }
        public void InitProjectile(Transform transform, float damage)
        {
            this.transform.position = transform.position;
            this.transform.rotation = transform.rotation;
            this.damage = damage;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            foreach (var obj in projectiles)
            {
                obj.SetActive(true);
            }
            Shoot();
            StartCoroutine(ExistTime(5.0f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                StartCoroutine(DisableGameObjectCoroutine());
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = other.ClosestPoint(transform.position) - other.transform.position;
                Enemy target = other.GetComponent<Enemy>();
                if (null != target) target.OnDamage(damage, hitPoint, hitNormal);
            }
        }

        protected void Shoot()
        {
            rigid.velocity = transform.forward * force;
        }

        // �Ѿ��� ���� �ð����� ���� �� ��Ȱ��ȭ�ǵ��� ó��
        protected IEnumerator ExistTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }

        private IEnumerator DisableGameObjectCoroutine()
        {
            foreach (var obj in projectiles)
            {
                obj.SetActive(false);
            }
            while (audioSource.isPlaying)
            {
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}