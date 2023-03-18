using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ActiveSkill : Skill
    {
        public float cooldown;

        public ActiveSkill(SkillInfo skillInfo, float cooldown) : base(skillInfo)
        {
            this.skillInfo = skillInfo;
            this.cooldown = cooldown;
        }

        public virtual void UseSkill()
        {
            
        }
    }
}
