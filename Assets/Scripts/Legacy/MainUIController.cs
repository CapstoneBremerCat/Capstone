using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
namespace Game
{
    public class MainUIController : MonoBehaviour
    {
        [SerializeField] private Button startButton;    // 시작 버튼
        [SerializeField] private Button shopButton;    // 상점(NFT) 버튼
        [SerializeField] private Button quitButton;    // 종료 버튼

        // Start is called before the first frame update
        void Start()
        {
            // 시작 버튼
            if (startButton) startButton.onClick.AddListener(() => GameManager.Instance.LoadScene());

            /* 수정 필요 */
            //if (shopButton) shopButton.onClick.AddListener(() => GameManager.Instance.LoadScene());

            // 종료 버튼
            if (quitButton) quitButton.onClick.AddListener(() => Application.Quit());
        }

    }
}