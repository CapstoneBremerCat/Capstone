using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform cameraTransform; // ī�޶��� transform
    [SerializeField] private Transform unitTransform; // ī�޶� �ͼӵ� ������ transform
    [SerializeField] private float turnSpeed = 4.0f; // ���콺 ȸ�� �ӵ�
    [SerializeField] private float xRotate = 0.0f;   // x�� ȸ����
    [SerializeField] private float yRotate = 0.0f;   // x�� ȸ����
    [SerializeField] private float minRot = -45f;    // ���Ʒ� �ּ� ȸ����
    [SerializeField] private float maxRot = 80f;     // ���Ʒ� �ִ� ȸ����

    // Update is called once per frame
    void FixedUpdate()
    {        
        // ���Ʒ��� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� ȸ���� �� ���
        var xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        // ���Ʒ� ȸ���� �ּ�, �ִ밪 ���̷� ����(minRot : �ϴù���, maxRot : �ٴڹ���)
        xRotate = Mathf.Clamp(xRotate + xRotateSize, minRot, maxRot);

        // ī�޶� ȸ������ ī�޶� �ݿ�(x�ุ ȸ��)
        cameraTransform.localEulerAngles = new Vector3(xRotate, 0, 0);


        // �¿�� ������ ���콺 �̵��� * ȸ�� �ӵ�
        var yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // ���� y�� ȸ������ ���� ���ο� ȸ������ ���
        yRotate = unitTransform.eulerAngles.y + yRotateSize;
        // y�� ȸ������ �ش� ī�޶� �ͼӵ� ���ֿ� �ݿ�
        unitTransform.eulerAngles = new Vector3(0, yRotate, 0);

    }
}
