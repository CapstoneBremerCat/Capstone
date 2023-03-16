using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
namespace Game
{
    public class CinemaController : MonoBehaviour
    {
        [SerializeField] private Button skipButton;

        private void Start()
        {
            skipButton.onClick.AddListener(() => GameManager.Instance.MextScene());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StartCoroutine(OnSkip());
            }
        }

        // 스킵 버튼 활성화
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
}