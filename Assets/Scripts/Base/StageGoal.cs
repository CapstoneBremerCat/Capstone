using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGoal : MonoBehaviour
{
    // 해당 컴포넌트를 부착한 오브젝트가 비활성화되면 스테이지 클리어로 간주.
    private void OnDisable()
    {
        GameManager.Instance.StageClear();
    }
}
