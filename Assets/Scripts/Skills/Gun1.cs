using System;
using System.Collections;
using UnityEngine;

// ���� �����Ѵ�
public class Gun1 : MonoBehaviour
{
    // ���� ���¸� ǥ���ϴµ� ����� Ÿ���� �����Ѵ�
    public enum State
    {
        Ready, // �߻� �غ��
        Empty, // źâ�� ��
        Reloading // ������ ��
    }
    public State state { get; private set; } // ���� ���� ����

    private PlayerShooter gunHolder;
    private LineRenderer bulletLineRenderer; // �Ѿ� ������ �׸��� ���� ������

    private AudioSource gunAudioPlayer; // �� �Ҹ� �����
    public AudioClip shotClip; // �߻� �Ҹ�
    public AudioClip reloadClip; // ������ �Ҹ�

    public ParticleSystem muzzleFlashEffect; // �ѱ� ȭ�� ȿ��
    public ParticleSystem shellEjectEffect; // ź�� ���� ȿ��

    public Transform fireTransform; // �Ѿ��� �߻�� ��ġ
    public Transform leftHandMount;

    public float damage = 25; // ���ݷ�
    public float fireDistance = 100f; // �����Ÿ�

    public int ammoRemain = 100; // ���� ��ü ź��
    public int magAmmo; // ���� źâ�� �����ִ� ź��
    public int magCapacity = 30; // źâ �뷮

    public float timeBetFire = 0.12f; // �Ѿ� �߻� ����
    public float reloadTime = 1.8f; // ������ �ҿ� �ð�

    [Range(0f, 10f)] public float maxSpread = 3f;
    [Range(1f, 10f)] public float stability = 1f;
    [Range(0.01f, 3f)] public float restoreFromRecoilSpeed = 2f;
    private float currentSpread;
    private float currentSpreadVelocity;

    private float lastFireTime; // ���� ���������� �߻��� ����

    private LayerMask excludeTarget;

    private void Awake()
    {
        // ����� ������Ʈ���� ������ ��������
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();

        // ����� ���� �ΰ��� ����
        bulletLineRenderer.positionCount = 2;
        // ���� �������� ��Ȱ��ȭ
        bulletLineRenderer.enabled = false;
    }

    public void Setup(PlayerShooter gunHolder)
    {
        this.gunHolder = gunHolder;
        //excludeTarget = gunHolder.excludeTarget;
    }

    private void OnEnable()
    {
        currentSpread = 0;
        // ���� źâ�� ����ä���
        magAmmo = magCapacity;
        // ���� ���� ���¸� ���� �� �غ� �� ���·� ����
        state = State.Ready;
        // ���������� ���� �� ������ �ʱ�ȭ
        lastFireTime = 0;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool Fire(Vector3 aimTarget)
    {
        // ���� ���°� �߻� ������ ����
        // && ������ �� �߻� �������� timeBetFire �̻��� �ð��� ����
        if (state == State.Ready
            && Time.time >= lastFireTime + timeBetFire)
        {
            var xError = Utility.GetRandomNormalDistribution(0f, currentSpread);
            var yError = Utility.GetRandomNormalDistribution(0f, currentSpread);


            var fireDirection = aimTarget - fireTransform.position;

            fireDirection = Quaternion.AngleAxis(yError, Vector3.up) * fireDirection;
            fireDirection = Quaternion.AngleAxis(xError, Vector3.right) * fireDirection;

            currentSpread += 1f / stability;

            // ������ �� �߻� ������ ����
            lastFireTime = Time.time;
            // ���� �߻� ó�� ����
            Shot(fireTransform.position, fireDirection);

            return true;
        }

        return false;
    }

    // ���� �߻� ó��
    private void Shot(Vector3 startPoint, Vector3 direction)
    {
        // ����ĳ��Ʈ�� ���� �浹 ������ �����ϴ� �����̳�
        RaycastHit hit;
        // �Ѿ��� ���� ���� ������ ����
        var hitPosition = Vector3.zero;

        // ����ĳ��Ʈ(��������, ����, �浹 ���� �����̳�, �����Ÿ�)
        if (Physics.Raycast(startPoint, direction, out hit, fireDistance, ~excludeTarget))
        {
            // ���̰� � ��ü�� �浹�� ���

            // �浹�� �������κ��� IDamageable ������Ʈ�� �������� �õ�
            var target =
                hit.collider.GetComponent<IDamageable>();

            // �������� ���� IDamageable ������Ʈ�� �������µ� �����ߴٸ�
            if (target != null)
            {
/*                DamageMessage damageMessage;

                damageMessage.damager = gunHolder.gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hit.point;
                damageMessage.hitNormal = hit.normal;*/

                // ������ OnDamage �Լ��� ������Ѽ� ���濡�� ������ �ֱ�
                target.OnDamage(damage, hit.point, hit.normal);
            }
            else
            {
                //EffectManager.Instance.PlayHitEffect(hit.point, hit.normal, hit.transform);
            }
            // ���̰� �浹�� ��ġ ����
            hitPosition = hit.point;
        }
        else
        {
            // ���̰� �ٸ� ��ü�� �浹���� �ʾҴٸ�
            // �Ѿ��� �ִ� �����Ÿ����� ���ư������� ��ġ�� �浹 ��ġ�� ���
            hitPosition = startPoint + direction * fireDistance;
        }

        // �߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPosition));

        // ���� źȯ�� ���� -1
        magAmmo--;
        if (magAmmo <= 0)
            // źâ�� ���� ź���� ���ٸ�, ���� ���� ���¸� Empty���� ����
            state = State.Empty;
    }

    // �߻� ����Ʈ�� �Ҹ��� ����ϰ� �Ѿ� ������ �׸���
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        // �ѱ� ȭ�� ȿ�� ���
        muzzleFlashEffect.Play();
        // ź�� ���� ȿ�� ���
        shellEjectEffect.Play();

        // �Ѱ� �Ҹ� ���
        gunAudioPlayer.PlayOneShot(shotClip);

        // ���� �������� �ѱ��� ��ġ
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        // ���� ������ �Է����� ���� �浹 ��ġ
        bulletLineRenderer.SetPosition(1, hitPosition);
        // ���� �������� Ȱ��ȭ�Ͽ� �Ѿ� ������ �׸���
        bulletLineRenderer.enabled = true;

        // 0.03�� ���� ��� ó���� ���
        yield return new WaitForSeconds(0.03f);

        // ���� �������� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����
        bulletLineRenderer.enabled = false;
    }

    // ������ �õ�
    public bool Reload()
    {
        if (state == State.Reloading ||
            ammoRemain <= 0 || magAmmo >= magCapacity)
            // �̹� ������ ���̰ų�, ���� �Ѿ��� ���ų�
            // źâ�� �Ѿ��� �̹� ������ ��� ������ �Ҽ� ����
            return false;

        // ������ ó�� ����
        StartCoroutine(ReloadRoutine());
        return true;
    }

    // ���� ������ ó���� ����
    private IEnumerator ReloadRoutine()
    {
        // ���� ���¸� ������ �� ���·� ��ȯ
        state = State.Reloading;
        // ������ �Ҹ� ���
        gunAudioPlayer.PlayOneShot(reloadClip);

        // ������ �ҿ� �ð� ��ŭ ó���� ����
        yield return new WaitForSeconds(reloadTime);

        // źâ�� ä�� ź���� ����Ѵ�
        var ammoToFill = magCapacity - magAmmo;

        // źâ�� ä������ ź���� ���� ź�ຸ�� ���ٸ�,
        // ä������ ź�� ���� ���� ź�� ���� ���� ���δ�
        if (ammoRemain < ammoToFill) ammoToFill = ammoRemain;

        // źâ�� ä���
        magAmmo += ammoToFill;
        // ���� ź�࿡��, źâ�� ä�ŭ ź���� �A��
        ammoRemain -= ammoToFill;

        // ���� ���� ���¸� �߻� �غ�� ���·� ����
        state = State.Ready;
    }

    private void Update()
    {
        currentSpread = Mathf.SmoothDamp(currentSpread, 0f, ref currentSpreadVelocity, 1f / restoreFromRecoilSpeed);
        currentSpread = Mathf.Clamp(currentSpread, 0f, maxSpread);
    }
}