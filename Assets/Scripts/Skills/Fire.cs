using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private Bolt bolt;
    [SerializeField] private Transform FirePos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bolt.gameObject, FirePos.transform.position, FirePos.transform.rotation);
        }

    }
}
