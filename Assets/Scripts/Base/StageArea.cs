using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class StageArea : MonoBehaviour
    {
        [SerializeField] private Achievement achievement;
        [SerializeField] private Goal[] goals;
        [SerializeField] private string clearDescription;
        private bool allGoalsCompleted;

        public void SetGoal()
        {
            allGoalsCompleted = false; // 초기화
            for (int i = 0; i < goals.Length; i++)
            {
                goals[i].GoalCompleted += HandleGoalCompleted; // GoalCompleted 이벤트에 대한 핸들러 등록
                goals[i].RegisterGoal(i);
                UIManager.Instance.SetDisplayGoal(i, goals[i].GetDisplayGoals());
            }
            if(goals.Length == 0)
            {
                StartCoroutine(UpdateGoalRountine());
            }
        }
        private void HandleGoalCompleted()
        {
            // 모든 Goal들이 Complete되었는지 체크
            allGoalsCompleted = true;
            foreach (var goal in goals)
            {
                if (!goal.isComplete)
                {
                    allGoalsCompleted = false;
                    break;
                }
            }
            if (allGoalsCompleted)
            {
                StartCoroutine(UpdateGoalRountine());
            }
        }

        private IEnumerator UpdateGoalRountine()
        {
            yield return new WaitForSeconds(1.5f);
            // 모든 Goal이 Complete된 경우 원하는 동작 수행
            UIManager.Instance.ReSetDisplayGoal();
            UIManager.Instance.SetDisplayGoal(0, clearDescription);

        }

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
            if (achievement != Achievement.Default) Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, achievement);
        }
    }
}