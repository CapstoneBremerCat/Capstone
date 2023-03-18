using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PassiveSkill : Skill
    {
        public StatusData status { get; private set; }

        public PassiveSkill(SkillInfo skillInfo, StatusData status) : base(skillInfo)
        {
            this.skillInfo = skillInfo;
            this.status = status;
        }
    }
}