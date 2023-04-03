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
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;

            // Scene �̵� �� ���� ���� �ʵ��� ó��
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        [SerializeField] private GameObject[] canvasList;    // ĵ���� ���
        [Header("Main UI")]
        [SerializeField] private Button startButton;    // ���� ��ư
        [SerializeField] private Button quitButton;    // ���� ��ư
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
            // ���� ��ư
            if (startButton) startButton.onClick.AddListener(() => GameManager.Instance.LoadScene());
            // ���� ��ư
            if (quitButton) quitButton.onClick.AddListener(() => Application.Quit());

            // ��ŵ ��ư
            //skipButton.onClick.AddListener(() => GameManager.Instance.MextScene());

            // �������� UI
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