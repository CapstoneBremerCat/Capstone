using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaController : MonoBehaviour
{
    // �ó׸� ������ ������ �� ȣ��
    public void EndCinema(string nextScene)
    {
        if (UIMgr.Instance) UIMgr.Instance.MoveScene(nextScene);
    }
}
