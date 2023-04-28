using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingImage : MonoBehaviour
{
    public float rotationSpeed = -100f; // ȸ�� �ӵ�

    private RectTransform rectTransform; // RectTransform ������Ʈ

    private void Awake()
    {
        // RectTransform ������Ʈ ��������
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // �ڷ�ƾ ����
        StartCoroutine(Rotate());
    }

    private void OnDisable()
    {
        // �ڷ�ƾ ����
        StopCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            // z�� ȸ�� ���� ���
            float angle = rotationSpeed * Time.deltaTime;
            Vector3 rotation = new Vector3(0f, 0f, angle);

            // z�� ȸ�� ����
            rectTransform.Rotate(rotation);

            yield return null;
        }
    }
}




