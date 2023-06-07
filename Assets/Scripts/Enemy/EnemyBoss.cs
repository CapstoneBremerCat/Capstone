using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;
using System.Threading;
using Game;
namespace Game
{
    public class EnemyBoss : Enemy
    {
        [SerializeField] private Slider bossHealth; // 슬라이더(보스 체력)
        [SerializeField] private GameObject canvas; // panel
        [SerializeField] private GameObject panel; // panel
        [SerializeField] private GameObject BossBody; // 보스 오브젝트
        [SerializeField] private GameObject BossMomentHide; // 보스 은신시 panel
        [SerializeField] private GameObject FallingWarning; // 떨어지는 투사체 object

        private bool isFallingAttack = false;
        private int assasin = 0;

        protected override void Awake()
        {
            base.Awake();
            OnDeath += () =>
            {
                if (ItemMgr.Instance) ItemMgr.Instance.SpawnItem(transform.position + Vector3.up);
                canvas.SetActive(false);
                panel.SetActive(false);
                BossMomentHide.SetActive(false);
            };
            bossHealth.interactable = false;
            canvas.SetActive(false);
        }
        private IEnumerator FallingAttack()
        {
            if (assasin != 2) yield return new WaitForSeconds(3.0f);
            else yield return new WaitForSeconds(0.5f);
            FallingWarning.SetActive(false); // 떨어지는 투사체 Anim 비활성화.
            // 떨어지는 투사체를 player위치에 맞게 변경 시킴.
            var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // 설정한 탐색 범위 내에 Target(Player)이 있는 지 확인.
            // 탐색 범위 내에 Target이 없으면 코루틴 종료.
            if(targets == null)
            {
                isFallingAttack = false;
                yield break;
            }

            var targetEntity = targets[0].GetComponent<Status>(); // player위치 : livingEntity1
            if (targetEntity && !targetEntity.isDead)
            { // 대상이 존재하고 죽지 않았을 경우.
                FallingWarning.transform.position = targetEntity.transform.position;
            }

            // 오류발견
            var attackedEntity = targets[0].GetComponent<Status>(); // player위치 : livingEntity2
            Vector3 attackPos = attackedEntity.transform.position; // 공격이 떨어지는 Player의 위치를 저장 : attackPos

            FallingWarning.SetActive(true); // 떨어지는 투사체 Anim 재생.

            yield return new WaitForSeconds(1.1f); //////////////////////////////////////////////  1초후 , 바닥에 떨어지는 시간 이후

            var targets2 = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // 설정한 탐색 범위 내에 Target(Player)이 있는 지 확인.
            if (null != targets2 && 0 < targets.Length)
            {
                var livingEntity3 = targets2[0].GetComponent<Status>(); // 1초후 player위치 : livingEntity3
                Vector3 afterPos = livingEntity3.transform.position; // 1초후 Player의 위치를 저장 : afterPos

                // 공격이 제대로 들어갔을때 (원 내부 인지 아닌지 판별)
                if ((afterPos.x - attackPos.x) * (afterPos.x - attackPos.x) + (afterPos.z - attackPos.z) * (afterPos.z - attackPos.z) < 1.7 * 1.7)
                {
                    attackedEntity.OnDamage(totalDamage, Vector3.zero, attackedEntity.transform.position);
                }

            }
            FallingWarning.SetActive(false);
            isFallingAttack = false;
        }

        public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.OnDamage(damage, hitPoint, hitNormal);
            bossHealth.value = curHealth / maxHealth; // 체력 슬라이더 부분
            Debug.Log("Boss HIt: "+ damage);
            if (assasin == 0 && curHealth <= maxHealth * 0.66f) // 체력이 66퍼 밑으로 처음 떨어질때
            {
                agent.speed = 8;
                BossMomentHide.SetActive(true);
                BossBody.SetActive(false); // 몸체만 사라짐.
                CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
                capsuleCollider.enabled = false;
                assasin = 1;
            }
            else if (assasin == 1 && curHealth <= maxHealth * 0.33f) // 체력이 33퍼 밑으로 처음 떨어질때
            {
                agent.speed = 12;
                BossMomentHide.SetActive(true);
                BossBody.SetActive(false); // 몸체만 사라짐.
                CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
                capsuleCollider.enabled = false;
                assasin = 2;
            }
        }

        protected override IEnumerator UpdatePath()
        {
            while (!isHpZero)
            {
                if (agent)
                {
                    var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // 설정한 탐색 범위 내에 Target(Player)이 있는 지 확인.

                    if (targets != null && targets.Length > 0) // 탐색 범위 내에 플레이어 오브젝트가 있는 경우
                    {
                        canvas.SetActive(true); // canvas 오브젝트 활성화
                        // 수정 필요
                        var livingEntity = targets[0].GetComponent<Status>();
                        if (livingEntity && !livingEntity.isHpZero)
                        { // 대상이 존재하고 죽지 않았을 경우.
                            var targetPos = livingEntity.transform.position;
                            agent.isStopped = false;
                            agent.SetDestination(targetPos);    // 해당 Target을 향하여 이동.
                            if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)   // 일정 거리(stoppingDistance)만큼 다가갔을 경우,
                            {
                                targetPos.y = transform.position.y;
                                var dir = (targetPos - transform.position).normalized;
                                transform.rotation = Quaternion.LookRotation(dir); //target을 향하여 바라보고,
                                StartCoroutine(Attack(livingEntity));   // 공격을 시도한다. Transform을 LivingEntity로 변경.
                            }
                            if (!isFallingAttack)
                            {
                                StartCoroutine(FallingAttack()); // 떨어지는 공격 코루틴 실행
                                isFallingAttack = true;
                            }
                        }
                    }
                    else // 탐색 범위 내에 플레이어 오브젝트가 없는 경우
                    {
                        canvas.SetActive(false); // canvas 오브젝트 비활성화
                        assasin = 0;
                        panel.SetActive(false);
                        BossMomentHide.SetActive(false);
                    }
                    // Enemy가 움직이는 속도(velocity)의 크기(magnitude)를 이용하여, 움직이는 애니메이션 처리를 한다.
                    if (anim) anim.SetFloat("Magnitude", agent.velocity.magnitude);
                } // if(agent)
                yield return new WaitForSeconds(0.04f);
            } //while()
        } // UpdatePath()

        protected override IEnumerator Attack(Status target)
        {
            if (assasin == 1 | assasin == 2) // 몸체가 다시 돌아옴.
            {
                BossMomentHide.SetActive(false);
                BossBody.SetActive(true);
                CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
                capsuleCollider.enabled = true;
                if (assasin == 2)
                {
                    panel.SetActive(true);
                }
                //assasin = 0;
            }

            if (agent && target)
            {
                var trTarget = target.transform;
                while (!isDead && !target.isHpZero)
                {
                    // 공격 모션 실행.
                    if (anim) anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(1.1f);


                    // 피격 판정 타이밍에 target이 유효한 거리에 있는지 확인.
                    if (!trTarget || Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;

                    // TODO : Player Damageable Code 추가.
                    if (isDead || target.isDead) yield break;
                    var hitNormal = transform.position - trTarget.position;
                    target.OnDamage(totalDamage, Vector3.zero, hitNormal);

                    yield return new WaitForSeconds(1.2f);

                    // 모션 종료 후, target이 유효한 거리에 있는지 확인.
                    if (!trTarget || Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                }
            }
            // target과의 거리가 벌어진다면 다시 target을 쫓아 간다.
            if (!isDead) StartCoroutine(UpdatePath());   // if(!isDead) 조건 추가.
        }
    }
}