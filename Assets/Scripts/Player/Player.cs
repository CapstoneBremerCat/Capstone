using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Status
{
    [Header("Scripts")]
    [SerializeField] private PlayerShooter playerShooter;   // 총 발사 

    [Header("PlayerMove")]
    [SerializeField] private Transform cam; // 메인 카메라
    //[SerializeField] private Transform cameraArm; // 카메라 축
    [SerializeField] private Transform charBody; //  캐릭터 몸체
    [SerializeField] private CharacterController charController;    //  캐릭터 컨트롤러
    [SerializeField] private PlayerInput playerInput;       // 입력 감지

    [Range(0.01f, 1f)] public float airControlPercent;  // 체공 시 속도 보정
    [Range(0.01f, 2f)] public float gravityPercent;   // 중력 보정
    [SerializeField] private float speedSmoothTime = 0.1f;    // 스무스하게 이동하는 지연 시간
    [SerializeField] private float turnSmoothTime = 0.1f;     // 스무스하게 회전하는 지연 시간

    private float speedSmoothVelocity;  // 이동 보정 속도
    private float turnSmoothVelocity;   // 회전 보정 속도
    private float currentVelocityY;     // 중력에 의해서 바닥에 떨어지는 y방향 속도

    [Header("SFX")]
    [SerializeField] private AudioClip deathSound;  // 사망 효과음.
    [SerializeField] private AudioClip hitSound; // 피격 효과음.
    [SerializeField] private ParticleSystem hitEffect;  // 피격 이펙트.
    [SerializeField] private Animator anim; // 애니메이션
    private AudioSource audioSource;    // 효과음을 출력하는데 사용.
    private int animSpeed = 0;   // 애니메이션 속도

    private void Awake()
    {
        OnDeath += () =>
        {
            // 더 이상 피격 판정이 되지 않게 collider를 끈다.
            //if (collider) collider.enabled = false;
            if (anim) anim.SetBool("isDead", isDead);   // Zombie Death 애니메이션 실행.
            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);     // 사망 효과음 1회 재생.
            //GameMgr.instance.AddScore(100); // enemy 처치 시, 100 score 상승.
            //EnemyMgr.Instance.DecreaseSpawnCount(); // enemy 처치 시, Spawn Count 감소.
            GameManager.Instance.GameOver();
        };
    }

    public void Init()
    {
        InitStatus();
    }
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        StageUIController.Instance.SetHealthBar(GetHpRatio());
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

    public void SetPosition(Vector3 pos)
    {
        charController.enabled = false;
        transform.position = pos;
        charController.enabled = true;
    }

    public void UpdateAttack()
    {
        if (playerShooter) playerShooter.ShootUpdate();
    }
    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    public float currentSpeed =>
     new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

    public void UpdateMovement()
    {
        if (isDead) return;
        if (currentSpeed > 0.2f || playerInput.fire) Rotate();
        Run(playerInput.run);
        Move(playerInput.moveInput);

        if (playerInput.jump) Jump();
    }

    public void Move(Vector2 moveInput)
    {
        var targetSpeed = moveSpeed * moveInput.magnitude * animSpeed;
        var moveDirection = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

        // 이동 입력이 있을 경우 캐릭터 바디 방향을 실제 이동방향으로 변경
        if (moveInput.magnitude != 0) charBody.forward = moveDirection;  

        var smoothTime = charController.isGrounded ? speedSmoothTime : speedSmoothTime / airControlPercent;

        targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
        currentVelocityY += Time.deltaTime * Physics.gravity.y * gravityPercent;

        var velocity = moveDirection * targetSpeed + Vector3.up * currentVelocityY;

        charController.Move(velocity * Time.deltaTime);
        MoveAnim(moveDirection.x, moveDirection.z);

        if (charController.isGrounded) currentVelocityY = 0;
    }

    public void Run(bool isRun)
    {
        if (isRun)
        {
            animSpeed = (UseStamina(runStamina)) ? 2 : 1;
        }
        else
        {
            animSpeed = 1;
            RestoreStamina(regenStamina);
        }
        StageUIController.Instance.SetStaminaBar(GetStaminaRatio());
    }

    public void Rotate()
    {
        var targetRotation = cam.transform.eulerAngles.y;

        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                                    ref turnSmoothVelocity, turnSmoothTime);
    }

    public void Jump()
    {
        // 캐릭터가 땅에 붙어있는 경우에만 작동 
        if (!charController.isGrounded) return;

        currentVelocityY = jumpPower;
    }

    public void MoveAnim(float h, float v)
    {
        float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
        // 달리기 입력 시 값 두배로 증가
        if (anim) anim.SetFloat("Magnitude", value * animSpeed);
    }
}
