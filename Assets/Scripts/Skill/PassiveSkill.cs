using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PassiveSkill : Skill
    {
/*        [SerializeField] private StatusObject statusObject;*/
        public StatusData statusData { get; private set; }
        public void SetStatusData(StatusData statusData)
        {
            this.statusData = statusData;
        }
    }
}