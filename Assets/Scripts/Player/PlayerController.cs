using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private PlayerInput playerInput;
    //[SerializeField] private PlayerMove playerMove;
    [SerializeField] private LivingEntity livingEntity;

    [Header("PlayerMove")]
    [SerializeField] private Transform cam; // 메인 카메라
    [SerializeField] private Transform cameraArm; // 카메라 축
    [SerializeField] private Transform charBody; //  캐릭터 몸체
    [SerializeField] private CharacterController charController;    //  캐릭터 컨트롤러

    private float gravity;  // 중력
    private float yVelocity;  // z 이동값
    private Vector3 moveDir; // 캐릭터의 움직이는 방향

    [SerializeField] private Animator anim;
    private int animSpeed = 0;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        gravity = -9.81f;
        yVelocity = 0f;
        moveDir = Vector3.zero;
    }
    public void MoveAnim(float h, float v)
    {
        float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
        // 달리기 입력 시 값 두배로 증가
        if (anim && playerInput) anim.SetFloat("Magnitude", value * animSpeed);
        //if (anim && playerInput) anim.SetFloat("Magnitude", h * animSpeed);
/*        if (anim && playerInput) anim.SetFloat("Vertical", v * animSpeed);
        if (anim && playerInput) anim.SetFloat("Horizontal", h * animSpeed);*/
    }

    /*
    public void Run(bool isRun)
    {
        animSpeed = isRun ? 2 : 1;
        if (isRun)
        {
            livingEntity.UseStamina(playerStatus.runStamina);
        }
        else
        {
            livingEntity.RestoreStamina(playerStatus.regenStamina);
        }
        //UIMgr.Instance.SetStaminaBar(playerStatus.GetStaminaRatio());
    }
    */
    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        //추가
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void Update()
    {
        if (charController == null) return;
        //LookAround();
        Move();
/*
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            var offset = cam.transform.forward;
            offset.y = 0;
            transform.LookAt(charController.transform.position + offset);
        }*/
        //runStamina(playerInput.run);
    }

    private void Move()
    {

        // 캐릭터가 땅에 붙어있는 경우에만 작동
        if (playerInput.jump && charController.isGrounded)
        {
            yVelocity = playerStatus.jumpPower;
        }


        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        //anim.SetBool("isMove", isMove);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;

            moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            charBody.forward = moveDir;

            // y속도를 최종 dir의 y에 대입.
            moveDir.y = yVelocity;

            // run 키 입력 시 animSpeed를 2로 변경
            animSpeed = playerInput.run ? 2 : 1;
            //transform.position += moveDir * playerStatus.speed * animSpeed * Time.deltaTime * 5f;
            charController.Move(moveDir * playerStatus.speed * animSpeed * Time.deltaTime);
        }
        MoveAnim(moveDir.x, moveDir.z);

        // y속도를 지속적으로 갱신.
        yVelocity += gravity * Time.deltaTime;
    }

    /*
    private void FixedUpdate()
    {
        if (!charController) return;

        yVelocity += gravity * Time.deltaTime;
        // 캐릭터가 땅에 붙어있는 경우에만 작동
        if (playerInput.jump && charController.isGrounded)
        {
            yVelocity = playerStatus.jumpPower;
        }

        // 키보드 입력에 따른 이동량 측정
        moveDir = transform.forward * playerInput.vertical +
                 transform.right * playerInput.horizontal;

        //대각선 이동으로 하면서 루트2로 길이가 늘어나기에 1로 만들어준다.
        moveDir.Normalize();

        // y속도를 최종 dir의 y에 대입.
        moveDir.y = yVelocity;

        // 실제 캐릭터의 이동
        charController.Move(moveDir * playerStatus.speed * animSpeed * Time.deltaTime);
        Run(playerInput.run);
        MoveAnim(moveDir.x, moveDir.z);

    }*/

    /* public GameObject Cam; // 제어할 캐릭터 컨트롤러
     public CharacterController SelectPlayer; // 제어할 캐릭터 컨트롤러
     public float Speed;  // 이동속도
     public float JumpPow;

     private float Gravity; // 중력   
     private Vector3 MoveDir; // 캐릭터의 움직이는 방향.
     private bool JumpButtonPressed;  //  최종 점프 버튼 눌림 상태
     private bool FlyingMode;  // 행글라이더 모드여부

     // Start is called before the first frame update
     void Start()
     {
         // 기본값
         Speed = 5.0f;
         Gravity = 10.0f;
         MoveDir = Vector3.zero;
         JumpPow = 5.0f;
         JumpButtonPressed = false;
         FlyingMode = false;
     }

     // Update is called once per frame
     void Update()
     {
         if (SelectPlayer == null) return;

         if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
         {
             var offset = Cam.transform.forward;
             offset.y = 0;
             transform.LookAt(SelectPlayer.transform.position + offset);
         }
         // 캐릭터가 바닥에 붙어 있는 경우만 작동합니다.
         // 캐릭터가 바닥에 붙어 있지 않다면 바닥으로 추락하고 있는 중이므로
         // 바닥 추락 도중에는 방향 전환을 할 수 없기 때문입니다.
         if (SelectPlayer.isGrounded)
         {
             // 키보드에 따른 X, Z 축 이동방향을 새로 결정합니다.
             MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
             // 오브젝트가 바라보는 앞방향으로 이동방향을 돌려서 조정합니다.
             MoveDir = SelectPlayer.transform.TransformDirection(MoveDir);
             // 속도를 곱해서 적용합니다.
             MoveDir *= Speed;

             // 스페이스 버튼에 따른 점프 : 최종 점프버튼이 눌려있지 않았던 경우만 작동
             if (JumpButtonPressed == false && Input.GetButton("Jump"))
             {
                 SelectPlayer.transform.rotation = Quaternion.Euler(0, 45, 0);
                 JumpButtonPressed = true;
                 MoveDir.y = JumpPow;
             }
         }
         // 캐릭터가 바닥에 붙어 있지 않다면
         else
         {
             // 하강중에 스페이스 버튼을 누르면 슬로우 낙하모드 발동!
             if (MoveDir.y < 0 && JumpButtonPressed == false && Input.GetButton("Jump"))
             {
                 FlyingMode = true;
             }

             if (FlyingMode)
             {
                 JumpButtonPressed = true;

                 // 중력 수치를 감속합니다.
                 MoveDir.y *= 0.95f;

                 // 하지만 하늘에서 정지해 있는 일은 벌어지지 않게 하기 위해
                 // 최소 초당 -1의 하강 속도는 유지합니다.
                 if (MoveDir.y > -1) MoveDir.y = -1;

                 // 또한 이 때는 방향전환이 가능합니다.
                 MoveDir.x = Input.GetAxis("Horizontal");
                 MoveDir.z = Input.GetAxis("Vertical");
             }
             else
                 // 중력의 영향을 받아 아래쪽으로 하강합니다.           
                 MoveDir.y -= Gravity * Time.deltaTime;
         }

         // 점프버튼이 눌려지지 않은 경우
         if (!Input.GetButton("Jump"))
         {
             JumpButtonPressed = false;  // 최종점프 버튼 눌림 상태 해제
             FlyingMode = false;         // 행글라이더 모드 해제
         }
         // 앞 단계까지는 캐릭터가 이동할 방향만 결정하였으며,
         // 실제 캐릭터의 이동은 여기서 담당합니다.
         SelectPlayer.Move(MoveDir * Time.deltaTime);
     }*/
}
