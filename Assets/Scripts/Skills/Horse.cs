using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;

public enum SkillState
{
    Ready
}

public class Horse : MonoBehaviour
{
    private Transform pivot;
    private SkillState state;
    private bool isReady;   // 스킬 사용이 가능한 상태인지 확인
    private bool isCharging;   // 차지 공격 중인지 확인
    private int currentAlly;  //현재 진영
    [SerializeField] private string currentEnemy;  // 적 진영
    [SerializeField] private LayerMask obstacle;  // 장애물

    private float lastFireTime; // 총을 마지막으로 발사한 시점.
    private LineRenderer bulletLineRenderer;    // 총알 궤적을 그리기 위한 도구.

    [SerializeField] private float skillCoolTime = 3.0f;   // 쿨타임.
    [SerializeField] private float damage = 100;   // 무기의 공격력
    [SerializeField] private float effectiveDistance;  // 사정거리.
    private Vector3 originPos;  // 원래 위치

    [Header("SFX")]
    [SerializeField] private ParticleSystem muzzleFlashEffect;  // 총구의 화염 효과.
    [SerializeField] private AudioClip shootSound;  // 총 발포 효과음.
    [SerializeField] private AudioClip reloadSound; // 탄창 재장전 효과음.
    private AudioSource audioSource;

    [SerializeField] private float force = 10f;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        bulletLineRenderer = GetComponent<LineRenderer>();
        if (bulletLineRenderer)
        {
            bulletLineRenderer.positionCount = 2; // 선을 그리기 위한 두 점을 설정.
            bulletLineRenderer.enabled = false;   // 총을 쏘기 전까지 궤적이 보이지 않도록 비활성화.
        }

    }

    private void OnEnable()
    {
        isReady = true;
        lastFireTime = Time.time - skillCoolTime;
        // 첫 위치 등록
        originPos = transform.localPosition;
        currentAlly = gameObject.layer;
        // 피아 식별
        if (currentAlly == LayerMask.NameToLayer("Enemy"))
        {
            currentEnemy = "Player";
        }
        else
        {
            currentEnemy = "Enemy";
        }
    }
    
    private void Update()
    {
        // 적인 경우
        if (currentAlly == LayerMask.NameToLayer("Enemy"))
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AttackCharge();
            }
        }
    }

    public void AttackCharge()
    {
        // 현재 스킬을 사용할 수 있는 상태인지 확인.
        if (isReady && Time.time >= lastFireTime + skillCoolTime)    
        {
            lastFireTime = Time.time;   // 사용 시점 갱신.
            StartCoroutine(AttackChargeCoroutine());
        }
    }

    IEnumerator AttackChargeCoroutine()
    {
        isReady = false;    // 준비 상태를 false로 변경
        isCharging = true;    // 차지 상태 활성화

        RaycastHit hit; // Physics.RayCast()를 이용하여 충돌 지점 정보를 알아온다.
        Vector3 hitPos = transform.position + transform.forward * effectiveDistance; // 충돌 위치를 저장, 최대 거리 위치를 기본 값으로 가진다.

        // 장애물 직전에서 멈추도록 한다.
        if (Physics.Raycast(transform.position, transform.forward, out hit, effectiveDistance, obstacle))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            hitPos = hit.point; // 실제 충돌이 일어난 지점으로 갱신.
        }

/*        // 돌진 준비
        while (transform.localPosition.z < effectiveDistance)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hitPos, 0.3f);
            yield return null;
        }*/

        // 돌진 시작
        while (transform.position.z < hitPos.z - 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, hitPos, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            yield return null;
        }
        isCharging = false;    //  차지 상태 비활성화.
        isReady = true;    //  준비된 상태로 변경.

    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        if (muzzleFlashEffect) muzzleFlashEffect.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        yield return new WaitForSeconds(0.03f); // 0.03초 동안 잠시 처리를 대기.
        if (bulletLineRenderer) bulletLineRenderer.enabled = false; // LineRenderer를 비활성화하여 총알 궤적을 지운다.
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.layer);
        //Debug.Log(collision.gameObject.layer);
        // 차지 중 적과 닿으면 해당 적에게 피해를 준다.
        if(isCharging && collision.gameObject.layer == LayerMask.NameToLayer(currentEnemy))
        {
            IDamageable entity = collision.gameObject.GetComponent<IDamageable>();
            var navMesh = collision.gameObject.GetComponent<NavMeshAgent>();
            // 충돌지점 데이터 받아오기
            navMesh.enabled = false;
            ContactPoint contact = collision.contacts[0];
            if (null != entity) entity.OnDamage(damage, contact.point, contact.normal);

            Rigidbody body = collision.gameObject.GetComponent<Rigidbody>();
            if (body != null)
            {
                StartCoroutine(ApplyForce(body, transform.forward, force));
            }
        }
    }

    public IEnumerator ApplyForce(Rigidbody body, Vector3 direction, float force)
    {
        var time = 0.1f;
        var duration = 0.5f;
        while (duration > 0)
        {
            body.velocity = direction * force;
            yield return new WaitForSeconds(time);
            duration -= time;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        // 차지 중 적과 닿으면 해당 적에게 피해를 준다.
        if (isCharging && other.gameObject.layer == LayerMask.NameToLayer(currentEnemy))
        {
            IDamageable entity = other.gameObject.GetComponent<IDamageable>();
            var navMesh = other.gameObject.GetComponent<NavMeshAgent>();
            // 충돌지점 데이터 받아오기
            navMesh.enabled = false;
            Vector3 position = other.ClosestPointOnBounds(transform.position);
            Vector3 normal = (position - transform.position).normalized;
            if (null != entity) entity.OnDamage(damage, position, normal);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(normal * force, ForceMode.Impulse);
            }
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
