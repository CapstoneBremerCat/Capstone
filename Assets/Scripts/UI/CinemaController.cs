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

    // ��ŵ ��ư Ȱ��ȭ
    public IEnumerator OnSkip()
    {
        skipButtonObj.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        skipButtonObj.SetActive(false);
    }

    // �ó׸� ������ ������ �� ȣ��
    public void EndCinema()
    {
        if (GameManager.Instance) GameManager.Instance.MextScene();
    }
}
