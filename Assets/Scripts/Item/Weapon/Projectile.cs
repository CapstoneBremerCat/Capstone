using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigid;   // 투사체의 rigidbody
        [SerializeField] private float force = 100.0f; // 가해지는 힘
        private float damage;
        public void InitProjectile(Transform transform, float damage)
        {
            this.transform.position = transform.position;
            this.transform.rotation = transform.rotation;
            this.damage = damage;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            rigid.velocity = transform.forward * force;
            StartCoroutine(ExistTime(10.0f));
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

        // 총알이 일정 시간동안 존재 후 비활성화되도록 처리
        private IEnumerator ExistTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }
}