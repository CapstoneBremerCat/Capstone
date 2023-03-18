using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.EventSystems;
using System;

namespace Game
{
    public class SkillInventory : MonoBehaviour, IDropHandler
    {
        // Singleton instance
        private static SkillInventory instance;
        public static SkillInventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SkillInventory>();
                }
                return instance;
            }
        }

        // UI elements
        [SerializeField] private GameObject go_SkillInventoryBase;
        [SerializeField] private Transform tf_SkillSlotsParent;
        [SerializeField] private SkillSlot[] skillSlots;
        private bool inventoryActivated = false;


        private void Start()
        {
            // Get all the SkillSlots from the skillSlotsParent
            skillSlots = tf_SkillSlotsParent.GetComponentsInChildren<SkillSlot>();
        }

        void Update()
        {
            // Toggle the inventory on/off with the K key
            if (Input.GetKeyDown(KeyCode.K))
            {
                inventoryActivated = !inventoryActivated;

                if (inventoryActivated)
                    OpenInventory();
                else
                    CloseInventory();
            }
        }
        // Open the skill inventory UI
        public void OpenInventory()
        {
            go_SkillInventoryBase.SetActive(true);
        }
        // Close the skill inventory UI
        public void CloseInventory()
        {
            go_SkillInventoryBase.SetActive(false);
        }
        // Called when a skill slot is dropped onto another skill slot
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                SkillSlot draggedSlot = eventData.pointerDrag.GetComponent<SkillSlot>();
                SkillSlot targetSlot = eventData.pointerEnter.GetComponent<SkillSlot>();

                if (draggedSlot != null && targetSlot != null)
                {
                    // Swap the equipped skill between the two slots
                    Skill draggedSkill = draggedSlot.equippedSkill;
                    Skill targetSkill = targetSlot.equippedSkill;

                    if (draggedSkill != null && targetSkill != null)
                    {
                        draggedSlot.SetSlot(targetSkill);
                        targetSlot.SetSlot(draggedSkill);
                    }
                }
            }
        }
        // Add a skill to the inventory
        public bool AddSkill(Skill skill)
        {
            // Find an empty skill slot to add the skill
            SkillSlot emptySlot = Array.Find(skillSlots, slot => slot.IsEmpty());
            if (emptySlot == null)
            {
                Debug.LogWarning("Cannot add skill. No empty skill slot available.");
                return false;
            }

            emptySlot.SetSlot(skill);
            return true;
        }
        // Remove a skill from the inventory
        public void RemoveSkill(Skill skill)
        {
            // Find the skill slot containing the skill to remove
            SkillSlot slotToRemove = Array.Find(skillSlots, slot => slot.equippedSkill == skill);
            if (slotToRemove == null)
            {
                Debug.LogWarning("Cannot remove skill. The skill is not equipped in any skill slot.");
                return;
            }

            slotToRemove.ClearSlot();
        }
    }
}