using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class AbsoluteRing : ActiveSkill
    {
        [SerializeField] [Range(0, 20)] private float radius = 4;    // ���� ����
        [SerializeField] private float damage = 400;    // ���ط�
        [SerializeField] protected LayerMask targetLayer;
        public AbsoluteRing(SkillInfo skillInfo, float cooldown) : base(skillInfo, cooldown)
        {
            this.skillInfo = skillInfo;
            this.cooldown = cooldown;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public override void UseSkill()
        {
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
        }
    }
}
