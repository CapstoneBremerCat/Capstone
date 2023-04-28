using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigid;   // 투사체의 rigidbody
        [SerializeField] private float force = 100.0f; // 가해지는 힘
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
            Shoot();
            StartCoroutine(ExistTime(5.0f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                gameObject.SetActive(false);
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

        // 총알이 일정 시간동안 존재 후 비활성화되도록 처리
        protected IEnumerator ExistTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }
}