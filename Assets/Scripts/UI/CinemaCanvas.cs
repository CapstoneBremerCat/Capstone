using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
public class CinemaCanvas : MonoBehaviour
{
    public void EndCinema()
    {
        GameManager.Instance.MextScene();
    }
}
