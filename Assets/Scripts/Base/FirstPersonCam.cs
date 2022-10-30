using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private Transform cameraTransform; // 카메라의 transform
    [SerializeField] private Transform unitTransform; // 카메라가 귀속된 유닛의 transform
    [SerializeField] private float turnSpeed = 4.0f; // 마우스 회전 속도
    [SerializeField] private float xRotate = 0.0f;   // x축 회전량
    [SerializeField] private float yRotate = 0.0f;   // x축 회전량
    [SerializeField] private float minRot = -45f;    // 위아래 최소 회전값
    [SerializeField] private float maxRot = 80f;     // 위아래 최대 회전값

    // Update is called once per frame
    void FixedUpdate()
    {        
        // 위아래로 움직인 마우스의 이동량 * 속도에 따라 카메라가 회전할 양 계산
        var xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        // 위아래 회전을 최소, 최대값 사이로 제한(minRot : 하늘방향, maxRot : 바닥방향)
        xRotate = Mathf.Clamp(xRotate + xRotateSize, minRot, maxRot);

        // 카메라 회전량을 카메라에 반영(x축만 회전)
        cameraTransform.localEulerAngles = new Vector3(xRotate, 0, 0);


        // 좌우로 움직인 마우스 이동량 * 회전 속도
        var yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // 현재 y축 회전값에 더한 새로운 회전각도 계산
        yRotate = unitTransform.eulerAngles.y + yRotateSize;
        // y축 회전량은 해당 카메라가 귀속된 유닛에 반영
        unitTransform.eulerAngles = new Vector3(0, yRotate, 0);

    }
}
