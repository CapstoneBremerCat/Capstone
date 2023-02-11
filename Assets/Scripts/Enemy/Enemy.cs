using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Status
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] [Range(0, 100)] private float searchRange = 20;    // Ž�� ����
    [SerializeField] [Range(0, 100)] private float damagedSearchRange = 30; // �ǰ� �� Ž�� ����

    [SerializeField] private NavMeshAgent agent;
    private Animator anim;

    private new Collider collider;

    [SerializeField] private float health; // ���� ü��.
    [SerializeField] private float damage = 20f; // ���ݷ�.

    [SerializeField] private AudioClip deathSound;  // ��� ȿ����.
    [SerializeField] private AudioClip hitSound; // �ǰ� ȿ����.
    [SerializeField] private ParticleSystem hitEffect;  // �ǰ� ����Ʈ.
    private AudioSource audioSource;    // ȿ������ ����ϴµ� ���.

    [SerializeField] private bool isWaveEnemy;   // ���̺� ������ �Ǵ�
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

    // Gizmo�� �̿��Ͽ� target�� ã�� ���� ������ �� �� �ִ�.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider>();    // Collider�� ������ �Ű澲�� �ʴ´�.


        // ���� ������Ʈ�� AudioSource ������Ʈ �߰�
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;    // �÷��� ��, ���� ������� �ʵ��� �Ѵ�.
        OnDeath += () =>
        {
            // �� �̻� �ǰ� ������ ���� �ʰ� collider�� ����.
            //if (collider) collider.enabled = false;
            if (agent && agent.enabled) agent.isStopped = true;  // navigation ����.
            if (anim) anim.SetBool("isDead", isDead);   // Zombie Death �ִϸ��̼� ����.
            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);     // ��� ȿ���� 1ȸ ���.
            if (GameManager.Instance)
            {
                GameManager.Instance.AddScore(100); // enemy óġ ��, 100 score ���.
                if (isWaveEnemy) GameManager.Instance.DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
            }
            //EnemyMgr.Instance.DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
            //gameObject.SetActive(false);
        };
    }
    protected override void OnEnable()
    {
        base.OnEnable();    // Status�� OnEnable() ȣ��.
        if (collider) collider.enabled = true;  // �ǰ��� ���� �� �ֵ��� collider�� Ȱ��ȭ.
        InitStatus();
        health = curHealth;
        // ������Ʈ�� Ȱ��ȭ �� ���(Respawn), target�� ã�� �̵�.
        if (agent) agent.isStopped = false;
        isDamaged = false;
        StartCoroutine(UpdatePath());
    }
/*    protected override void OnEnable()
    {
        base.OnEnable();    // LivingEntity�� OnEnable() ȣ��.

        if (anim) anim.SetBool("isDead", isDead);   // ��� ���¸� false, isDead=false/
        if (collider) collider.enabled = true;  // �ǰ��� ���� �� �ֵ��� collider�� Ȱ��ȭ.
        // ������Ʈ�� Ȱ��ȭ �� ���(Respawn), target�� ã�� �̵�.
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
                hitEffectTR.position = hitPoint;    // ����Ʈ�� �ǰ� �������� �̵�.
                // �ǰ� ���� �������� ȸ��.
                hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play();   // ����Ʈ ���.
            }

            // �ǰ� ȿ���� 1ȸ ���.
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
            anim.SetTrigger("Damaged"); // �������� �԰� ���� �ʾҴٸ�, �ǰ� �ִϸ��̼� ����.
            if(!isDamaged) StartCoroutine(DamagedReact()); // ���ظ� ���� ���� ������ ��� �ǰ� �ڷ�ƾ ����
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

    private IEnumerator UpdatePath()
    {
        while (!isHpZero)
        {
            if (agent)
            {
                var targets = Physics.OverlapSphere(transform.position, searchRange, targetLayer);  // ������ Ž�� ���� ���� Target(Player)�� �ִ� �� Ȯ��.
                if (null != targets && 0 < targets.Length)
                {
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

    private IEnumerator Attack(Status target)
    {
        if (agent && target)
        {
            var trTarget = target.transform;
            while (!isHpZero && !target.isHpZero)
            {
                // ���� ��� ����.
                if (anim) anim.SetTrigger("Attack");
                yield return new WaitForSeconds(1.1f);

                // �ǰ� ���� Ÿ�ֿ̹� target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.
                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;

                // TODO : Player Damageable Code �߰�.
                if (isHpZero || target.isHpZero) yield break;
                var hitNormal = transform.position - trTarget.position;
                target.OnDamage(damage, Vector3.zero, hitNormal);

                yield return new WaitForSeconds(1.2f);

                // ��� ���� ��, target�� ��ȿ�� �Ÿ��� �ִ��� Ȯ��.
                if (Vector3.Distance(trTarget.position, transform.position) > agent.stoppingDistance) break;
            }
        }
        // target���� �Ÿ��� �������ٸ� �ٽ� target�� �Ѿ� ����.
        if (!isHpZero) StartCoroutine(UpdatePath());   // if(!isDead) ���� �߰�.
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
