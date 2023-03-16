using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Skill : MonoBehaviour
    {
        protected SkillData skillData;

        public Skill(SkillData skillData)
        {
            this.skillData = skillData;
        }

        public virtual void UseSkill()
        {
            Debug.Log("Use Skill: " + skillData.skillName);
        }
    }
}