using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private Button startButton;    // ���� ��ư
    [SerializeField] private Button shopButton;    // ����(NFT) ��ư
    [SerializeField] private Button quitButton;    // ���� ��ư

    // Start is called before the first frame update
    void Start()
    {
        // ���� ��ư
        if (startButton) startButton.onClick.AddListener(() => GameManager.Instance.LoadScene());

        /* ���� �ʿ� */
        if (shopButton) shopButton.onClick.AddListener(() => GameManager.Instance.LoadScene());

        // ���� ��ư
        if (quitButton) quitButton.onClick.AddListener(() => Application.Quit());
    }

}
