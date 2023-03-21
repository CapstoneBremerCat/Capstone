using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Game;
using BlockChain;
namespace Game
{
    public class SkillWindow : MonoBehaviour
    {
        // UI elements
        [SerializeField] private GameObject go_SkillInventoryBase;
        [SerializeField] private Transform tf_SkillSlotsParent;
        [SerializeField] private SkillSlot[] skillSlots;

        private void Start()
        {
            // Get all the SkillSlots from the skillSlotsParent
            skillSlots = tf_SkillSlotsParent.GetComponentsInChildren<SkillSlot>();
            LoadOwnedSkills();
        }

        private void LoadOwnedSkills()
        {
            ClearAllSlots();
            List<Skill> skillList = NFTManager.Instance.GetOwnedSkills();
            if (skillList == null) return;
            foreach (Skill skill in skillList)
            {
                AddSkill(skill);
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
        public void ClearAllSlots()
        {
            foreach (SkillSlot slot in skillSlots)
            {
                slot.ClearSlot();
            }
        }

    }
}