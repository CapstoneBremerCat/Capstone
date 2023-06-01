using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game;
namespace Game
{
    public class CinemaController : MonoBehaviour
    {
        [SerializeField] private Button skipButton;

        private void Start()
        {
            if (skipButton) skipButton.onClick.AddListener(() => {
                GameManager.Instance.MextScene();
                skipButton.gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().name.Contains("Cinema"))
                    StartCoroutine(OnSkip());
            }
        }

        // ��ŵ ��ư Ȱ��ȭ
        public IEnumerator OnSkip()
        {
            if (!skipButton) yield break;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            skipButton.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            skipButton.gameObject.SetActive(false);
        }
    }
}