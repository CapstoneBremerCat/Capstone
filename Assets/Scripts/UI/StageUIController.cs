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
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;
        }
        #endregion

        /*    [SerializeField] private RectTransform healthBar;   // ü�� ��
            [SerializeField] private float defaultLength = 400f;    // ü�� �� ����*/
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Text waveText; // �� ���̺� ǥ�ÿ� �ؽ�Ʈ.
        [SerializeField] private TextMeshProUGUI ammoText; // ź�� ǥ�ÿ� �ؽ�Ʈ.
        [SerializeField] private GameObject gameoverUI; // ���� ������ Ȱ��ȭ�� UI.
        [SerializeField] private GameObject sleepUI; // ���� UI.
        [SerializeField] private Animator waveStartUI; // ���̺� ���� UI.
        [SerializeField] private Animator waveClearUI; // ���̺� Ŭ���� UI.
        [SerializeField] private Animator stageClearUI; // �������� Ŭ���� UI.
        [SerializeField] private Animator nextDayUI; // ���� �� ���� UI.
        [SerializeField] private TextMeshProUGUI beforeDayText; // ���� ���� �ؽ�Ʈ
        [SerializeField] private TextMeshProUGUI afterDayText;  // ���� ���� �ؽ�Ʈ
        [SerializeField] private Text scoreText; // ���� ǥ�ÿ� �ؽ�Ʈ.
        [SerializeField] private Text dayText; // ��¥ ǥ�ÿ� �ؽ�Ʈ.
        public event System.Action RestartEvent;
        private float healthLength;
        [SerializeField] private Slider coolTimeSlider; // ��ų ��Ÿ�� �����̴�
        [SerializeField] private Animation coolTimeAnim; // ��ų ��Ÿ�� �ִϸ��̼�
        private bool isCoolTime;
        public void Init()
        {
            if (gameoverUI) gameoverUI.SetActive(false);
            StopAllCoroutines();
            coolTimeSlider.value = 0;
            isCoolTime = false;
        }

        // ��Ÿ�� ǥ��
        public bool DisplayCooltime(float timeRemaining)
        {
            if (isCoolTime) return false;
            StartCoroutine(CooltimeRountine(timeRemaining));
            return true;
        }

        IEnumerator CooltimeRountine(float timeRemaining)
        {
            isCoolTime = true;
            var totalTime = timeRemaining;
            var interval = 0.1f;
            while (timeRemaining > 0)
            {
                coolTimeSlider.value = timeRemaining / totalTime;
                timeRemaining -= interval;
                yield return new WaitForSeconds(interval);
            }
            if (coolTimeAnim)
            {
                // ��Ÿ�� ���� ����
                coolTimeAnim.Play();
            }
            isCoolTime = false;
        }

        /*    public void SetHealthBar(float ratio)
            {
                healthLength = GetHealthLength(ratio);
                //if (playerStatus.isHpFull) healthLength = defaultLength;

                healthBar.sizeDelta = new Vector2(healthLength, 30);
            }
            private float GetHealthLength(float ratio)
            {
                return defaultLength * ratio;
            }*/
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
            if (gameoverUI) gameoverUI.SetActive(true);
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

            // �ش� �ִϸ��̼� ���� �������� ���
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            };
            anim.gameObject.SetActive(false);
        }

        public void Restart()
        {
            Init();
            if (null != RestartEvent) RestartEvent();
        }
    }
}