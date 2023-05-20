using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using BlockChain;
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
        [SerializeField] private AudioClip idleSound; // Sound effect for getting hit
        [SerializeField] private ParticleSystem hitEffect;  // Particle effect for getting hit
        [SerializeField] private GameObject healEffect;  // Particle effect for getting hit
        [SerializeField] private Animator anim; // Animation
        [SerializeField] private AudioSource audioSource;    // Audio source for playing sounds
        private int animSpeed = 0;   // Animation speed

        [Header("Game")]
        [SerializeField] private Transform weaponSocket;
        public List<PassiveSkill> equippedPassiveSkills { get; private set; }
        public ActiveSkill equippedActiveSkill { get; private set; }
        private bool isCoolTime;
        private bool isRun;
        public bool inputState { get; private set; }
        private Inventory inventory;
        private void Awake()
        {
            OnDeath += () =>
            {
                // If dead, disable collider
                //if (collider) collider.enabled = false;
                //if (anim) anim.SetBool("isDead", isDead);   // Set Death animation.
                if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);
                SetInputState(false);
                GameManager.Instance.GameOver();
            };
            UIManager.Instance.RestartEvent += () =>
            {
                UIManager.Instance.UpdateHealthBar(GetHpRatio());
                UIManager.Instance.UpdateStaminaBar(GetStaminaRatio());
            };
            EquipManager.Instance.SetWeaponSocket(weaponSocket);
        }
        private void Start()
        {
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_WEAPON, InitWeaponStatus);
            inventory = GameObject.FindWithTag("Inventory")?.GetComponent<Inventory>();
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
            SoundManager.Instance.OnPlaySFX("Equip");
            //playerStatusWindow.RefreshStatusUI(this);
        }
        public void UnequipPassiveSkill(PassiveSkill passive)
        {
            equippedPassiveSkills.Remove(passive);
            RemoveStatus(passive.statusData);
            // Notify the mediator that equipment was equipped
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_PASSIVE, this);
            SoundManager.Instance.OnPlaySFX("Equip");
        }
        public void EquipActiveSkill(ActiveSkill active)
        {
            equippedActiveSkill = Instantiate(active, transform);
            SoundManager.Instance.OnPlaySFX("Equip");
        }
        public void UnequipActiveSkill()
        {
            if(equippedActiveSkill) Destroy(equippedActiveSkill);
            equippedActiveSkill = null;
            SoundManager.Instance.OnPlaySFX("Equip");
        }

        public void Init(Vector3 initPos)
        {
            if(equippedPassiveSkills == null) equippedPassiveSkills = new List<PassiveSkill>();
            InitStatus();
            SetPlayerPosition(initPos);
            SetInputState(true);
            //Mediator.Instance.Notify(this, GameEvent.REFRESH_STATUS, this);
            UnequipActiveSkill();
            equippedPassiveSkills.Clear();
            StartCoroutine(SoundRoutine(5));
        }

        public void SetPlayerPosition(Vector3 pos)
        {
            charController.enabled = false;
            this.transform.position = pos;
            charController.enabled = true;
        }

        public Vector3 GetCharacterPosition()
        {
            return charBody.position;
        }

        private void InitWeaponStatus(object weaponItem)
        {
            // If there is no currently equipped weapon, sets weapon to null.
            var weapon = EquipManager.Instance.EquippedWeapon;
            if (weapon)
            {
                weapon.Init(weaponSocket);
                SetWeaponDamage(weapon.Damage);
                weapon.UpdateWeaponStats(this);
            }
            else SetWeaponDamage(0);
            Mediator.Instance.Notify(this, GameEvent.REFRESH_STATUS, this);
        }

        private void OnDestroy()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_WEAPON, InitWeaponStatus);
            DataManager.Instance.SaveEquipmentsById(GetEquipmentIds());
        }

        private List<int> GetEquipmentIds()
        {
            List<int> equippedEquipmentIds = new List<int>();
            if(equippedActiveSkill) equippedEquipmentIds.Add(equippedActiveSkill.skillInfo.skillId);
            foreach (Skill skill in equippedPassiveSkills)
            {
                equippedEquipmentIds.Add(skill.skillInfo.skillId);
            }
            return equippedEquipmentIds;
        }

        public void UseHealKit()
        {
            // if inventory does not have healkit or Hp is full, return.
            if (!inventory.TryUseHealKit() || GetHpRatio() == 1) return;
            RestoreHealth(100);
            UIManager.Instance.UpdateHealthBar(GetHpRatio());
            SoundManager.Instance.OnPlaySFX("Item_Heal");
            var hitInstance = Instantiate(healEffect, transform.position, Quaternion.identity);

            //Destroy hit effects depending on particle Duration time
            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                Destroy(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitInstance, hitPsParts.main.duration);
            }
        }

/*        public void LoadSavedEquipments()
        {
            List<int> equippedEquipmentIds = DataManager.Instance.LoadEquipmentsById();
            foreach (int id in equippedEquipmentIds)
            {
                Skill skill = NFTManager.Instance.GetNFTSkillByID(id);
                switch (skill.skillType)
                {
                    case SkillType.Active:
                        equippedActiveSkill = skill as ActiveSkill;
                        break;
                    case SkillType.Passive:
                        equippedPassiveSkills.Add(skill as PassiveSkill);
                        break;
                    default:
                        break;
                }
            }
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_SKILL, this);
        }*/

        public void SetInputState(bool value)
        {
            inputState = value;
            Rotate();
            MoveAnim(0, 0);
        }

        public void OnInputUpdated()
        {
            if (playerInput.inventory) UIManager.Instance.ToggleInventoryUI();
            if (playerInput.skillWindow) UIManager.Instance.ToggleSkillWindowUI();
            if (playerInput.equipWindow) UIManager.Instance.ToggleEquipWindowUI();
            if (playerInput.statusWindow)
            {
                Mediator.Instance.Notify(this, GameEvent.REFRESH_STATUS, this);
                UIManager.Instance.ToggleStatusUI();
            }
            // if inputState is false, return.
            if (!inputState) return;
            // 입력값에 따라 적절한 처리를 수행합니다.
            if (playerShooter && playerInput.fire && !UIManager.Instance.IsWindowOpen())
            {
                playerShooter.ShootUpdate(playerInput.fire);
            }
            if (equippedActiveSkill && playerInput.skillSlot1) UseSkill(equippedActiveSkill);

            if (playerInput.useHealKit) UseHealKit();
        }

        // 물리 갱신 주기에 맞춰 회전, 이동 실행.  
        private void FixedUpdate()
        {
            // 게임오버되지 않았거나, 입력을 받을 수 있는 경우에만 실행
            if (isDead || !inputState) return;
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
                if (audioSource && hitSound)
                {
                    audioSource.clip = hitSound;
                    audioSource.Play();
                }
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
            isRun = playerInput.run;
            Run();
            Move(playerInput.moveInput);

            if (playerInput.jump) Jump();
        }

        public void Move(Vector2 moveInput)
        {
            var targetSpeed = moveSpeed * moveInput.magnitude * animSpeed;
            var moveDirection = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

            // �̵� �Է��� ���� ��� ĳ���� �ٵ� ������ ���� �̵��������� ����
            if (!playerInput.fire && moveInput.magnitude != 0) charBody.forward = moveDirection;

            var smoothTime = charController.isGrounded ? speedSmoothTime : speedSmoothTime / airControlPercent;

            targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
            currentVelocityY += Time.deltaTime * Physics.gravity.y * gravityPercent;

            var velocity = moveDirection * targetSpeed + Vector3.up * currentVelocityY;

            charController.Move(velocity * Time.deltaTime);
            MoveAnim(moveDirection.x, moveDirection.z);

            if (charController.isGrounded) currentVelocityY = 0;
        }

        public void Run()
        {
            // If the character is running,
            // set animSpeed to 2 if UseStamina returns true (stamina is consumed), otherwise set it to 1.
            // If the character is not running, set animSpeed to 1.
            animSpeed = isRun ? ((UseStamina(runStamina)) ? 2 : 1) : 1;
            StageUIController.Instance.SetStaminaBar(GetStaminaRatio());
        }

        public override void RestoreStamina(float value)
        {
            // Don't restore Stamina if the character is running.
            if (!isRun) base.RestoreStamina(value);
        }

        public void Rotate()
        {
            var targetRotation = cam.transform.eulerAngles.y;

            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                                        ref turnSmoothVelocity, turnSmoothTime);
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0; // y 축 값 제거
            charBody.forward = camForward;
        }

        public void Jump()
        {
            // If the character is not on the ground, return without doing anything.
            if (!charController.isGrounded) return;

            // Set the vertical velocity to the jump power to make the character jump.
            currentVelocityY = jumpPower;
        }

        private void UseSkill(ActiveSkill skill)
        {
            // 쿨타임 중에는 스킬을 사용할 수 없습니다.
            if (isCoolTime) return;
            equippedActiveSkill.UseSkill();
            float actualCoolTime = skill.cooldown * ((100.0f - coolTimeReduce) * 0.01f);
            // 쿨타임을 시작합니다.
            StartCoroutine(CooltimeRoutine(actualCoolTime));
            Mediator.Instance.Notify(this, GameEvent.SKILL_ACTIVATED, actualCoolTime);
        }
        private IEnumerator CooltimeRoutine(float timeRemaining)
        {
            isCoolTime = true;
            float interval = 0.1f;
            while (timeRemaining > 0)
            {
                timeRemaining -= interval;
                yield return new WaitForSeconds(interval);
            }
            isCoolTime = false;
        }
        private IEnumerator SoundRoutine(float interval)
        {
            while (true)
            {
                var randomInterval = interval + Random.Range(1, 20);
                yield return new WaitForSeconds(randomInterval);
                if (audioSource && idleSound)
                {
                    audioSource.clip = idleSound;
                    audioSource.Play();
                }
            }
        }

        public void MoveAnim(float h, float v)
        {
            float value = Mathf.Abs(v) > Mathf.Abs(h) ? v : h;
            if (!inputState) value = 0.0f;
            // �޸��� �Է� �� �� �ι�� ����
            if (anim) anim.SetFloat("Magnitude", value * animSpeed);
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}