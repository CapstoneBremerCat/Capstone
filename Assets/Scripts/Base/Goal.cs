using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isComplete { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        isComplete = false;
    }

    public void Complete()
    {
        isComplete = true;
    }
}
