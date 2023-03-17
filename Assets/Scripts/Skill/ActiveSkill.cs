using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ActiveSkill : Skill
    {
        public float cooldown;

        public virtual void UseSkill()
        {
        }
    }
}
