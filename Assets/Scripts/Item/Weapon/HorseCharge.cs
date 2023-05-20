using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class HorseCharge : Projectile
    {
        [SerializeField] private Collider trigger;   // 투사체의 rigidbody

        private void OnEnable()
        {
            StopAllCoroutines();
            trigger.enabled = false;
            StartCoroutine(TriggerOn(0.4f));
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
            trigger.enabled = false;
            gameObject.SetActive(false);
        }
    }
}