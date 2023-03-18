using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using UnityEngine.EventSystems;

namespace Game
{
    public class SkillSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public SkillType SlotType { get; private set; } // Slot type
        public Image icon { get; private set; }  // Slot icon
        public Skill equippedSkill  { get; private set; }    // Equipped skill
        // Original parent and position for drag and drop
        private Transform originalParent;
        private Vector3 originalPosition;
        // Set the skill to the slot
        public void SetSlot(Skill skill)
        {
            equippedSkill = skill;
            icon.sprite = skill.skillInfo.skillImage;
            icon.enabled = true;
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
        // Handle the beginning of a drag and drop operation
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                originalParent = transform.parent;
                originalPosition = transform.position;

                transform.SetParent(transform.parent.parent);
                GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
        // Handle the dragging operation
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                transform.position = eventData.position;
            }
        }
        // Handle the end of a drag and drop operation
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsEmpty())
            {
                transform.SetParent(originalParent);
                transform.position = originalPosition;

                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
    }
}