using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PassiveSkill : Skill
    {
        [SerializeField] private StatusObject statusObject;
        public StatusData statusData { get { return statusObject.status; } }

        public PassiveSkill(SkillInfo skillInfo, StatusObject statusObject) : base(skillInfo)
        {
            this.skillInfo = skillInfo;
            this.statusObject = statusObject;
        }
    }
}