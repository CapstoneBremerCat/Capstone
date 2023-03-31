using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ActiveSkill : Skill
    {
        [SerializeField] public float cooldown;

        public virtual void UseSkill()
        {
            Debug.Log("Skill Activated");
        }
    }
}
