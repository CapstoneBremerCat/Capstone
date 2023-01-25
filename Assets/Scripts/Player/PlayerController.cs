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
    [SerializeField] private Transform cam; // ���� ī�޶�
    [SerializeField] private Transform cameraArm; // ī�޶� ��
    [SerializeField] private Transform charBody; //  ĳ���� ��ü
    [SerializeField] private CharacterController charController;    //  ĳ���� ��Ʈ�ѷ�

    private float gravity;  // �߷�
    private float yVelocity;  // z �̵���
    private Vector3 moveDir; // ĳ������ �����̴� ����

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
        // �޸��� �Է� �� �� �ι�� ����
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

        //�߰�
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

        // ĳ���Ͱ� ���� �پ��ִ� ��쿡�� �۵�
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

            // y�ӵ��� ���� dir�� y�� ����.
            moveDir.y = yVelocity;

            // run Ű �Է� �� animSpeed�� 2�� ����
            animSpeed = playerInput.run ? 2 : 1;
            //transform.position += moveDir * playerStatus.speed * animSpeed * Time.deltaTime * 5f;
            charController.Move(moveDir * playerStatus.speed * animSpeed * Time.deltaTime);
        }
        MoveAnim(moveDir.x, moveDir.z);

        // y�ӵ��� ���������� ����.
        yVelocity += gravity * Time.deltaTime;
    }

    /*
    private void FixedUpdate()
    {
        if (!charController) return;

        yVelocity += gravity * Time.deltaTime;
        // ĳ���Ͱ� ���� �پ��ִ� ��쿡�� �۵�
        if (playerInput.jump && charController.isGrounded)
        {
            yVelocity = playerStatus.jumpPower;
        }

        // Ű���� �Է¿� ���� �̵��� ����
        moveDir = transform.forward * playerInput.vertical +
                 transform.right * playerInput.horizontal;

        //�밢�� �̵����� �ϸ鼭 ��Ʈ2�� ���̰� �þ�⿡ 1�� ������ش�.
        moveDir.Normalize();

        // y�ӵ��� ���� dir�� y�� ����.
        moveDir.y = yVelocity;

        // ���� ĳ������ �̵�
        charController.Move(moveDir * playerStatus.speed * animSpeed * Time.deltaTime);
        Run(playerInput.run);
        MoveAnim(moveDir.x, moveDir.z);

    }*/

    /* public GameObject Cam; // ������ ĳ���� ��Ʈ�ѷ�
     public CharacterController SelectPlayer; // ������ ĳ���� ��Ʈ�ѷ�
     public float Speed;  // �̵��ӵ�
     public float JumpPow;

     private float Gravity; // �߷�   
     private Vector3 MoveDir; // ĳ������ �����̴� ����.
     private bool JumpButtonPressed;  //  ���� ���� ��ư ���� ����
     private bool FlyingMode;  // ��۶��̴� ��忩��

     // Start is called before the first frame update
     void Start()
     {
         // �⺻��
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
         // ĳ���Ͱ� �ٴڿ� �پ� �ִ� ��츸 �۵��մϴ�.
         // ĳ���Ͱ� �ٴڿ� �پ� ���� �ʴٸ� �ٴ����� �߶��ϰ� �ִ� ���̹Ƿ�
         // �ٴ� �߶� ���߿��� ���� ��ȯ�� �� �� ���� �����Դϴ�.
         if (SelectPlayer.isGrounded)
         {
             // Ű���忡 ���� X, Z �� �̵������� ���� �����մϴ�.
             MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
             // ������Ʈ�� �ٶ󺸴� �չ������� �̵������� ������ �����մϴ�.
             MoveDir = SelectPlayer.transform.TransformDirection(MoveDir);
             // �ӵ��� ���ؼ� �����մϴ�.
             MoveDir *= Speed;

             // �����̽� ��ư�� ���� ���� : ���� ������ư�� �������� �ʾҴ� ��츸 �۵�
             if (JumpButtonPressed == false && Input.GetButton("Jump"))
             {
                 SelectPlayer.transform.rotation = Quaternion.Euler(0, 45, 0);
                 JumpButtonPressed = true;
                 MoveDir.y = JumpPow;
             }
         }
         // ĳ���Ͱ� �ٴڿ� �پ� ���� �ʴٸ�
         else
         {
             // �ϰ��߿� �����̽� ��ư�� ������ ���ο� ���ϸ�� �ߵ�!
             if (MoveDir.y < 0 && JumpButtonPressed == false && Input.GetButton("Jump"))
             {
                 FlyingMode = true;
             }

             if (FlyingMode)
             {
                 JumpButtonPressed = true;

                 // �߷� ��ġ�� �����մϴ�.
                 MoveDir.y *= 0.95f;

                 // ������ �ϴÿ��� ������ �ִ� ���� �������� �ʰ� �ϱ� ����
                 // �ּ� �ʴ� -1�� �ϰ� �ӵ��� �����մϴ�.
                 if (MoveDir.y > -1) MoveDir.y = -1;

                 // ���� �� ���� ������ȯ�� �����մϴ�.
                 MoveDir.x = Input.GetAxis("Horizontal");
                 MoveDir.z = Input.GetAxis("Vertical");
             }
             else
                 // �߷��� ������ �޾� �Ʒ������� �ϰ��մϴ�.           
                 MoveDir.y -= Gravity * Time.deltaTime;
         }

         // ������ư�� �������� ���� ���
         if (!Input.GetButton("Jump"))
         {
             JumpButtonPressed = false;  // �������� ��ư ���� ���� ����
             FlyingMode = false;         // ��۶��̴� ��� ����
         }
         // �� �ܰ������ ĳ���Ͱ� �̵��� ���⸸ �����Ͽ�����,
         // ���� ĳ������ �̵��� ���⼭ ����մϴ�.
         SelectPlayer.Move(MoveDir * Time.deltaTime);
     }*/
}
