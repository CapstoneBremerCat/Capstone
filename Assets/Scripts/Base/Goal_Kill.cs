using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class Goal_Kill : Goal
{
    private void Start()
    {
        var status = gameObject.GetComponent<Status>();
        status.OnDeath += () =>
        {
            Complete();
        };
    }
}
