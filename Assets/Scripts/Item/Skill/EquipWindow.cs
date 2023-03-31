using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using System;

namespace Game
{
    public class EquipWindow : MonoBehaviour
    {
        [SerializeField] private Slot weaponSlot;
        [SerializeField] private SkillSlot[] skillSlots;
        [SerializeField] private GameObject windowBase;

        private void Start()
        {
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_WEAPON, RefreshEquippedWeapon);
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshEquippedSkills);
        }

        public void Initialize()
        {
            ClearAllSlots();
        }
        private void ClearAllSlots()
        {
            weaponSlot.ClearSlot();

            foreach (SkillSlot skillSlot in skillSlots)
            {
                skillSlot.ClearSlot();
            }
        }
        private void RefreshEquippedWeapon(object weaponItemObject)
        {
            if (EquipManager.Instance.EquippedWeapon == null)
            {
                weaponSlot.ClearSlot();
                return;
            }
            Item weaponItem = weaponItemObject as Item;
            weaponSlot.AddItem(weaponItem);
        }
        private void RefreshEquippedSkills(object playerObject)
        {
            ClearAllSlots();
            Player player = playerObject as Player;
            
            foreach (Skill skill in player.equippedPassiveSkills)
            {
                EquipSkill(skill);
            }
            EquipSkill(player.equippedActiveSkill);
        }

        // Ca
        public bool EquipSkill(Skill skill)
        {
            if (skill == null)
            {
                return false;
            }

            SkillSlot skillSlot = null;
            skillSlot = Array.Find(skillSlots, slot => slot.SkillType == skill.skillInfo.skillType);

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
            skillSlot = Array.Find(skillSlots, slot => slot.SkillType == skill.skillInfo.skillType);

            if (skillSlot == null)
            {
                Debug.LogWarning($"No equipped skill found in equip slots for skill {skill.skillInfo.skillName}");
                return false;
            }

            skillSlot.ClearSlot();
            GameManager.Instance.UnEquipSkill(skill);
            return true;
        }
        private void OnDestroy()
        {            
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshEquippedSkills);
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshEquippedWeapon);
        }
    }
}
