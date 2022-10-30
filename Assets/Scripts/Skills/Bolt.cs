using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bolt : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;   // ����ü�� rigidbody
    [SerializeField] private float force = 1.0f; // �������� ��

    public void Start()
    {
        rigid.velocity = Camera.main.transform.forward * force;
    }
}