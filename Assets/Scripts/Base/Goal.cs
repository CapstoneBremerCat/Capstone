using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Achievement achievement;
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
    }
}
