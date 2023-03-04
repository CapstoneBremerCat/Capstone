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
    private bool isReady;   // ��ų ����� ������ �������� Ȯ��
    private bool isCharging;   // ���� ���� ������ Ȯ��
    private int currentAlly;  //���� ����
    [SerializeField] private string currentEnemy;  // �� ����
    [SerializeField] private LayerMask obstacle;  // ��ֹ�

    private float lastFireTime; // ���� ���������� �߻��� ����.
    private LineRenderer bulletLineRenderer;    // �Ѿ� ������ �׸��� ���� ����.

    [SerializeField] private float skillCoolTime = 3.0f;   // ��Ÿ��.
    [SerializeField] private float damage = 100;   // ������ ���ݷ�
    [SerializeField] private float effectiveDistance;  // �����Ÿ�.
    private Vector3 originPos;  // ���� ��ġ

    [Header("SFX")]
    [SerializeField] private ParticleSystem muzzleFlashEffect;  // �ѱ��� ȭ�� ȿ��.
    [SerializeField] private AudioClip shootSound;  // �� ���� ȿ����.
    [SerializeField] private AudioClip reloadSound; // źâ ������ ȿ����.
    private AudioSource audioSource;

    [SerializeField] private float force = 10f;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        bulletLineRenderer = GetComponent<LineRenderer>();
        if (bulletLineRenderer)
        {
            bulletLineRenderer.positionCount = 2; // ���� �׸��� ���� �� ���� ����.
            bulletLineRenderer.enabled = false;   // ���� ��� ������ ������ ������ �ʵ��� ��Ȱ��ȭ.
        }

    }

    private void OnEnable()
    {
        isReady = true;
        lastFireTime = Time.time - skillCoolTime;
        // ù ��ġ ���
        originPos = transform.localPosition;
        currentAlly = gameObject.layer;
        // �Ǿ� �ĺ�
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
        // ���� ���
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
        // ���� ��ų�� ����� �� �ִ� �������� Ȯ��.
        if (isReady && Time.time >= lastFireTime + skillCoolTime)    
        {
            lastFireTime = Time.time;   // ��� ���� ����.
            StartCoroutine(AttackChargeCoroutine());
        }
    }

    IEnumerator AttackChargeCoroutine()
    {
        isReady = false;    // �غ� ���¸� false�� ����
        isCharging = true;    // ���� ���� Ȱ��ȭ

        RaycastHit hit; // Physics.RayCast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
        Vector3 hitPos = transform.position + transform.forward * effectiveDistance; // �浹 ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.

        // ��ֹ� �������� ���ߵ��� �Ѵ�.
        if (Physics.Raycast(transform.position, transform.forward, out hit, effectiveDistance, obstacle))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            hitPos = hit.point; // ���� �浹�� �Ͼ �������� ����.
        }

/*        // ���� �غ�
        while (transform.localPosition.z < effectiveDistance)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hitPos, 0.3f);
            yield return null;
        }*/

        // ���� ����
        while (transform.position.z < hitPos.z - 0.02f)
        {
            transform.position = Vector3.Lerp(transform.position, hitPos, 0.1f);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            yield return null;
        }
        isCharging = false;    //  ���� ���� ��Ȱ��ȭ.
        isReady = true;    //  �غ�� ���·� ����.

    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        if (muzzleFlashEffect) muzzleFlashEffect.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        yield return new WaitForSeconds(0.03f); // 0.03�� ���� ��� ó���� ���.
        if (bulletLineRenderer) bulletLineRenderer.enabled = false; // LineRenderer�� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����.
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.layer);
        //Debug.Log(collision.gameObject.layer);
        // ���� �� ���� ������ �ش� ������ ���ظ� �ش�.
        if(isCharging && collision.gameObject.layer == LayerMask.NameToLayer(currentEnemy))
        {
            IDamageable entity = collision.gameObject.GetComponent<IDamageable>();
            var navMesh = collision.gameObject.GetComponent<NavMeshAgent>();
            // �浹���� ������ �޾ƿ���
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
        // ���� �� ���� ������ �ش� ������ ���ظ� �ش�.
        if (isCharging && other.gameObject.layer == LayerMask.NameToLayer(currentEnemy))
        {
            IDamageable entity = other.gameObject.GetComponent<IDamageable>();
            var navMesh = other.gameObject.GetComponent<NavMeshAgent>();
            // �浹���� ������ �޾ƿ���
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
