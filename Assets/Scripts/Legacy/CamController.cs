using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject player; // 바라볼 플레이어
    public float xmove = 0;  // X축 누적 이동량
    public float ymove = 0;  // Y축 누적 이동량
    public float distance = 3;
    [SerializeField] float m_MaxRayDist = 1;
    [SerializeField] float m_Zoom = 3f;
    RaycastHit m_Hit;

    // Update is called once per frame
    void Update()
    {
        //Zoom();
        //  마우스 우클릭 중에만 카메라 무빙 적용
        //if (Input.GetMouseButton(1))
        {
            xmove += Input.GetAxis("Mouse X"); // 마우스의 좌우 이동량을 xmove 에 누적합니다.
            ymove -= Input.GetAxis("Mouse Y"); // 마우스의 상하 이동량을 ymove 에 누적합니다.
        }
        transform.rotation = Quaternion.Euler(ymove, xmove, 0); // 이동량에 따라 카메라의 바라보는 방향을 조정합니다.
        //Vector3 reverseDistance = new Vector3(0.0f, 0.0f, distance); // 카메라가 바라보는 앞방향은 Z 축입니다. 이동량에 따른 Z 축방향의 벡터를 구합니다.
        //transform.position = player.transform.position - transform.rotation * reverseDistance + new Vector3(0.0f, 1.0f, 0.0f); // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감합니다.
    }
    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Transform cam = Camera.main.transform;
            if (CheckRay(cam, scroll))
            {
                Vector3 targetDist = cam.transform.position - player.transform.position;
                targetDist = Vector3.Normalize(targetDist);
                Camera.main.transform.position -= (targetDist * scroll * m_Zoom);
            }
        }

        Camera.main.transform.LookAt(player.transform);
    }

    bool CheckRay(Transform cam, float scroll)
    {
        if (Physics.Raycast(cam.position, transform.forward, out m_Hit, m_MaxRayDist))
        {
            Debug.Log("hit point : " + m_Hit.point + ", distance : " + m_Hit.distance + ", name : " + m_Hit.collider.name);
            Debug.DrawRay(cam.position, transform.forward * m_Hit.distance, Color.red);
            cam.position += new Vector3(0, 0, m_Hit.point.z);
            return false;
        }

        return true;
    }
}
