using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

        [SerializeField] private GameObject optionWindow;    // �ɼ� â
        [SerializeField] private GameObject[] canvasList;    // ĵ���� ���
        [SerializeField] private Button[] defaultButtons;    // �⺻ ��ư���� ����ϴ� ��ư ���
        [Header("Main UI")]
        [SerializeField] private Button startButton;    // ���� ��ư

        [SerializeField] private Button quitButton;    // ���� ��ư
        [Header("Cinema UI")]
        [SerializeField] private Button skipButton;
        [Header("Stage UI")]
        [SerializeField] private StageUIController stageUIController;
        [SerializeField] private PartnerHUD partnerHUD; // ��Ʈ�� ü�� ��.
        [SerializeField] private Text itemInfoText; // ������ ȹ�� UI
        [SerializeField] private TextMeshProUGUI[] stageGoalTexts; // �������� ��ǥ UI
        [SerializeField] private PopupController popupController; // �������� ��ǥ UI
        public event System.Action RestartEvent;

        private void Start()
        {
            for(int i = 0; i < defaultButtons.Length; i++)
            {
                int index = i; // i�� �ӽ� ������ ����
                defaultButtons[index].onClick.AddListener(() => {
                    StartCoroutine(BtnInterval(defaultButtons[index]));
                    SoundManager.Instance.OnPlaySFX("Default_Button");
                });
            }

            // ���� ��ư
            if (startButton) startButton.onClick.AddListener(() => GameManager.Instance.LoadScene());
            // ���� ��ư
            if (quitButton) quitButton.onClick.AddListener(() => Application.Quit());

            // ��ŵ ��ư
            //skipButton.onClick.AddListener(() => GameManager.Instance.MextScene());

            // �������� UI
            if (!stageUIController) stageUIController = FindObjectOfType<StageUIController>();
            InitUI();
            SetPartnerHUD(false);
        }
        public void ClearPopup()
        {
            popupController.ClearPopup();
        }

        // �˾��� ���� ���� �޼���
        public void OpenPopup(GameObject popup)
        {
            popupController.OpenPopup(popup);
        }

        // �˾��� �ݱ� ���� �޼���
        public void ClosePopup()
        {
            popupController.ClosePopup();
        }

        // �˾��� �����ϱ� ���� �޼���
        public void RemovePopup(GameObject popup)
        {
            popupController.RemovePopup(popup);
        }

        public void SetPartnerHUD(bool value)
        {
            partnerHUD.gameObject.SetActive(value);
        }
        public void SetPartnerHealthBar(float ratio)
        {
            if (partnerHUD) partnerHUD.SetHealthBar(ratio);
        }

        public void DisplayItemInfoText(string text)
        {
            itemInfoText.gameObject.SetActive(true);
            itemInfoText.text = text;
        }
        public void DisableItemInfoText()
        {
            itemInfoText.gameObject.SetActive(false);
        }
        public void SetDisplayGoal(int index, string contents)
        {
            if (!stageGoalTexts[index].gameObject.activeSelf)
                stageGoalTexts[index].gameObject.SetActive(true);
            stageGoalTexts[index].text = contents;
        }
        public void ReSetDisplayGoal()
        {
            foreach (var text in stageGoalTexts)
                text.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!SceneManager.GetActiveScene().name.Contains("Cinema") &&
                    popupController.IsPopupEmpty())
                {
                    bool isActive = optionWindow.activeSelf;
                    optionWindow.SetActive(!isActive);
                    GameManager.Instance.SetPause(!isActive);
                }
                else
                {
                    popupController.ClosePopup();
                }
            }
        }

        public void ClearCanvas()
        {
            foreach (GameObject canvas in canvasList)
            {
                canvas.SetActive(false);
            }
        }

        public void SwitchCanvas(CavasIndex index)
        {
            ClearCanvas();
            canvasList[(int)index].SetActive(true);
        }

        public void InitUI()
        {
            stageUIController.InitUI();
            DisableActiveSkill();
            DisableWaveUI();
            ReSetDisplayGoal();
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
        public void PlayStageClearUI(Timer clearTime, int clearScore)
        {
            if (stageUIController != null)
            {
                stageUIController.StageClear(clearTime, clearScore);
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

        public bool IsWindowOpen()
        {
            return stageUIController.IsPlayerWindowOpen();
        }

        public void RestartGame()
        {
            stageUIController.InitUI();
            if (null != RestartEvent) RestartEvent();
        }
        private IEnumerator BtnInterval(Button btn)
        {
            btn.interactable = false;
            yield return new WaitForSeconds(0.2f);
            btn.interactable = true;
        }
    }
}