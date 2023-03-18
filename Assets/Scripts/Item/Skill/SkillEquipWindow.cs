using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using System;

namespace Game
{
    public class SkillEquipWindow : MonoBehaviour
    {
        [SerializeField] private Slot weaponSlot;
        [SerializeField] private SkillSlot[] skillSlots;
        [SerializeField] private GameObject windowBase;
        private bool windowActivated = false;


        public void Initialize()
        {
            foreach (SkillSlot skillSlot in skillSlots)
            {
                skillSlot.ClearSlot();
            }
        }

        void Update()
        {
            // Toggle the inventory on/off with the K key
            if (Input.GetKeyDown(KeyCode.U))
            {
                windowActivated = !windowActivated;

                if (windowActivated)
                    OpenInventory();
                else
                    CloseInventory();
            }
        }
        // Open the skill inventory UI
        public void OpenInventory()
        {
            windowBase.SetActive(true);
        }
        // Close the skill inventory UI
        public void CloseInventory()
        {
            windowBase.SetActive(false);
        }
        // Ca
        public bool EquipSkill(Skill skill)
        {
            if (skill == null)
            {
                return false;
            }

            SkillSlot skillSlot = null;
            skillSlot = Array.Find(skillSlots, slot => slot.SkillType == skill.skillType);

            if (skillSlot == null)
            {
                Debug.LogWarning($"No equip slot found for skill {skill.skillInfo.skillName}");
                return false;
            }

            if (skillSlot.equippedSkill != null)
            {
                UnequipSkill(skillSlot.equippedSkill);
            }

            skillSlot.SetSlot(skill);
            GameManager.Instance.EquipSkill(skill);
            return true;
        }

        public bool UnequipSkill(Skill skill)
        {
            if (skill == null)
            {
                return false;
            }

            SkillSlot skillSlot = null;
            skillSlot = Array.Find(skillSlots, slot => slot.SkillType == skill.skillType);

            if (skillSlot == null)
            {
                Debug.LogWarning($"No equipped skill found in equip slots for skill {skill.skillInfo.skillName}");
                return false;
            }

            skillSlot.ClearSlot();
            GameManager.Instance.UnEquipSkill(skill);
            return true;
        }
    }
}
