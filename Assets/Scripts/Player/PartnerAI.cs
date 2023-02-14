using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PArtnerMode
{
    Patrol,
    Follow,
    LookTarget,
    Attack
}

public class PartnerAI : Status
{
    [SerializeField] private Gun gun; // ��
    [SerializeField] [Range(0, 100)] private float followRange;
    [SerializeField] [Range(0, 100)] private float searchRange;
    [SerializeField] private float rateOfAccuracy; // ��Ȯ��(0�� ����� ���� ��Ȯ�� ����)
    [SerializeField] private float rateOfFire; // ����ӵ�(rateOfFire�ʸ��� �߻�)
    private float currentRateOfFire; // ����ӵ� ���(���ŵ�)
    [SerializeField] private float viewAngle; // �þ߰�
    [SerializeField] private float spinSpeed; // ���� ȸ�� �ӵ�
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private ParticleSystem particle_MuzzleFlash; // �ѱ� ����
    [SerializeField] private GameObject go_HitEffect_Prefab; // ���� ȿ�� ����Ʈ

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ParticleSystem hitEffect;  // �ǰ� ����Ʈ.
    [SerializeField] private AudioClip hitSound; // �ǰ� ȿ����.
    private AudioSource audioSource;    // ȿ������ ����ϴµ� ���.
    private RaycastHit hitInfo;
    [SerializeField] private Animator anim;
    private AudioSource theAudio;
    [SerializeField] private PartnerHUD partnerHUD; // ���� ü��.

    private bool isFindTarget = false; // �� Ÿ�� �߽߰� True
    private bool isAttack = false; // ��Ȯ�� Ÿ���� ���� ���� ȸ�� �Ϸ�� True (�ѱ� ����� �� ������ ��ġ�� ��)

    private Transform tf_Target; // ���� ������ Ÿ���� Ʈ������

    // Start is called before the first frame update
    void Start()
    {
        //gun = GetComponent<Gun>();
       // anim = GetComponent<Animator>();
    }
    protected override void OnEnable()
    {
        Debug.Log("Partner On");
        base.OnEnable();    // Status�� OnEnable() ȣ��.
        //if (collider) collider.enabled = true;  // �ǰ��� ���� �� �ֵ��� collider�� Ȱ��ȭ.
        InitStatus();
        partnerHUD.SetHealthBar(GetHpRatio());
        // ������Ʈ�� Ȱ��ȭ �� ���(Respawn), target�� ã�� �̵�.
        if (agent) agent.isStopped = false;
        StartCoroutine(UpdatePath());

    }

    // Gizmo�� �̿��Ͽ� target�� ã�� ���� ������ �� �� �ִ�.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
        Gizmos.DrawWireSphere(transform.position, followRange);
    }

    // Update is called once per frame
    void Update()
    {
        SearchEnemy();
        LookTarget();
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        partnerHUD.SetHealthBar(GetHpRatio());
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
        }
    }

    private void Patrol()
    {
        if (!isFindTarget && !isAttack)
        {
            Quaternion _spin = Quaternion.Euler(0f, transform.eulerAngles.y + (1f * spinSpeed * Time.deltaTime), 0f);
            transform.rotation = _spin;
        }
    }

    private IEnumerator UpdatePath()
    {
        while (!isHpZero)
        {
            if (agent)
            {
                var targets = Physics.OverlapSphere(transform.position, followRange, playerLayer);  // ������ Ž�� ���� ���� Target(Player)�� �ִ� �� Ȯ��.
                if (null != targets && 0 < targets.Length)
                {
                    var livingEntity = targets[0].GetComponent<Status>();
                    if (livingEntity && !livingEntity.isHpZero)
                    { // ����� �����ϰ� ���� �ʾ��� ���.
                        var targetPos = livingEntity.transform.position;
                        agent.SetDestination(targetPos);    // �ش� Target�� ���Ͽ� �̵�.
                        if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)   // ���� �Ÿ�(stoppingDistance)��ŭ �ٰ����� ���,
                        {
                            //Debug.Log("Partner Stop");
                            /*                            targetPos.y = transform.position.y;
                                                        var dir = (targetPos - transform.position).normalized;
                                                        transform.rotation = Quaternion.LookRotation(dir); //target�� ���Ͽ� �ٶ󺸰�,
                                                        //StartCoroutine(Attack(livingEntity));   // ������ �õ��Ѵ�. Transform�� LivingEntity�� ����.
                                                        yield break;*/
                        }
                    }
                }
                // Enemy�� �����̴� �ӵ�(velocity)�� ũ��(magnitude)�� �̿��Ͽ�, �����̴� �ִϸ��̼� ó���� �Ѵ�.
                if (anim) anim.SetFloat("Magnitude", agent.velocity.normalized.magnitude);
            } // if(agent)
            yield return new WaitForSeconds(0.04f);
        } //while()
    } // UpdatePath()

    private void SearchEnemy()
    {
        Collider[] _target = Physics.OverlapSphere(transform.position, searchRange, targetLayer);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform; // �̰� �� ������

            if (_targetTf.tag == "Enemy")
            {
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    tf_Target = _targetTf;
                    isFindTarget = true;

                    if (_angle < 5f) // ���� ���� �ȳ���
                        isAttack = true;
                    else
                        isAttack = false;

                    return;
                }
            }
        }
        // �÷��̾� �� ã��
        tf_Target = null;
        isAttack = false;
        isFindTarget = false;
    }

    private void LookTarget()
    {
        if (isFindTarget)
        {
            Vector3 _direction = (tf_Target.position - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            Quaternion _rotation = Quaternion.Lerp(transform.rotation, _lookRotation, 0.2f);
            transform.rotation = _rotation;
            gun.Fire();
            if ((gun.GetState.Equals(State.Empty)) && gun.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.

        }
    }

    private void Attack()
    {
        if (isAttack)
        {
            currentRateOfFire += Time.deltaTime;
            if (currentRateOfFire >= rateOfFire)
            {
                currentRateOfFire = 0;
                anim.SetTrigger("Fire");
                particle_MuzzleFlash.Play();

                if (Physics.Raycast(transform.position,
                                    transform.forward + new Vector3(Random.Range(-1, 1f) * rateOfAccuracy, Random.Range(-1, 1f) * rateOfAccuracy, 0f),
                                    out hitInfo,
                                    searchRange,
                                    targetLayer))
                {
                    /*                    GameObject _HitEffect = Instantiate(go_HitEffect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                                        Destroy(_HitEffect, 1f);*/
                    gun.Fire();   // �Ѿ� �߻�.  
                                  // �Ѿ��� ������� ������ �õ�.
                    if ((gun.GetState.Equals(State.Empty)) && gun.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.

                    if (hitInfo.transform.name == "Player")
                    {
                        //hitInfo.transform.GetComponent<Status>().OnDamage(damage);
                    }
                }
            }
        }
    }
}
