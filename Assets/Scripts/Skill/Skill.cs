using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public enum SkillType
    {
        Passive,
        Active
    }

    [System.Serializable]
    public class SkillInfo
    {
        public int skillId;
        public string skillName;
        public string skillDescription;
        public Sprite skillImage;

        public SkillInfo(int tokenId, string name, string description, Sprite image)
        {
            this.skillId = tokenId;
            this.skillName = name;
            this.skillDescription = description;
            this.skillImage = image;
        }
    }

    public class Skill: MonoBehaviour
    {
        private NFTSkill nftSkill;

        private void Start()
        {
            nftSkill = new NFTSkill();
        }

        public SkillInfo skillInfo;
        public SkillType skillType = SkillType.Passive;

        public void SetSkillInfo(SkillInfo skillInfo)
        {
            this.skillInfo = skillInfo;
        }
        public void SetSkill(SkillInfo skillInfo, int nftType)
        {
            this.skillInfo = skillInfo;
            if (nftType == 1)
                skillType = SkillType.Active;
            else if (nftType == 2)
                skillType = SkillType.Passive;
        }

    }
}