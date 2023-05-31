using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Game;
namespace Game
{
    public class Enemy : Status
    {
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] [Range(0, 100)] protected float searchRange = 20;    // Ž�� ����
        [SerializeField] [Range(0, 100)] protected float damagedSearchRange = 30; // �ǰ� �� Ž�� ����

        [SerializeField] protected Slider slider;
        [SerializeField] protected NavMeshAgent agent;
        protected Animator anim;

        private new Collider collider;  

        [SerializeField] protected AudioClip deathSound;  // ��� ȿ����.
        [SerializeField] protected AudioClip hitSound; // �ǰ� ȿ����.
        [SerializeField] protected ParticleSystem hitEffect;  // �ǰ� ����Ʈ.
        protected AudioSource audioSource;    // ȿ������ ����ϴµ� ���.
        private int maxPlayingSounds = 5; // ���� ����� ����� �������� �ִ� ����
        [SerializeField] private bool isWaveEnemy;   // ���̺� ������ �Ǵ�
        public bool isDamaged { get; private set; }

        public void Setup(float damage, float maxhealth, float speed, Vector3 pos)
        {
            totalStat.damage = damage;
            if (agent) agent.speed = speed;
            agent.enabled = false;
            transform.position = pos;
            agent.enabled = true;
            gameObject.SetActive(true);

        }

        // Gizmo�� �̿��Ͽ� target�� ã�� ���� ������ �� �� �ִ�.
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, searchRange);
        }

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            collider = GetComponent<Collider>();    // Collider�� ������ �Ű澲�� �ʴ´�.
            if(slider) slider.gameObject.SetActive(false);

            // ���� ������Ʈ�� AudioSource ������Ʈ �߰�
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;    // �÷��� ��, ���� ������� �ʵ��� �Ѵ�.
            OnDeath += () =>
            {
                StopAllCoroutines();
                if (slider) slider.value = GetHpRatio();
                // �� �̻� �ǰ� ������ ���� �ʰ� collider�� ����.
                if (collider) collider.enabled = false;
                if (agent && agent.enabled)
                {
                    //agent.isStopped = true;  // navigation ����.
                    agent.enabled = false;  // navigation ����.
                }
                if (anim) anim.SetBool("isDead", isDead);   // Zombie Death �ִϸ��̼� ����.
                // ��� ȿ���� 1ȸ ���.
                if (audioSource && deathSound)
                {
                    PlayOneShotWithLimit(deathSound);
                }
                if (GameManager.Instance) GameManager.Instance.KillEnemy(isWaveEnemy);
                //gameObject.SetActive(false);
                if (ItemMgr.Instance) ItemMgr.Instance.SpawnItem(transform.position + Vector3.up);
            };
        }
        public void PlayOneShotWithLimit(AudioClip clip)
        {
            if (SoundManager.Instance.curPlayingSounds < maxPlayingSounds)
            {
                SoundManager.Instance.AddPlayingSounds();
                audioSource.volume = SoundManager.Instance.SFXSoundVolume; // ���ϴ� ���� ������ ����
                audioSource.mute = SoundManager.Instance.SFXSoundMute; // ���ϴ� ���� ������ ����
                audioSource.PlayOneShot(clip);
                StartCoroutine(ResetAfterClipLength(clip.length));
            }
        }

        private IEnumerator ResetAfterClipLength(float clipLength)
        {
            yield return new WaitForSeconds(clipLength);
            // �ʱ�ȭ �۾� ����
            SoundManager.Instance.InitPlayingSounds();
        }

        protected override void OnEnable()
        {
            base.OnEnable();    // Status�� OnEnable() ȣ��.
            if (collider) collider.enabled = true;  // �ǰ��� ���� �� �ֵ��� collider�� Ȱ��ȭ.
            InitStatus();
            isDamaged = false;
            StartCoroutine(UpdatePath());
        }

        public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if(slider) slider.gameObject.SetActive(true);
            
            base.OnDamage(damage, hitPoint, hitNormal);
            if (anim && !isDead)
            {
                if (hitEffect)
                {
                    var hitEffectTR = hitEffect.transform;
                    hitEffectTR.position = hitPoint;    // ����Ʈ�� �ǰ� �������� �̵�.
                                                        // �ǰ� ���� �������� ȸ��.
                    hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                    hitEffect.Play();   // ����Ʈ ���.
                }

                // �ǰ� ȿ���� 1ȸ ���.
                if (audioSource && hitSound)
                {
                    PlayOneShotWithLimit(hitSound);
                }
                anim.SetTrigger("Damaged"); // �������� �԰� ���� �ʾҴٸ�, �ǰ� �ִϸ��̼� ����.
                if (!isDamaged) StartCoroutine(DamagedReact()); // ���ظ� ���� ���� ������ ��� �ǰ� �ڷ�ƾ ����
            }
        }

        private IEnumerator DamagedReact()
        {
            var originRange = searchRange;

            isDamaged = true;
            searchRange = damagedSearchRange;   // �ǰ� �� Ž������ ����
            yield return new WaitForSeconds(3.0f);
            searchRange = originRange;  // ���� �ð��� ������ Ž������ ����
            isDamaged = false;  // �ǰ� ���� �ʱ�ȭ
        }

        public void UnactiveObject() // Zombie Death ���� �� ȣ���Ͽ� ������Ʈ�� ��Ȱ��ȭ ��Ų��.
        {
            gameObject.SetActive(false);
        }

        protected virtual IEnumerator UpdatePath()
        {
            agent.enabled = true;
            while (!isHpZero)
            {
                if (slider) slider.value = GetHpRatio();
                if (agent)
                {
                    var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // ������ Ž�� ���� ���� Target(Player)�� �ִ� �� Ȯ��.
                    if (null != targets && 0 < targets.Length)
                    {
                        // ���� �ʿ�
                        var livingEntity = targets[0].GetComponent<Status>();
                        if (livingEntity && !livingEntity.isHpZero)
                        { // ����� �����ϰ� ���� �ʾ��� ���.
                            var targetPos = livingEntity.transform.position;
                            agent.SetDestination(targetPos);    // �ش� Target�� ���Ͽ� �̵�.
                            if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)   // ���� �Ÿ�(stoppingDistance)��ŭ �ٰ����� ���,
                            {
                                targetPos.y = transform.position.y;
                                var dir = (targetPos - transform.position).normalized;
                                transform.rotation = Quaternion.LookRotation(dir); //target�� ���Ͽ� �ٶ󺸰�,
                                StartCoroutine(Attack(livingEntity));   // ������ �õ��Ѵ�. Transform�� LivingEntity�� ����.
                                yield break;
                            }
                        }
                    }
                    // Enemy�� �����̴� �ӵ�(velocity)�� ũ��(magnitude)�� �̿��Ͽ�, �����̴� �ִϸ��̼� ó���� �Ѵ�.
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
                    // ���� ��� ����.
                    if (anim) anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(1.1f);

                    // �ǰ� ���� Ÿ�ֿ̹� target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;

                    // TODO : Player Damageable Code �߰�.
                    if (isDead || target.isDead) yield break;
                    var hitNormal = transform.position - trTarget.position;
                    target.OnDamage(totalDamage, Vector3.zero, hitNormal);

                    yield return new WaitForSeconds(1.2f);

                    // ��� ���� ��, target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.
                    if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
                }
            }
            // target���� �Ÿ��� �������ٸ� �ٽ� target�� �Ѿ� ����.
            if (!isDead) StartCoroutine(UpdatePath());   // if(!isDead) ���� �߰�.
        }

        private void OnCollisionEnter(Collision collision)
        {
            IDamageable target = collision.collider.GetComponent<IDamageable>();
            if (null != target) target.OnDamage(totalDamage, collision.transform.position, Vector3.zero);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            //if (EnemyMgr.Instance) EnemyMgr.Instance.SetPooling(this);
        }
    }
}