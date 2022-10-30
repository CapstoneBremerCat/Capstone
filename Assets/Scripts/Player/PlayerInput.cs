using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private string verticalAxisName = "Vertical";
    [SerializeField] private string horizontalAxisName = "Horizontal";
    [SerializeField] private string runButtonName = "Run";
    [SerializeField] private string jumpButtonName = "Jump";
    //[SerializeField] private string SkillSlot1ButtonName = "SkillSlot 1";
    //[SerializeField] private string SkillSlot2ButtonName = "SkillSlot 2";
    //[SerializeField] private string SkillSlot3ButtonName = "SkillSlot 3";
    //[SerializeField] private string SkillSlot4ButtonName = "SkillSlot 4";
    //[SerializeField] private string ItemSlotButtonName = "ItemSlot";

    public float vertical { get; private set; }
    public float horizontal { get; private set; }
    public bool run { get; private set; }
    public bool jump { get; private set; }
    //public bool skillSlot1 { get; private set; }
    //public bool skillSlot2 { get; private set; }
    //public bool skillSlot3 { get; private set; }
    //public bool skillSlot4 { get; private set; }
    //public bool itemSlot { get; private set; }

    //public event System.Action<float, float> MoveHandler;


    // Update is called once per frame
    void Update()
    {
        //MoveHandler?.Invoke(Input.GetAxis(moveAxisName), Input.GetAxis(horizontalAxisName));
        vertical = Input.GetAxis(verticalAxisName);
        horizontal = Input.GetAxis(horizontalAxisName);
        run = Input.GetButton(runButtonName);
        jump = Input.GetButton(jumpButtonName);
        //skillSlot1 = Input.GetButton(SkillSlot1ButtonName);
        //skillSlot2 = Input.GetButton(SkillSlot2ButtonName);
        //skillSlot3 = Input.GetButton(SkillSlot3ButtonName);
        //skillSlot4 = Input.GetButton(SkillSlot4ButtonName);
        //itemSlot = Input.GetButton(ItemSlotButtonName);
    }
}
