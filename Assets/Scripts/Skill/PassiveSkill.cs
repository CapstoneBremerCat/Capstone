using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PassiveSkill : Skill
    {
        protected bool isActive;
        public float buffValue;

        public virtual void ApplyPassiveEffect(Status status)
        {
            // 스킬 효과를 적용하는 로직 구현
        }
    }
}