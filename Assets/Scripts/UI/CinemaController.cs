using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemaController : MonoBehaviour
{
    [SerializeField] private GameObject skipButtonObj;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(OnSkip());
        }
    }

    // 스킵 버튼 활성화
    public IEnumerator OnSkip()
    {
        skipButtonObj.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        skipButtonObj.SetActive(false);
    }

    // 시네마 영상이 끝났을 때 호출
    public void EndCinema()
    {
        if (GameManager.Instance) GameManager.Instance.MextScene();
    }
}
