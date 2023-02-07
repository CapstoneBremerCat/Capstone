using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private PlayerStatus playerStatus; // �÷��̾� �������ͽ�
    [SerializeField] private PlayerInput playerInput;   // �Է� ����
    //[SerializeField] private PlayerMove playerMove;
    [SerializeField] private LivingEntity livingEntity; 

    [Header("PlayerMove")]
    [SerializeField] private Transform cam; // ���� ī�޶�
    [SerializeField] private Transform cameraArm; // ī�޶� ��
    [SerializeField] private Transform charBody; //  ĳ���� ��ü
    [SerializeField] private CharacterController charController;    //  ĳ���� ��Ʈ�ѷ�

    [Range(0.01f, 1f)] public float airControlPercent;  // ü�� �� �ӵ� ����
    [Range(0.01f, 2f)] public float gravityPercent;   // �߷� ����
    [SerializeField] private float speedSmoothTime = 0.1f;    // �������ϰ� �̵��ϴ� ���� �ð�
    [SerializeField] private float turnSmoothTime = 0.1f;     // �������ϰ� ȸ���ϴ� ���� �ð�

    private float speedSmoothVelocity;  // �̵� ���� �ӵ�
    private float turnSmoothVelocity;   // ȸ�� ���� �ӵ�
    private float currentVelocityY;     // �߷¿� ���ؼ� �ٴڿ� �������� y���� �ӵ�

    [SerializeField] private Animator anim; // �ִϸ��̼�
    private int animSpeed = 0;   // �ִϸ��̼� �ӵ�


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

    // ���� ���� �ֱ⿡ ���� ȸ��, �̵� ����.  
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
        // run Ű �Է� �� animSpeed�� 2�� ����
        animSpeed = playerInput.run ? 2 : 1;
        var targetSpeed = playerStatus.speed * moveInput.magnitude * animSpeed;
        var moveDirection = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

        // �̵� �Է��� ���� ��� ĳ���� �ٵ� ������ ���� �̵��������� ����
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
        // ĳ���Ͱ� ���� �پ��ִ� ��쿡�� �۵� 
        if (!charController.isGrounded) return;

        currentVelocityY = playerStatus.jumpPower;
    }

    public void MoveAnim(float h, float v)
    {
        float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
        // �޸��� �Է� �� �� �ι�� ����
        if (anim && playerInput) anim.SetFloat("Magnitude", value * animSpeed);
    }
}
