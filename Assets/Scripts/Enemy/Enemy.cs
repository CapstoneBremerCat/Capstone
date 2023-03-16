using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Game;
namespace Game
{
    public class Enemy : Status
    {
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] [Range(0, 100)] private float searchRange = 20;    // 탐색 범위
        [SerializeField] [Range(0, 100)] private float damagedSearchRange = 30; // 피격 시 탐색 범위

        [SerializeField] private NavMeshAgent agent;
        private Animator anim;

        private new Collider collider;

        [SerializeField] private float health; // 현재 체력.
        [SerializeField] private float damage = 20f; // 공격력.

        [SerializeField] private AudioClip deathSound;  // 사망 효과음.
        [SerializeField] private AudioClip hitSound; // 피격 효과음.
        [SerializeField] private ParticleSystem hitEffect;  // 피격 이펙트.
        private AudioSource audioSource;    // 효과음을 출력하는데 사용.

        [SerializeField] private bool isWaveEnemy;   // 웨이브 적인지 판단
        private bool isDamaged = false;

        //[SerializeField] private Renderer enemyRenderer;

        public void Setup(float damage, float maxhealth, float speed, Color color, Vector3 pos)
        {
            this.damage = damage;
            //this.maxHealth = maxHealth;
            if (agent) agent.speed = speed;
            //if (enemyRenderer) enemyRenderer.material.color = color;
            transform.position = pos;
            gameObject.SetActive(true);
        }

        // Gizmo를 이용하여 target을 찾는 가시 범위를 벌 수 있다.
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, searchRange);
        }

        private void Awake()
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
            //EnemyMgr.Instance.DecreaseSpawnCount(); // enemy 처치 시, Spawn Count 감소.
            //gameObject.SetActive(false);
            if (ItemMgr.Instance) ItemMgr.Instance.SpawnItem(transform.position + Vector3.up);
            };
        }

        protected override void OnEnable()
        {
            base.OnEnable();    // Status의 OnEnable() 호출.
            if (collider) collider.enabled = true;  // 피격을 받을 수 있도록 collider를 활성화.
            InitStatus();
            health = curHealth;
            // 오브젝트가 활성화 될 경우(Respawn), target을 찾아 이동.
            if (agent) agent.isStopped = false;
            isDamaged = false;
            StartCoroutine(UpdatePath());
        }
        /*    protected override void OnEnable()
            {
                base.OnEnable();    // LivingEntity의 OnEnable() 호출.

                if (anim) anim.SetBool("isDead", isDead);   // 사망 상태를 false, isDead=false/
                if (collider) collider.enabled = true;  // 피격을 받을 수 있도록 collider를 활성화.
                // 오브젝트가 활성화 될 경우(Respawn), target을 찾아 이동.
                if (agent) agent.isStopped = false;
                StartCoroutine(UpdatePath());
            }*/

        public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            base.OnDamage(damage, hitPoint, hitNormal);
            health = curHealth;
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
            searchRange = originRange;  // 일정 시간이 지나면 탐색범위 복구
            isDamaged = false;  // 피격 상태 초기화
        }

        public void UnactiveObject() // Zombie Death 실행 후 호출하여 오브젝트를 비활성화 시킨다.
        {
            gameObject.SetActive(false);
        }

        private IEnumerator UpdatePath()
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

        private IEnumerator Attack(Status target)
        {
            if (agent && target)
            {
                var trTarget = target.transform;
                while (!isHpZero && !target.isHpZero)
                {
                    // 공격 모션 실행.
                    if (anim) anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(1.1f);

                    // 피격 판정 타이밍에 target이 유효한 거리에 있는지 확인.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;

                    // TODO : Player Damageable Code 추가.
                    if (isHpZero || target.isHpZero) yield break;
                    var hitNormal = transform.position - trTarget.position;
                    target.OnDamage(damage, Vector3.zero, hitNormal);

                    yield return new WaitForSeconds(1.2f);

                    // 모션 종료 후, target이 유효한 거리에 있는지 확인.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                }
            }
            // target과의 거리가 벌어진다면 다시 target을 쫓아 간다.
            if (!isHpZero) StartCoroutine(UpdatePath());   // if(!isDead) 조건 추가.
        }

        private void OnCollisionEnter(Collision collision)
        {
            IDamageable target = collision.collider.GetComponent<IDamageable>();
            if (null != target) target.OnDamage(damage, collision.transform.position, Vector3.zero);
        }

        private void OnDisable()
        {
            //if (EnemyMgr.Instance) EnemyMgr.Instance.SetPooling(this);
        }
    }
}