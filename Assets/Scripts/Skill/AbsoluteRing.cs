using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class AbsoluteRing : ActiveSkill
    {
        [SerializeField] [Range(0, 20)] private float radius = 4;    // 공격 범위
        [SerializeField] private float damage = 400;    // 피해량
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected GameObject effect;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public override void UseSkill()
        {
            effect.SetActive(false);
            effect.SetActive(true);
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer); // 주어진 반경 내의 모든 타겟을 가져옵니다.

            foreach (Collider hitCollider in hitColliders) // 각 충돌체에 대해 루프를 돕니다.
            {
                Enemy target = hitCollider.GetComponent<Enemy>(); // 적인지 확인합니다.
                if (target != null) // 적인 경우에만 데미지를 가합니다.
                {
                    Vector3 hitNormal = hitCollider.transform.position - transform.position; // 공격이 가해질 방향을 구합니다.
                    target.OnDamage(damage, Vector3.zero, hitNormal); // 데미지를 가합니다.
                }
            }
        }
    }
}
