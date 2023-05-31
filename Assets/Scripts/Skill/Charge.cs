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
        [SerializeField] private float effectiveDistance;  // �����Ÿ�.
        [SerializeField] private LayerMask obstacle;  // ��ֹ�
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

            RaycastHit hit; // Physics.RayCast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
            Vector3 hitPos = transform.position + transform.forward * effectiveDistance; // �浹 ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.

            // ��ֹ� �������� ���ߵ��� �Ѵ�.
            if (Physics.Raycast(transform.position, transform.forward, out hit, effectiveDistance, obstacle))
            {
                Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
                hitPos = hit.point; // ���� �浹�� �Ͼ �������� ����.
            }

            // ���� ����
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