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
    [SerializeField] private Gun gun; // 총
    [SerializeField] [Range(0, 100)] private float searchRange = 20;
    [SerializeField] private float rateOfAccuracy; // 정확도(0에 가까울 수록 정확도 높음)
    [SerializeField] private float rateOfFire; // 연사속도(rateOfFire초마다 발사)
    private float currentRateOfFire; // 연사속도 계산(갱신됨)
    [SerializeField] private float viewAngle; // 시야각
    [SerializeField] private float spinSpeed; // 포신 회전 속도
    [SerializeField] private LayerMask layerMask; // 움직이는 대상만 타겟으로 지정(플레이어 혹은 동물)
    [SerializeField] private Transform tf_TopGun; // 포신
    [SerializeField] private ParticleSystem particle_MuzzleFlash; // 총구 섬광
    [SerializeField] private GameObject go_HitEffect_Prefab; // 적중 효과 이펙트

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float health; // 현재 체력.
    [SerializeField] private ParticleSystem hitEffect;  // 피격 이펙트.
    [SerializeField] private AudioClip hitSound; // 피격 효과음.
    private AudioSource audioSource;    // 효과음을 출력하는데 사용.
    private RaycastHit hitInfo;
    private Animator anim;
    private AudioSource theAudio;

    private bool isFindTarget = false; // 적 타겟 발견시 True
    private bool isAttack = false; // 정확히 타겟을 향해 포신 회전 완료시 True (총구 방향과 적 방향이 일치할 때)

    private Transform tf_Target; // 현재 설정된 타겟의 트랜스폼

    // Start is called before the first frame update
    void Start()
    {
        gun = GetComponent<Gun>();
        anim = GetComponent<Animator>();
    }
    protected override void OnEnable()
    {
        Debug.Log("Partner On");
        base.OnEnable();    // Status의 OnEnable() 호출.
        //if (collider) collider.enabled = true;  // 피격을 받을 수 있도록 collider를 활성화.
        InitStatus();
        health = curHealth;
        // 오브젝트가 활성화 될 경우(Respawn), target을 찾아 이동.
        if (agent) agent.isStopped = false;
        StartCoroutine(UpdatePath());
    }

    // Gizmo를 이용하여 target을 찾는 가시 범위를 벌 수 있다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }

    // Update is called once per frame
    void Update()
    {

    }

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
        }
    }

    private void Patrol()
    {
        if (!isFindTarget && !isAttack)
        {
            Quaternion _spin = Quaternion.Euler(0f, tf_TopGun.eulerAngles.y + (1f * spinSpeed * Time.deltaTime), 0f);
            tf_TopGun.rotation = _spin;
        }
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
                    var livingEntity = targets[0].GetComponent<Status>();
                    if (livingEntity && !livingEntity.isHpZero)
                    { // 대상이 존재하고 죽지 않았을 경우.
                        var targetPos = livingEntity.transform.position;
                        agent.SetDestination(targetPos);    // 해당 Target을 향하여 이동.
                        if (Vector3.Distance(targetPos, transform.position) <= agent.stoppingDistance)   // 일정 거리(stoppingDistance)만큼 다가갔을 경우,
                        {
                            Debug.Log("Partner Stop");
/*                            targetPos.y = transform.position.y;
                            var dir = (targetPos - transform.position).normalized;
                            transform.rotation = Quaternion.LookRotation(dir); //target을 향하여 바라보고,
                            //StartCoroutine(Attack(livingEntity));   // 공격을 시도한다. Transform을 LivingEntity로 변경.
                            yield break;*/
                        }
                    }
                }
                // Enemy가 움직이는 속도(velocity)의 크기(magnitude)를 이용하여, 움직이는 애니메이션 처리를 한다.
                if (anim) anim.SetFloat("Magnitude", agent.velocity.magnitude);
            } // if(agent)
            yield return new WaitForSeconds(0.04f);
        } //while()
    } // UpdatePath()

    private void SearchEnemy()
    {
        Collider[] _target = Physics.OverlapSphere(tf_TopGun.position, searchRange, layerMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform; // 이게 더 빠르다

            if (_targetTf.name == "Player")
            {
                Vector3 _direction = (_targetTf.position - tf_TopGun.position).normalized;
                float _angle = Vector3.Angle(_direction, tf_TopGun.forward);

                if (_angle < viewAngle * 0.5f)
                {
                    tf_Target = _targetTf;
                    isFindTarget = true;

                    if (_angle < 5f) // 거의 차이 안나면
                        isAttack = true;
                    else
                        isAttack = false;

                    return;
                }
            }
        }
        // 플레이어 못 찾음
        tf_Target = null;
        isAttack = false;
        isFindTarget = false;
    }

    private void LookTarget()
    {
        if (isFindTarget)
        {
            Vector3 _direction = (tf_Target.position - tf_TopGun.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            Quaternion _rotation = Quaternion.Lerp(tf_TopGun.rotation, _lookRotation, 0.2f);
            tf_TopGun.rotation = _rotation;
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

                if (Physics.Raycast(tf_TopGun.position,
                                    tf_TopGun.forward + new Vector3(Random.Range(-1, 1f) * rateOfAccuracy, Random.Range(-1, 1f) * rateOfAccuracy, 0f),
                                    out hitInfo,
                                    searchRange,
                                    layerMask))
                {
                    GameObject _HitEffect = Instantiate(go_HitEffect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Destroy(_HitEffect, 1f);

                    if (hitInfo.transform.name == "Player")
                    {
                        //hitInfo.transform.GetComponent<Status>().OnDamage(damage);
                    }
                }
            }
        }
    }
}
