using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class Goal_Item : Goal
{
    private void Start()
    {
        var itemPickUp = gameObject.GetComponent<ItemPickUp>();
        itemPickUp.OnPickup += () =>
        {
            Complete();
        };
    }
}
