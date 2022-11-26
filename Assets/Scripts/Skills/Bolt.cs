using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bolt : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;   // 투사체의 rigidbody
    [SerializeField] private float force = 100.0f; // 가해지는 힘

    public void Start()
    {
        rigid.velocity = transform.forward * force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);   
    }
}
