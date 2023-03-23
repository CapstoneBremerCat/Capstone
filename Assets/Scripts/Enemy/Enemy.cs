using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game;
namespace Game
{
    public class Enemy : Status
    {
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] [Range(0, 100)] protected float searchRange = 20;    // 탐색 범위
        [SerializeField] [Range(0, 100)] protected float damagedSearchRange = 30; // 피격 시 탐색 범위

        [SerializeField] protected NavMeshAgent agent;
        protected Animator anim;

        private new Collider collider;  

        [SerializeField] protected AudioClip deathSound;  // 사망 효과음.
        [SerializeField] protected AudioClip hitSound; // 피격 효과음.
        [SerializeField] protected ParticleSystem hitEffect;  // 피격 이펙트.
        protected AudioSource audioSource;    // 효과음을 출력하는데 사용.

        [SerializeField] private bool isWaveEnemy;   // 웨이브 적인지 판단
        public bool isDamaged { get; private set; }

        public void Setup(float damage, float maxhealth, float speed, Vector3 pos)
        {
            totalStat.damage = damage;
            if (agent) agent.speed = speed;
            transform.position = pos;
            gameObject.SetActive(true);
        }

        // Gizmo를 이용하여 target을 찾는 가시 범위를 벌 수 있다.
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, searchRange);
        }

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            collider = GetComponent<Collider>();    // Collider의 종류를 신경쓰지 않는다.


            // 현재 오브젝트에 AudioSource 컴포넌트 추가
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;    // 플레이 시, 사운드 실행되지 않도록 한다.
            OnDeath += () =>
            {
            // 더 이상 피격 판정이 되지 않게 collider를 끈다.
            //if (collider) collider.enabled = false;
            if (agent && agent.enabled) agent.isStopped = true;  // navigation 정지.
            if (anim) anim.SetBool("isDead", isDead);   // Zombie Death 애니메이션 실행.
            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);     // 사망 효과음 1회 재생.
            if (GameManager.Instance) GameManager.Instance.KillEnemy(isWaveEnemy);
            //gameObject.SetActive(false);
            if (ItemMgr.Instance) ItemMgr.Instance.SpawnItem(transform.position + Vector3.up);
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();    // Status의 OnEnable() 호출.
            if (collider) collider.enabled = true;  // 피격을 받을 수 있도록 collider를 활성화.
            InitStatus();
            // 오브젝트가 활성화 될 경우(Respawn), target을 찾아 이동.
            if (agent) agent.isStopped = false;
            isDamaged = false;
            StartCoroutine(UpdatePath());
        }

        public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.OnDamage(damage, hitPoint, hitNormal);
            if (anim && !isDead)
            {
                if (hitEffect)
                {
                    var hitEffectTR = hitEffect.transform;
                    hitEffectTR.position = hitPoint;    // 이펙트를 피격 지점으로 이동.
                                                        // 피격 당한 방향으로 회전.
                    hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                    hitEffect.Play();   // 이펙트 재생.
                }

                // 피격 효과음 1회 재생.
                if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
                anim.SetTrigger("Damaged"); // 데미지를 입고 죽지 않았다면, 피격 애니메이션 실행.
                if (!isDamaged) StartCoroutine(DamagedReact()); // 피해를 입지 않은 상태일 경우 피격 코루틴 실행
            }
        }

        private IEnumerator DamagedReact()
        {
            var originRange = searchRange;

            isDamaged = true;
            searchRange = damagedSearchRange;   // 피격 시 탐색범위 증가
            yield return new WaitForSeconds(3.0f);
            agent.isStopped = true;
            searchRange = originRange;  // 일정 시간이 지나면 탐색범위 복구
            isDamaged = false;  // 피격 상태 초기화
        }

        public void UnactiveObject() // Zombie Death 실행 후 호출하여 오브젝트를 비활성화 시킨다.
        {
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator UpdatePath()
        {
            while (!isHpZero)
            {
                if (agent)
                {
                    var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // 설정한 탐색 범위 내에 Target(Player)이 있는 지 확인.
                    if (null != targets && 0 < targets.Length)
                    {
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
                                yield break;
                            }
                        }
                    }
                    // Enemy가 움직이는 속도(velocity)의 크기(magnitude)를 이용하여, 움직이는 애니메이션 처리를 한다.
                    if (anim) anim.SetFloat("Magnitude", agent.velocity.magnitude);
                } // if(agent)
                yield return new WaitForSeconds(0.04f);
            } //while()
        } // UpdatePath()

        protected virtual IEnumerator Attack(Status target)
        {
            if (agent && target)
            {
                var trTarget = target.transform;
                while (!isDead && !target.isDead)
                {
                    // 공격 모션 실행.
                    if (anim) anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(1.1f);

                    // 피격 판정 타이밍에 target이 유효한 거리에 있는지 확인.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;

                    // TODO : Player Damageable Code 추가.
                    if (isDead || target.isDead) yield break;
                    var hitNormal = transform.position - trTarget.position;
                    target.OnDamage(totalDamage, Vector3.zero, hitNormal);

                    yield return new WaitForSeconds(1.2f);

                    // 모션 종료 후, target이 유효한 거리에 있는지 확인.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                }
            }
            // target과의 거리가 벌어진다면 다시 target을 쫓아 간다.
            if (!isDead) StartCoroutine(UpdatePath());   // if(!isDead) 조건 추가.
        }

        private void OnCollisionEnter(Collision collision)
        {
            IDamageable target = collision.collider.GetComponent<IDamageable>();
            if (null != target) target.OnDamage(totalDamage, collision.transform.position, Vector3.zero);
        }

        private void OnDisable()
        {
            //if (EnemyMgr.Instance) EnemyMgr.Instance.SetPooling(this);
        }
    }
}