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
            // ��ų ȿ���� �����ϴ� ���� ����
        }
    }
}