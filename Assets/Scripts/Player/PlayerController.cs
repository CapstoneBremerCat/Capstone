using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStatus playerStatus; // 플레이어 스테이터스
    [SerializeField] private PlayerInput playerInput;   // 입력 감지
    //[SerializeField] private PlayerMove playerMove;
    [SerializeField] private LivingEntity livingEntity; 

    [Header("PlayerMove")]
    [SerializeField] private Transform cam; // 메인 카메라
    [SerializeField] private Transform cameraArm; // 카메라 축
    [SerializeField] private Transform charBody; //  캐릭터 몸체
    [SerializeField] private CharacterController charController;    //  캐릭터 컨트롤러

    [Range(0.01f, 1f)] public float airControlPercent;  // 체공 시 속도 보정
    [Range(0.01f, 2f)] public float gravityPercent;   // 중력 보정
    [SerializeField] private float speedSmoothTime = 0.1f;    // 스무스하게 이동하는 지연 시간
    [SerializeField] private float turnSmoothTime = 0.1f;     // 스무스하게 회전하는 지연 시간

    private float speedSmoothVelocity;  // 이동 보정 속도
    private float turnSmoothVelocity;   // 회전 보정 속도
    private float currentVelocityY;     // 중력에 의해서 바닥에 떨어지는 y방향 속도

    [SerializeField] private Animator anim; // 애니메이션
    private int animSpeed = 0;   // 애니메이션 속도


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void SetPosition(Vector3 pos)
    {
        charController.enabled = false;
        transform.position = pos;
        charController.enabled = true;
    }

    // 물리 갱신 주기에 맞춰 회전, 이동 실행.  
    private void FixedUpdate()
    {
        if (livingEntity.IsDead || charController == null) return;
        if (currentSpeed > 0.2f || playerInput.fire) Rotate();

        Move(playerInput.moveInput);

        if (playerInput.jump) Jump();
    }



    public float currentSpeed =>
     new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

    public void Move(Vector2 moveInput)
    {
        // run 키 입력 시 animSpeed를 2로 변경
        animSpeed = playerInput.run ? 2 : 1;
        var targetSpeed = playerStatus.speed * moveInput.magnitude * animSpeed;
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

        currentVelocityY = playerStatus.jumpPower;
    }

    public void MoveAnim(float h, float v)
    {
        float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
        // 달리기 입력 시 값 두배로 증가
        if (anim && playerInput) anim.SetFloat("Magnitude", value * animSpeed);
    }
}
