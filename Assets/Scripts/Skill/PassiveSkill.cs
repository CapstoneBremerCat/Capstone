using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PassiveSkill : Skill
    {
        public StatusObject statusObject { get; private set; }

        public PassiveSkill(SkillInfo skillInfo, StatusObject statusObject) : base(skillInfo)
        {
            this.skillInfo = skillInfo;
            this.statusObject = statusObject;
        }
    }
}