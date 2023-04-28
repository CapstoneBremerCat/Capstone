using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class GetRingAndToMain : MonoBehaviour
{
    public void OnClickClaim()
    {
        GameManager.Instance.ToMain();
    }
}
