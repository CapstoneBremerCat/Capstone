using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGoal : MonoBehaviour
{
    // �ش� ������Ʈ�� ������ ������Ʈ�� ��Ȱ��ȭ�Ǹ� �������� Ŭ����� ����.
    private void OnDisable()
    {
        GameManager.Instance.StageClear();
    }
}
