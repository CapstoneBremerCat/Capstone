using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Item item;
    public Item Item { get { return item; } }
    [SerializeField] private int remainingTime;

    void OnEnable()
    {
        if(remainingTime != 0)StartCoroutine(SpawnRemainingRoutine());
    }

    private IEnumerator SpawnRemainingRoutine(){
        yield return new WaitForSeconds(remainingTime);
        this.gameObject.SetActive(false);
    }
}
