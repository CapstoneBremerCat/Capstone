using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using UnityEngine.EventSystems;

namespace Game
{
    public enum SlotType
    {
        Inventory,
        EquipWindow
    }
    public class SkillSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private SkillType skillType;    // Slot type
        [SerializeField] private SlotType slotType;    // Slot type

        public SkillType SkillType { get { return skillType; } } 
        public SlotType SlotType { get { return slotType; } }
        public Skill equippedSkill  { get; private set; }    // Equipped skill
        // Original parent and position for drag and drop
        private Transform originalParent;
        private Vector3 originalPosition;
        private Rect baseRect;  // Inventory_Base 이미지의 Rect 정보 받아 옴.
        // Set the skill to the slot
        public void SetSlot(Skill skill)
        {
            equippedSkill = skill;
            skillType = skill.skillInfo.skillType;
            icon.sprite = skill.skillInfo.skillImage;
            icon.enabled = true;
            baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
            SetColor(1);
        }
        // Clear the skill from the slot
        public void ClearSlot()
        {
            equippedSkill = null;
            icon.sprite = null;
            icon.enabled = false;
        }
        // Check if the slot is empty
        public bool IsEmpty()
        {
            return equippedSkill == null;
        }
        // 이미지의 투명도 조절
        private void SetColor(float _alpha)
        {
            Color color = icon.color;
            color.a = _alpha;
            icon.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("click완료");
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("if1완료");
                if (!IsEmpty())
                {
                    if (slotType == SlotType.Inventory)
                        GameManager.Instance.EquipSkill(equippedSkill);
                    else
                        GameManager.Instance.UnEquipSkill(equippedSkill);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                DragSlot.instance.dragSlot = this.gameObject;
                DragSlot.instance.DragSetImage(icon);
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                ChangeSlot();
            }
        }

        // 드랍한 지점의 슬롯에서 호출
        private void ChangeSlot()
        {
            var tempSkill = equippedSkill; // 현재 슬롯의 스킬
            SkillSlot skillSlot = DragSlot.instance.dragSlot.GetComponent<SkillSlot>(); // 드래그한 슬롯의 스킬
            if (!skillSlot) return;

            //장비창은 고정
            if (skillSlot.slotType == SlotType.EquipWindow && slotType == SlotType.EquipWindow)
            {
                return;
            }
            // Check if the dragged skill slot has an equipped skill
            if (tempSkill != null)
            {
                // 인벤토리-인벤토리일 경우
                if (skillSlot.slotType == SlotType.Inventory && slotType == SlotType.Inventory)
                {
                    SetSlot(skillSlot.equippedSkill);   // 드래그한 슬롯 장착
                    skillSlot.SetSlot(tempSkill);
                    return;
                }
                if (skillType != skillSlot.equippedSkill.skillInfo.skillType)
                {
                    return;
                }
                // 할당된 슬롯이 인벤토리 -> 장비창일 경우
                if (skillSlot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                {
                    GameManager.Instance.UnEquipSkill(tempSkill);
                    GameManager.Instance.EquipSkill(skillSlot.equippedSkill);
                }
                // 장비창에서 가져온 슬롯일 경우
                else if (skillSlot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                {
                    GameManager.Instance.UnEquipSkill(skillSlot.equippedSkill);
                    GameManager.Instance.EquipSkill(tempSkill);
                }
                SetSlot(skillSlot.equippedSkill);   // 드래그한 슬롯 장착
                skillSlot.SetSlot(tempSkill);
            }
            else
            {
                // 할당된 슬롯이 인벤토리 -> 장비창일 경우
                if (skillSlot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                {
                    if (skillType != skillSlot.equippedSkill.skillInfo.skillType)
                    {
                        return;
                    }
                    GameManager.Instance.EquipSkill(skillSlot.equippedSkill);
                }
                // 장비창에서 가져온 슬롯일 경우
                else if (skillSlot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                {
                    GameManager.Instance.UnEquipSkill(skillSlot.equippedSkill);
                }
                // If the skill slot doesn't have an equipped skill, just move the current skill to the dragged skill slot
                SetSlot(skillSlot.equippedSkill);   // 드래그한 슬롯 장착
                skillSlot.ClearSlot();
            }

        }

    }
}