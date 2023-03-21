using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private string verticalAxisName = "Vertical";
        [SerializeField] private string horizontalAxisName = "Horizontal";
        [SerializeField] private string runButtonName = "Run";
        [SerializeField] private string jumpButtonName = "Jump";
        [SerializeField] private string fireBtnName = "Fire1";
        [SerializeField] private string reloadBtnName = "Reload";
        [SerializeField] private string SkillSlot1ButtonName = "SkillSlot 1";
        [SerializeField] private string InventoryButtonName = "Inventory";
        [SerializeField] private string SkillWindowButtonName = "SkillWindow";
        [SerializeField] private string EquipWindowButtonName = "EquipWindow";
        [SerializeField] private string StatusWindowButtonName = "StatusWindow";

        public Vector2 moveInput { get; private set; }  // Horizontal, Vertical
        public bool run { get; private set; }
        public bool jump { get; private set; }
        public bool fire { get; private set; }
        public bool reload { get; private set; }
        public bool skillSlot1 { get; private set; }
        public bool inventory { get; private set; }
        public bool skillWindow { get; private set; }
        public bool equipWindow { get; private set; }
        public bool statusWindow { get; private set; }

        //public event System.Action<float, float> MoveHandler;

        // Update is called once per frame
        void Update()
        {
            // 게임오버 상태에서는 사용자 입력을 감지하지 않는다
            if (GameManager.Instance != null
                && GameManager.Instance.isGameOver)
            {
                moveInput = Vector2.zero;
                fire = false;
                reload = false;
                jump = false;
                skillSlot1 = false;
                inventory = false;
                skillWindow = false;
                equipWindow = false;
                statusWindow = false;
                return;
            }
            moveInput = new Vector2(Input.GetAxis(horizontalAxisName), Input.GetAxis(verticalAxisName));

            if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;

            run = Input.GetButton(runButtonName);
            jump = Input.GetButtonDown(jumpButtonName);
            fire = Input.GetButton(fireBtnName);
            reload = Input.GetButtonDown(reloadBtnName);
            skillSlot1 = Input.GetButtonDown(SkillSlot1ButtonName);
            inventory = Input.GetButtonDown(InventoryButtonName);
            skillWindow = Input.GetButtonDown(SkillWindowButtonName);
            equipWindow = Input.GetButtonDown(EquipWindowButtonName);
            statusWindow = Input.GetButtonDown(StatusWindowButtonName);

            Mediator.Instance.Notify(this, GameEvent.INPUT_UPDATED, null);
        }
    }
}