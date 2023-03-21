using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Player : Status
    {
        [Header("Scripts")]
        [SerializeField] private PlayerShooter playerShooter;

        [Header("PlayerMove")]
        [SerializeField] private Transform cam; // Camera object
                                                //[SerializeField] private Transform cameraArm; // Camera arm
        [SerializeField] private Transform charBody; // Character body object
        [SerializeField] private CharacterController charController;    // Character controller
        [SerializeField] private PlayerInput playerInput;       // Input class

        [Range(0.01f, 1f)] public float airControlPercent;  // Movement ratio in the air
        [Range(0.01f, 2f)] public float gravityPercent;   // Gravity ratio
        [SerializeField] private float speedSmoothTime = 0.1f;    // Time to smoothly change speed
        [SerializeField] private float turnSmoothTime = 0.1f;     // Time to smoothly change rotation

        private float speedSmoothVelocity;  // Current value of movement speed
        private float turnSmoothVelocity;   // Current value of rotation speed
        private float currentVelocityY;     // Current y-axis movement speed constantly updated by gravity
        public float currentSpeed =>
         new Vector2(charController.velocity.x, charController.velocity.z).magnitude;

        [Header("SFX")]
        [SerializeField] private AudioClip deathSound;  // Sound effect for death
        [SerializeField] private AudioClip hitSound; // Sound effect for getting hit
        [SerializeField] private ParticleSystem hitEffect;  // Particle effect for getting hit
        [SerializeField] private Animator anim; // Animation
        private AudioSource audioSource;    // Audio source for playing sounds
        private int animSpeed = 0;   // Animation speed

        private List<PassiveSkill> equippedPassiveSkills = new List<PassiveSkill>();
        public ActiveSkill equippedActiveSkill { get; private set; }
        private bool isCoolTime;
        private void Awake()
        {
            OnDeath += () =>
            {
                // If dead, disable collider
                //if (collider) collider.enabled = false;
                if (anim) anim.SetBool("isDead", isDead);   // Set Death animation.
                if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);
                GameManager.Instance.GameOver();
            };
        }
        public int GetEquippedPassiveSkillCount()
        {
            return equippedPassiveSkills.Count;
        }
        public void EquipPassiveSkill(PassiveSkill passive)
        {
            equippedPassiveSkills.Add(passive);
            ApplyStatus(passive.statusData);
            // Notify the mediator that equipment was equipped
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_PASSIVE, this);

            //playerStatusWindow.RefreshStatusUI(this);
        }
        public void UnequipPassiveSkill(PassiveSkill passive)
        {
            equippedPassiveSkills.Remove(passive);
            RemoveStatus(passive.statusData);
            // Notify the mediator that equipment was equipped
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_PASSIVE, this);
        }
        public void EquipActiveSkill(ActiveSkill active)
        {
            equippedActiveSkill = active;
        }
        public void UnequipActiveSkill()
        {
            equippedActiveSkill = null;
        }


        public void Init(Vector3 initPos)
        {
            InitStatus();
            SetPlayerPosition(initPos);
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_PASSIVE, this);

            //playerStatusWindow.RefreshStatusUI(this);
        }

        public void SetPlayerPosition(Vector3 pos)
        {
            charController.enabled = false;
            this.transform.position = pos;
            charController.enabled = true;
        }

        private void OnDestroy()
        {

        }
        public void OnInputUpdated()
        {
            // 입력값에 따라 적절한 처리를 수행합니다.
            if (playerShooter && playerInput.fire) playerShooter.ShootUpdate(playerInput.fire, playerInput.reload);
            if (equippedActiveSkill && playerInput.skillSlot1) UseSkill(equippedActiveSkill);
            if (playerInput.inventory) UIManager.Instance.ToggleInventoryUI();
            if (playerInput.skillWindow) UIManager.Instance.ToggleSkillWindowUI();
            if (playerInput.equipWindow) UIManager.Instance.ToggleEquipWindowUI();
            if (playerInput.statusWindow) UIManager.Instance.ToggleStatusUI();
        }

        // 물리 갱신 주기에 맞춰 회전, 이동 실행.  
        private void FixedUpdate()
        {
            // 게임오버되지 않았을 경우에만 실행
            if (isDead) return;
            UpdateMovement();
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
                    hitEffectTR.position = hitPoint;    // Move the effect to the hit point.
                    // Orient the effect towards the hit normal.
                    hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                    hitEffect.Play();   // Play the effect.
                }

                // Play hit sound.
                if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
                anim.SetTrigger("Damaged"); // Trigger the damaged animation.
            }
        }

        public void SetPosition(Vector3 pos)
        {
            charController.enabled = false;
            transform.position = pos;
            charController.enabled = true;
        }

        public void UpdateMovement()
        {
            if (currentSpeed > 0.2f || playerInput.fire) Rotate();
            Run(playerInput.run);
            Move(playerInput.moveInput);

            if (playerInput.jump) Jump();
        }

        public void Move(Vector2 moveInput)
        {
            var targetSpeed = moveSpeed * moveInput.magnitude * animSpeed;
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
            // ĳ���Ͱ� ���� �پ��ִ� ��쿡�� �۵� 
            if (!charController.isGrounded) return;

            currentVelocityY = jumpPower;
        }

        private void UseSkill(ActiveSkill skill)
        {
            // 쿨타임 중에는 스킬을 사용할 수 없습니다.
            if (isCoolTime) return;

            Mediator.Instance.Notify(this, GameEvent.SKILL_ACTIVATED, skill);
            // 쿨타임을 시작합니다.
            StartCoroutine(CooltimeRoutine(skill.cooldown));
        }
        private IEnumerator CooltimeRoutine(float timeRemaining)
        {
            isCoolTime = true;
            float totalTime = timeRemaining;
            float interval = 0.1f;
            while (timeRemaining > 0)
            {
                timeRemaining -= interval;
                yield return new WaitForSeconds(interval);
            }
            isCoolTime = false;
        }

        public void MoveAnim(float h, float v)
        {
            float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
            // �޸��� �Է� �� �� �ι�� ����
            if (anim) anim.SetFloat("Magnitude", value * animSpeed);
        }
    }
}