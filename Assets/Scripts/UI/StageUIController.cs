using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
namespace Game
{
    public class StageUIController : MonoBehaviour
    {
        #region instance
        private static StageUIController instance = null;
        public static StageUIController Instance { get { return instance; } }

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
        }
        #endregion

        [Header("Player UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private TextMeshProUGUI ammoText; // Text for displaying ammo count
        [SerializeField] private Slider coolTimeSlider; // Slider for displaying the skill cooldown time
        [SerializeField] private Animation coolTimeAnim; // Animation for the skill cooldown time

        [Header("UI Animators")]
        [SerializeField] private Animator waveStartUI; // Wave start UI animator
        [SerializeField] private Animator waveClearUI; // Wave clear UI animator
        [SerializeField] private Animator stageClearUI; // Stage clear UI animator
        [SerializeField] private Animator nextDayUI; // Next day UI animator

        [Header("Stage UI")]
        [SerializeField] private Text scoreText; // Text for displaying the score
        [SerializeField] private Text waveText; // Text for displaying the enemy wave count
        [SerializeField] private TextMeshProUGUI beforeDayText; // Text for displaying the previous day
        [SerializeField] private TextMeshProUGUI afterDayText;  // Text for displaying the next day
        [SerializeField] private Text dayText;   // Text for displaying the day

        [Header("Game Over UI")]
        [SerializeField] private GameObject gameOverUI; // UI to display when the game is over

        [Header("Windows")]
        [SerializeField] private GameObject statusWindowUI; // Status window UI
        [SerializeField] private GameObject inventoryUI; // Inventory UI
        [SerializeField] private GameObject skillWindowUI; // Skill window UI
        [SerializeField] private GameObject equipWindowUI; // Equipment window UI
        [SerializeField] private GameObject sleepUI; // Sleep UI


        public void InitUI()
        {
            if (gameOverUI) gameOverUI.SetActive(false);
            StopAllCoroutines();
            coolTimeSlider.value = 0;
            Mediator.Instance.RegisterEventHandler(GameEvent.SKILL_ACTIVATED, DisplayCooltime);
        }
        public void ToggleStatusUI()
        {
            if (statusWindowUI) ToggleUI(statusWindowUI);
        }
        public void ToggleInventoryUI()
        {
            if (inventoryUI) ToggleUI(inventoryUI);
        }
        public void ToggleSkillWindowUI()
        {
            if (skillWindowUI) ToggleUI(skillWindowUI);
        }
        public void ToggleEquipWindowUI()
        {
            if (equipWindowUI) ToggleUI(equipWindowUI);
        }
        private void ToggleUI(GameObject ui)
        {
            if (ui) ui.SetActive(!ui.activeSelf);
        }
        // 쿨타임 표기
        public void DisplayCooltime(object activeSkillObj)
        {
            ActiveSkill skill = activeSkillObj as ActiveSkill;
            StartCoroutine(CooltimeRountine(skill.cooldown));
        }

        IEnumerator CooltimeRountine(float timeRemaining)
        {
            var totalTime = timeRemaining;
            var interval = 0.1f;
            while (timeRemaining > 0)
            {
                coolTimeSlider.value = timeRemaining / totalTime;
                timeRemaining -= interval;
                yield return new WaitForSeconds(interval);
            }
            coolTimeSlider.value = 0;
            if (coolTimeAnim)
            {
                // 쿨타임 연출 시작
                coolTimeAnim.Play();
            }
        }

        public void SetHealthBar(float ratio)
        {
            if (healthSlider) healthSlider.value = ratio;
        }

        public void SetStaminaBar(float ratio)
        {
            if (staminaSlider) staminaSlider.value = ratio;
        }

        public void UpdateAmmoText(int magAmmo, int remainAmmo)
        {
            //var strBuilder = new System.Text.StringBuilder();
            //strBuilder.Append(magAmmo);
            //strBuilder.Append(" / ");
            //strBuilder.Append(remainAmmo);
            if (ammoText) ammoText.text = string.Format("{0} / {1}", magAmmo, remainAmmo);
        }

        public void UpdateScoreText(int newScore)
        {
            scoreText.text = string.Format("Score : {0}", newScore);
        }

        public void EnableWaveText()
        {
            waveText.gameObject.SetActive(true);
        }
        public void DisableWaveText()
        {
            waveText.gameObject.SetActive(false);
        }
        public void UpdateWaveText(int waves, int count)
        {
            if (waveText.gameObject.activeSelf)
            {
                waveText.text = string.Format("Wave : {0}\nEnemy Left : {1}", waves, count);
            }
        }

        public void UpdateDateText(int day)
        {
            dayText.text = string.Format("Day - {0}", day);
        }

        public void GameOver()
        {
            if (gameOverUI) gameOverUI.SetActive(true);
        }

        public void WaveStart()
        {
            if (waveStartUI) StartCoroutine(PlayAnim(waveStartUI));
        }

        public void WaveClear()
        {
            if (waveClearUI) StartCoroutine(PlayAnim(waveClearUI));
        }

        public void StageClear()
        {
            if (stageClearUI) StartCoroutine(PlayAnim(stageClearUI));
        }

        public void DisplayNextDay(int nextDay)
        {
            if (beforeDayText) beforeDayText.text = (nextDay - 1).ToString();
            if (afterDayText) afterDayText.text = (nextDay).ToString();
            if (nextDayUI) StartCoroutine(PlayAnim(nextDayUI));
        }

        private IEnumerator PlayAnim(Animator anim)
        {
            anim.gameObject.SetActive(true);

            // 해당 애니메이션 종료 시점까지 대기
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            };
            anim.gameObject.SetActive(false);
        }
    }
}