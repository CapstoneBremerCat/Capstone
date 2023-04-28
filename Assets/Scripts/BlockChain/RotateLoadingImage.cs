using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingImage : MonoBehaviour
{
    public float rotationSpeed = -100f; // 회전 속도

    private RectTransform rectTransform; // RectTransform 컴포넌트

    private void Awake()
    {
        // RectTransform 컴포넌트 가져오기
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // 코루틴 시작
        StartCoroutine(Rotate());
    }

    private void OnDisable()
    {
        // 코루틴 정지
        StopCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            // z축 회전 각도 계산
            float angle = rotationSpeed * Time.deltaTime;
            Vector3 rotation = new Vector3(0f, 0f, angle);

            // z축 회전 적용
            rectTransform.Rotate(rotation);

            yield return null;
        }
    }
}





