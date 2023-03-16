using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [CreateAssetMenu(fileName = "New SkillData", menuName = "SkillData")]
    public class SkillData : ScriptableObject
    {
        public string skillName;
        public float coolTime;
        public float damage;
    }
}