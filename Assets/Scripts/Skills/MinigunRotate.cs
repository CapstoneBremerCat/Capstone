using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunRotate : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject barrel; //ȸ����ų �ѿ�
    [SerializeField] private float rotateSpeed; //ȸ���ӵ�
    // Update is called once per frame
    void Update()
    {
        if (playerInput.fire)
        {
            barrel.transform.Rotate(0f, 0f, rotateSpeed);
        }
    }
}
