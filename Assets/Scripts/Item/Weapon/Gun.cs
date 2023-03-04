using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum State
{
    Ready,  // �߻� �غ��
    Empty,  // źâ�� ��
    Reloading   // ������ ��
}

public class Gun : MonoBehaviour
{
    private Transform pivot;
    private State state = State.Ready;  // ���� ���� ���� ����.

    [Header("Gun")] //inspector�� ǥ��, �ش� Data���� ���ó�� �˷��ش�.
    [SerializeField] private Bullet bullet; // �Ѿ�
    [SerializeField] private Transform firePos; // �Ѿ� �߻� ��ġ.
    [SerializeField] private Transform weaponOffset;

    [Header("Gun �Ӽ�")] // [Range(a,b)] : ���� ������ ����(a~b).
    [SerializeField] private string gunName;  // �� �̸�
    [SerializeField] private float hitRange = 100f;  // �����Ÿ�.
    [SerializeField] private float timeBetFire = 0.12f;    // �Ѿ� �߻� ����.
    private float lastFireTime; // ���� ���������� �߻��� ����.
    private LineRenderer bulletLineRenderer;    // �Ѿ� ������ �׸��� ���� ����.

    [SerializeField] private readonly int magCapacity = 50;  // źâ �뷮.
    [SerializeField] private int ammoRemain = 900; // �����ϰ� �ִ� �Ѿ��� ��.
    private int magAmmo;    // ���� źâ�� �����ִ� �Ѿ��� ��.

    [SerializeField] private float reloadTime = 0.9f;   // ������ �ҿ� �ð�.

    [SerializeField] private float damage = 25;   // ������ ���ݷ�

    [Header("SFX")]
    [SerializeField] private ParticleSystem muzzleFlashEffect;  // �ѱ��� ȭ�� ȿ��.
    [SerializeField] private ParticleSystem shellEjectEffect;  // ź�� ���� ȿ��.
    [SerializeField] private AudioClip shootSound;  // �� ���� ȿ����.
    [SerializeField] private AudioClip reloadSound; // źâ ������ ȿ����.
    private AudioSource audioSource;

    private Vector3 originPos;  // ���� ���� ��ġ
    [SerializeField] private float retroActionForce;  // �ݵ� ����. ���� �������� �ٸ�.

    public Transform FirePos { get { return firePos; } }
    public Vector3 Pivot { get { return (pivot) ? pivot.position : Vector3.zero; } set { if (pivot) pivot.position = value; } }
    public Transform WeaponOffset { get { return weaponOffset; } }
    public State GetState { get { return state; } } // Ŀ����.
    public int AmmoRemain { get { return ammoRemain; } }
    public int MagAmmo { get { return magAmmo; } }
    public float HitRange { get { return hitRange; } }
    public string GunName { get { return gunName; } }

    public bool IsFire { get; private set; }


    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        //pivot = transform.parent;

        bulletLineRenderer = GetComponent<LineRenderer>();
        if (bulletLineRenderer)
        {
            bulletLineRenderer.positionCount = 2; // ���� �׸��� ���� �� ���� ����.
            bulletLineRenderer.enabled = false;   // ���� ��� ������ ������ ������ �ʵ��� ��Ȱ��ȭ.
        }
    }

    private void OnEnable()
    {
        magAmmo = magCapacity;  // �ʱ� źâ�� �Ѿ��� �ִ�ġ��.
        state = State.Ready;
        lastFireTime = Time.time - timeBetFire;

    }

    public void SetOriginPos(Vector3 position)
    {
        // ù ��ġ ���
        originPos = position;
    }

    public void Fire()
    {
        if (state.Equals(State.Ready) && Time.time >= lastFireTime + timeBetFire)    // ���� ���� �� �� �ִ� �������� Ȯ��.
        {
            lastFireTime = Time.time;   // �߻� ���� ����.
            Shot(); //�߻� ó��
            IsFire = true;
        }
        IsFire = false;
    }

    private void Shot()
    {
        if (firePos)
        {
            RaycastHit hit; // Physics.RayCast()�� �̿��Ͽ� �浹 ���� ������ �˾ƿ´�.
            Vector3 hitPos = firePos.position + firePos.forward * hitRange; // �Ѿ��� ���� ��ġ�� ����, �ִ� �Ÿ� ��ġ�� �⺻ ������ ������.
            if (Physics.Raycast(firePos.position, firePos.forward, out hit, hitRange))
            {
                Debug.DrawRay(firePos.position, firePos.forward * hit.distance, Color.red);
                Debug.DrawRay(firePos.position, hitPos, Color.blue);
                // TODO : Enemy(Zombie) Damageable Code �߰�
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (null != target) target.OnDamage(damage, hit.point, hit.normal);
                hitPos = hit.point; // ���� �Ѿ��� ���� �������� ����.
            }
            // �ѱ� �ݵ� �ڷ�ƾ ����
            StopAllCoroutines();
            StartCoroutine(RetroActionCoroutine());
            StartCoroutine(ShotEffect(hitPos)); // �Ѿ� �߻� ����Ʈ.

            //Instantiate(bullet.gameObject, firePos.transform.position, firePos.transform.rotation);   // �Ѿ� �߻�

            magAmmo--;  // źâ�� �Ѿ��� ����.
            if (0 >= magAmmo) state = State.Empty;  // ���� �Ѿ� �� Ȯ��. �Ѿ��� ���ٸ�, ���� ���¸� Empty�� ����.
        }
    }

    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(originPos.x, originPos.y, -retroActionForce);     // ������ �� ���� ���� �ִ� �ݵ�
        //Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);  // ������ ���� ���� �ִ� �ݵ�

        //if (!isFineSightMode)  // �������� �ƴ� ����
        {
            transform.localPosition = originPos;

            // �ݵ� ����
            while (transform.localPosition.z >= -retroActionForce)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // ����ġ
            while (transform.localPosition != originPos)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        /*        else  // ������ ����
                {
                    transform.localPosition = fineSightOriginPos;

                    // �ݵ� ����
                    while (transform.localPosition.x <= retroActionFineSightForce - 0.02f)
                    {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, retroActionRecoilBack, 0.4f);
                        yield return null;
                    }

                    // ����ġ
                    while (transform.localPosition != fineSightOriginPos)
                    {
                        transform.localPosition = Vector3.Lerp(transform.localPosition, fineSightOriginPos, 0.1f);
                        yield return null;
                    }
                }*/
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        if (muzzleFlashEffect) muzzleFlashEffect.Play();
        if (shellEjectEffect) shellEjectEffect.Play();
        if (audioSource && shootSound) audioSource.PlayOneShot(shootSound);

        if (bulletLineRenderer)
        {
            bulletLineRenderer.SetPosition(0, firePos.position); // �Ѿ��� �߻� ��������,
            bulletLineRenderer.SetPosition(1, hitPosition); // �Ѿ��� ���� ��ġ���� ���� �׸���.
            bulletLineRenderer.enabled = true; // �׸��� �׸������� LineRenderer�� Ȱ��ȭ ��Ų��.
        }
        yield return new WaitForSeconds(0.03f); // 0.03�� ���� ��� ó���� ���.
        if (bulletLineRenderer) bulletLineRenderer.enabled = false; // LineRenderer�� ��Ȱ��ȭ�Ͽ� �Ѿ� ������ �����.
    }


    public bool Reload()
    {
        // ������ ���̰ų� ���� ź���� ���ų�,
        // ź���� �������� ���̻� �߰��� �� ���� ���� ������ �Ұ���.
        if (state.Equals(State.Reloading) || 0 >= ammoRemain || magCapacity <= magAmmo) return false;
        StartCoroutine(ReloadRoutine()); // ������ ���·� ����.
        return true;
    }

    private IEnumerator ReloadRoutine()
    {
        state = State.Reloading; // ���� ���¸� ������ �� ���·� ��ȯ.

        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime); // ������ �ҿ� �ð� ��ŭ ó���� ����.

        // źâ�� ä�� �Ѿ��� ���.
        // �����ϰ� �ִ� �Ѿ˰� źâ�� �� ���� ���� Ȯ���Ͽ� ���� ���� ���.
        int ammoTofill = Mathf.Min(magCapacity - magAmmo, ammoRemain);
        ammoRemain -= ammoTofill; // �����ϰ� �ִ� �Ѿ��� ���� ����.
        magAmmo += ammoTofill; // ������ �Ѿ��� ����ŭ źâ�� �Ѿ��� �߰�.

        state = State.Ready; // ���� ���� ���¸� �߻� �غ�� ���·� ����.
    }

    public void AddAmmo(int value)
    {
        ammoRemain += value;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
