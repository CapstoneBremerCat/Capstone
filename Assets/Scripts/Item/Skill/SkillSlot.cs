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
        private Rect baseRect;  // Inventory_Base �̹����� Rect ���� �޾� ��.
        // Set the skill to the slot
        public void SetSlot(Skill skill)
        {
            equippedSkill = skill;
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
        // �̹����� ���� ����
        private void SetColor(float _alpha)
        {
            Color color = icon.color;
            color.a = _alpha;
            icon.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("click�Ϸ�");
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("if1�Ϸ�");
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

        private void ChangeSlot()
        {
            var tempSkill = equippedSkill;
            SkillSlot skillSlot = DragSlot.instance.dragSlot.GetComponent<SkillSlot>();
            if (!skillSlot|| skillType != skillSlot.equippedSkill.skillInfo.skillType) return;
            SetSlot(skillSlot.equippedSkill);
            // Check if the dragged skill slot has an equipped skill
            if (tempSkill != null)
            {
  
                // �Ҵ�� ������ �κ��丮 -> ���â�� ���
                if (skillSlot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                {

                    GameManager.Instance.UnEquipSkill(tempSkill);
                    GameManager.Instance.EquipSkill(skillSlot.equippedSkill);
                }
                // ���â���� ������ ������ ���
                else if (skillSlot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                {
                    GameManager.Instance.UnEquipSkill(skillSlot.equippedSkill);
                    GameManager.Instance.EquipSkill(tempSkill);
                }
                // If the skill slot has an equipped skill, swap it with the current skill slot
                skillSlot.SetSlot(tempSkill);
            }
            else
            {
                // �Ҵ�� ������ �κ��丮 -> ���â�� ���
                if (skillSlot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                {
                    GameManager.Instance.EquipSkill(skillSlot.equippedSkill);
                }
                // ���â���� ������ ������ ���
                else if (skillSlot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                {
                    GameManager.Instance.UnEquipSkill(skillSlot.equippedSkill);
                }
                // If the skill slot doesn't have an equipped skill, just move the current skill to the dragged skill slot
                skillSlot.ClearSlot();
            }

        }

    }
}