using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class StageArea : MonoBehaviour
    {
        [SerializeField] private Goal[] goals;
        private void OnTriggerEnter(Collider other)
        {
            if (goals.Length > 0)
            {
                foreach (var goal in goals)
                {
                    if (!goal.isComplete) return;
                }
            }
            if (other.tag == "Player")
            {
                GameManager.Instance.StageClear();
            }
        }
    }
}