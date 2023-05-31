using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Game;
namespace Game
{
    public class Charge : MonoBehaviour
    {
        [SerializeField] private float damage = 10.0f;
/*        [SerializeField] private float speed = 20.0f;*/
        [SerializeField] private float effectiveDistance;  // 사정거리.
        [SerializeField] private LayerMask obstacle;  // 장애물
        [SerializeField] private float knockbackForce = 10.0f;
        [SerializeField] private float knockbackDuration = 1.0f;

        private bool isAttacking = false;
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q) && !isAttacking)
            {
                StartCoroutine(AttackChargeCoroutine());
            }
        }
        IEnumerator AttackChargeCoroutine()
        {
            isAttacking = true;
            /*        agent.velocity = transform.forward * speed;
                    agent.isStopped = false;*/

            RaycastHit hit; // Physics.RayCast()를 이용하여 충돌 지점 정보를 알아온다.
            Vector3 hitPos = transform.position + transform.forward * effectiveDistance; // 충돌 위치를 저장, 최대 거리 위치를 기본 값으로 가진다.

            // 장애물 직전에서 멈추도록 한다.
            if (Physics.Raycast(transform.position, transform.forward, out hit, effectiveDistance, obstacle))
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
                hitPos = hit.point; // 실제 충돌이 일어난 지점으로 갱신.
            }

            // 돌진 시작
            while (transform.position.z < hitPos.z - 0.02f)
            {
                transform.position = Vector3.Lerp(transform.position, hitPos, 0.1f);
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
                yield return null;
            }

            isAttacking = false;
            /*        agent.velocity = Vector3.zero;
                    agent.isStopped = true;*/

        }

        void OnCollisionEnter(Collision collision)
        {
            if (isAttacking && collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    ContactPoint contact = collision.contacts[0];
                    enemy.OnDamage(damage, contact.point, contact.normal);

                    Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    Vector3 knockbackDirection = contact.normal;
                    enemyRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                    StartCoroutine(KnockbackCoroutine(enemyRigidbody));
                }
            }
        }

        IEnumerator KnockbackCoroutine(Rigidbody rigidbody)
        {
            NavMeshAgent enemyAgent = rigidbody.gameObject.GetComponent<NavMeshAgent>();
            enemyAgent.isStopped = true;

            yield return new WaitForSeconds(knockbackDuration);

            rigidbody.velocity = Vector3.zero;
            enemyAgent.isStopped = false;
        }
    }
}