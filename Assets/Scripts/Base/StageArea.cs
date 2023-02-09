using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.StageClear();
        }
    }
}
