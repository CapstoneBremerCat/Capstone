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
        //[SerializeField] private string SkillSlot2ButtonName = "SkillSlot 2";
        //[SerializeField] private string SkillSlot3ButtonName = "SkillSlot 3";
        //[SerializeField] private string SkillSlot4ButtonName = "SkillSlot 4";
        //[SerializeField] private string ItemSlotButtonName = "ItemSlot";

        public Vector2 moveInput { get; private set; }  // Horizontal, Vertical
        public bool run { get; private set; }
        public bool jump { get; private set; }
        public bool fire { get; private set; }
        public bool reload { get; private set; }
        public bool skillSlot1 { get; private set; }
        //public bool skillSlot2 { get; private set; }
        //public bool skillSlot3 { get; private set; }
        //public bool skillSlot4 { get; private set; }
        //public bool itemSlot { get; private set; }

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

                return;
            }

            //MoveHandler?.Invoke(Input.GetAxis(moveAxisName), Input.GetAxis(horizontalAxisName));
            moveInput = new Vector2(Input.GetAxis(horizontalAxisName), Input.GetAxis(verticalAxisName));

            if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;

            run = Input.GetButton(runButtonName);
            jump = Input.GetButton(jumpButtonName);
            fire = Input.GetButton(fireBtnName);
            reload = Input.GetButton(reloadBtnName);
            skillSlot1 = Input.GetButton(SkillSlot1ButtonName);
            //skillSlot2 = Input.GetButton(SkillSlot2ButtonName);
            //skillSlot3 = Input.GetButton(SkillSlot3ButtonName);
            //skillSlot4 = Input.GetButton(SkillSlot4ButtonName);
            //itemSlot = Input.GetButton(ItemSlotButtonName);
        }
    }
}