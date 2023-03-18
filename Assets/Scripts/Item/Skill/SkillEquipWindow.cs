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
        [SerializeField] private SkillSlot[] skillSlots;

        public void Initialize()
        {
            foreach (SkillSlot skillSlot in skillSlots)
            {
                skillSlot.ClearSlot();
            }
        }

        public bool EquipSkill(Skill skill)
        {
            if (skill == null)
            {
                return false;
            }

            SkillSlot skillSlot = null;
            skillSlot = Array.Find(skillSlots, slot => slot.SlotType == skill.skillType);

            if (skillSlot == null)
            {
                Debug.LogWarning($"No equip slot found for skill {skill.skillInfo.skillName}");
                return false;
            }

            if (skillSlot.equippedSkill != null)
            {
                Debug.LogWarning($"Equip slot {skillSlot.SlotType} is already occupied.");
                return false;
            }

            skillSlot.SetSlot(skill);
            return true;
        }

        public bool UnequipSkill(Skill skill)
        {
            if (skill == null)
            {
                return false;
            }

            SkillSlot skillSlot = null;
            skillSlot = Array.Find(skillSlots, slot => slot.SlotType == skill.skillType);

            if (skillSlot == null)
            {
                Debug.LogWarning($"No equipped skill found in equip slots for skill {skill.skillInfo.skillName}");
                return false;
            }

            skillSlot.ClearSlot();
            return true;
        }
    }
}
