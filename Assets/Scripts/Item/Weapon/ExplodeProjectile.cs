using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExplodeProjectile : Projectile
    {
        [SerializeField] [Range(0, 20)] private float radius = 4;    // ���� ����
        [SerializeField] protected LayerMask targetLayer;
/*        [SerializeField] private ParticleSystem explode;*/

        private void OnEnable()
        {
/*            explode.gameObject.SetActive(false);*/
            StopAllCoroutines();
            Shoot();
            StartCoroutine(ExistTime(5.0f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Partner" || other.tag == "Player") return;
/*            explode.Stop();
            explode.gameObject.SetActive(true);*/
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer); // �־��� �ݰ� ���� ��� Ÿ���� �����ɴϴ�.

            foreach (Collider hitCollider in hitColliders) // �� �浹ü�� ���� ������ ���ϴ�.
            {
                Enemy target = hitCollider.GetComponent<Enemy>(); // ������ Ȯ���մϴ�.
                if (target != null) // ���� ��쿡�� �������� ���մϴ�.
                {
                    Vector3 hitNormal = hitCollider.transform.position - transform.position; // ������ ������ ������ ���մϴ�.
                    target.OnDamage(damage, Vector3.zero, hitNormal); // �������� ���մϴ�.
                }
            }
/*            explode.Play();*/
        }
    }
}