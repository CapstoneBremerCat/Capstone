using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
namespace Game
{
    public enum CavasIndex
    {
        Main,
        Cinema,
        Stage
    }
    public class UIManager : MonoBehaviour
    {
        #region instance
        private static UIManager instance = null;
        public static UIManager Instance { get { return instance; } }

        private void Awake()
        {
            // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance를 유일 오브젝트로 만든다
            instance = this;

            // Scene 이동 시 삭제 되지 않도록 처리
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        [SerializeField] private GameObject optionWindow;    // 옵션 창
        [SerializeField] private GameObject[] canvasList;    // 캔버스 목록
        [Header("Main UI")]
        [SerializeField] private Button startButton;    // 시작 버튼
        [SerializeField] private Button quitButton;    // 종료 버튼
        [Header("Cinema UI")]
        [SerializeField] private Button skipButton;
        [Header("Stage UI")]
        [SerializeField] private StageUIController stageUIController;
        public event System.Action RestartEvent;

        private void Start()
        {
            InitUI();
        }

        public void SwitchCanvas(CavasIndex index)
        {
            foreach(GameObject canvas in canvasList)
            {
                canvas.SetActive(false);
            }
            canvasList[(int)index].SetActive(true);
        }

        public void InitUI()
        {
            // 시작 버튼
            if (startButton) startButton.onClick.AddListener(() => GameManager.Instance.LoadScene());
            // 종료 버튼
            if (quitButton) quitButton.onClick.AddListener(() => Application.Quit());

            // 스킵 버튼
            //skipButton.onClick.AddListener(() => GameManager.Instance.MextScene());

            // 스테이지 UI
            if (!stageUIController) stageUIController = FindObjectOfType<StageUIController>();
            stageUIController.InitUI();
            DisableActiveSkill();
        }

        public void UpdateHealthBar(float ratio)
        {
            if (stageUIController != null)
            {
                stageUIController.SetHealthBar(ratio);
            }
        }

        public void UpdateStaminaBar(float ratio)
        {
            if (stageUIController != null)
            {
                stageUIController.SetStaminaBar(ratio);
            }
        }

        public void EnableActiveSkill(Sprite sprite)
        {
            stageUIController.SetActiveSkillSprite(sprite);
            stageUIController.SetEnableActiveSkill(true);
        }

        public void DisableActiveSkill()
        {
            stageUIController.SetEnableActiveSkill(false);
        }

        public void UpdateScoreText(int newScore)
        {
            if (stageUIController != null)
            {
                stageUIController.UpdateScoreText(newScore);
            }
        }
        public void EnableWaveUI()
        {
            if (stageUIController != null)
            {
                stageUIController.EnableWaveText();
            }
        }
        public void DisableWaveUI()
        {
            if (stageUIController != null)
            {
                stageUIController.DisableWaveText();
            }
        }
        public void UpdateWaveUI(int waves, int count)
        {
            if (stageUIController != null)
            {
                stageUIController.UpdateWaveText(waves, count);
            }
        }
        public void UpdateDateUI(int day)
        {
            if (stageUIController != null)
            {
                stageUIController.UpdateDateText(day);
            }
        }
        public void EnableGameOverUI()
        {
            if (stageUIController != null)
            {
                stageUIController.GameOver();
            }
        }
        public void PlayWaveStartUI()
        {
            if (stageUIController != null)
            {
                stageUIController.WaveStart();
            }
        }
        public void PlayWaveClearUI()
        {
            if (stageUIController != null)
            {
                stageUIController.WaveClear();
            }
        }
        public void PlayStageClearUI()
        {
            if (stageUIController != null)
            {
                stageUIController.StageClear();
            }
        }

        public void PlayNextDayUI(int nextDay)
        {
            if (stageUIController != null)
            {
                stageUIController.DisplayNextDay(nextDay);
            }
        }
        public void ToggleStatusUI()
        {
            if (stageUIController) stageUIController.ToggleStatusUI();
        }
        public void ToggleInventoryUI()
        {
            if (stageUIController) stageUIController.ToggleInventoryUI();
        }
        public void ToggleSkillWindowUI()
        {
            if (stageUIController) stageUIController.ToggleSkillWindowUI();
        }
        public void ToggleEquipWindowUI()
        {
            if (stageUIController) stageUIController.ToggleEquipWindowUI();
        }

        public void RestartGame()
        {
            stageUIController.InitUI();
            if (null != RestartEvent) RestartEvent();
        }
    }
}