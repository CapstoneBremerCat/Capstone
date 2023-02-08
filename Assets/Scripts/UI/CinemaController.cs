using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaController : MonoBehaviour
{
    // 시네마 영상이 끝났을 때 호출
    public void EndCinema(string nextScene)
    {
        if (UIMgr.Instance) UIMgr.Instance.MoveScene(nextScene);
    }
}
