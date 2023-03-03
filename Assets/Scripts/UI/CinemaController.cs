using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinemaController : MonoBehaviour
{
    [SerializeField] private Button skipButton;

    private void Start()
    {
        skipButton.onClick.AddListener(() => GameManager.Instance.MextScene());
    }

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
        skipButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        skipButton.gameObject.SetActive(false);
    }

    public void EndCinema()
    {
        GameManager.Instance.MextScene();
    }
}
