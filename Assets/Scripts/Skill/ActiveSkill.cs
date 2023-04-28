using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ActiveSkill : Skill
    {
        [SerializeField] public float Cooldown;
        public float cooldown { get { return skillInfo.skillValue; } }

        public virtual void UseSkill()
        {
            Debug.Log("Skill Activated");
        }
    }
}
