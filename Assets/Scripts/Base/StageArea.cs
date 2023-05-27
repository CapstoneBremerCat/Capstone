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
            allGoalsCompleted = false; // �ʱ�ȭ
            for (int i = 0; i < goals.Length; i++)
            {
                goals[i].GoalCompleted += HandleGoalCompleted; // GoalCompleted �̺�Ʈ�� ���� �ڵ鷯 ���
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
            // ��� Goal���� Complete�Ǿ����� üũ
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
            // ��� Goal�� Complete�� ��� ���ϴ� ���� ����
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