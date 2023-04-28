using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Laser : Projectile
    {
        [SerializeField] private Collider trigger;   // 투사체의 rigidbody

        private void OnEnable()
        {
            GameManager.Instance.SetMovement(false);
            StopAllCoroutines();
            trigger.enabled = false;
            StartCoroutine(TriggerOn(1.0f));
        }
        private void OnDisable()
        {
            GameManager.Instance.SetMovement(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = other.ClosestPoint(transform.position) - other.transform.position;
                Enemy target = other.GetComponent<Enemy>();
                if (null != target) target.OnDamage(damage, hitPoint, hitNormal);
            }
        }

        protected IEnumerator TriggerOn(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            trigger.enabled = true;
            yield return new WaitForSeconds(seconds);
            GameManager.Instance.SetMovement(true);
            trigger.enabled = false;
            gameObject.SetActive(false);
        }
    }
}