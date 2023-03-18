/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using BlockChain;
namespace Game
{
    public class SkillManager
    {
        private List<Skill> skillList = new List<Skill>();
        private Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();

        public void Initialize()
        {
            List<BlockChain.Item> itemList = NFTManager.Instance.GetAllItems();

            // Create Skill objects from the filtered skill items and add them to the skill list and dictionary
            foreach (BlockChain.Item skillItem in itemList)
            {
                SkillInfo skillInfo = new SkillInfo(skillItem.tokenId, skillItem.name, skillItem.description, skillItem.image);
                Skill skill = new Skill(skillInfo);
                skillList.Add(skill);
                skillDictionary.Add(skill.skillInfo.skillId, skill);
            }
        }
        public Skill GetSkillByID(int skillID)
        {
            if (skillDictionary.ContainsKey(skillID))
            {
                return skillDictionary[skillID];
            }
            else
            {
                Debug.LogWarning($"SkillManager: Skill with ID {skillID} not found.");
                return null;
            }
        }
    }
}*/