using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class Goal : MonoBehaviour
{
    [SerializeField] private Achievement achievement;
    [SerializeField] private string goalDescription;
    public event Action GoalCompleted;
    private int registerNum;
    public bool isComplete { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        isComplete = false;
    }

    public void Complete()
    {
        isComplete = true;
        if (achievement != Achievement.Default) Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, achievement);
        UIManager.Instance.SetDisplayGoal(registerNum, GetDisplayGoals());
        GoalCompleted?.Invoke();
    }
    public string GetDisplayGoals()
    {
        int num = isComplete ? 1 : 0;
        return string.Format("{0}  [{1}/{2}]", goalDescription, num,1);
    }
    public void RegisterGoal(int num)
    {
        registerNum = num;
    }
    public int GetRegisterNum()
    {
        return registerNum;
    }
}
