using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class NFTSkill
{
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
