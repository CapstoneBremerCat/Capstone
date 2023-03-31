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
        public SkillType skillType = SkillType.Passive;

        public SkillInfo(int tokenId, string name, string description, Sprite image, int nftType)
        {
            this.skillId = tokenId;
            this.skillName = name;
            this.skillDescription = description;
            this.skillImage = image;
            if (nftType == 1)
                skillType = SkillType.Active;
            else if (nftType == 2)
                skillType = SkillType.Passive;
        }
    }

    public class Skill: MonoBehaviour
    {
        public SkillInfo skillInfo;

        public void SetSkillInfo(SkillInfo skillInfo)
        {
            this.skillInfo = skillInfo;
        }
    }
}