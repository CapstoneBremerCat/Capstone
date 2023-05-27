using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public enum SkillType
    {
        Passive,
        Active,
        Reward
    }

    [System.Serializable]
    public class SkillInfo
    {
        public int skillId;
        public string skillName;
        public string skillDescription;
        public Sprite skillImage;
        public SkillType skillType = SkillType.Passive;
        public int skillCode;
        public int skillValue;

        public SkillInfo(int tokenId, string name, string description, Sprite image, string nftType)
        {
            this.skillId = tokenId;
            this.skillName = name;
            this.skillDescription = description;
            this.skillImage = image;
            string[] nftTypeArgs = nftType.Split('_');
            if (nftTypeArgs.Length == 3 && int.TryParse(nftTypeArgs[1], out int skillcodeResult) && int.TryParse(nftTypeArgs[2], out int skillrateResult))
            {
                skillCode = skillcodeResult;
                skillValue = skillrateResult;
            }
            else
            {
                // nftType의 형식이 맞지 않는 경우 에러 처리
                Debug.LogError("nftType의 형식이 맞지 않습니다.");
            }
            if (nftType[0] == 'A')
            {
                skillType = SkillType.Active;
            }
            else if (nftType[0] == 'P')
            {
                skillType = SkillType.Passive;
            }
            else if (nftType[0] == 'R')
            {
                skillType = SkillType.Reward;
            }
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