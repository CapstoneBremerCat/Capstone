using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [CreateAssetMenu(fileName = "StatusObject", menuName = "ScriptableObjects/StatusObject", order = 0)]
    public class StatusObject : ScriptableObject
    {
        public int id;           // StatusObject id
        public int skillCode;           // StatusObject skill code
        public StatusData status;
    }
}