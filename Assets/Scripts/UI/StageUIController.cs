using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    [SerializeField] private RectTransform healthBar;   // 체력 바
    [SerializeField] private float defaultLength = 400f;    // 체력 바 길이
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Text waveText; // 적 웨이브 표시용 텍스트.
    [SerializeField] private TextMeshProUGUI ammoText; // 탄약 표시용 텍스트.
    [SerializeField] private GameObject gameoverUI; // 게임 오버시 활성화할 UI.
    [SerializeField] private GameObject sleepUI; // 수면 UI.
    [SerializeField] private Animator waveStartUI; // 웨이브 시작 UI.
    [SerializeField] private Animator waveClearUI; // 웨이브 클리어 UI.
    [SerializeField] private Animator stageClearUI; // 스테이지 클리어 UI.
    [SerializeField] private Animator nextDayUI; // 다음 날 연출 UI.
    [SerializeField] private TextMeshProUGUI beforeDayText; // 이전 요일 텍스트
    [SerializeField] private TextMeshProUGUI afterDayText;  // 다음 요일 텍스트
    [SerializeField] private Text scoreText; // 점수 표시용 텍스트.
    [SerializeField] private Text dayText; // 날짜 표시용 텍스트.
    public event System.Action RestartEvent;
    private float healthLength;

    public void Init()
    {
        gameoverUI.SetActive(false);
    }

    public void SetHealthBar(float ratio)
    {
        healthLength = GetHealthLength(ratio);
        //if (playerStatus.isHpFull) healthLength = defaultLength;

        healthBar.sizeDelta = new Vector2(healthLength, 30);
    }
    private float GetHealthLength(float ratio)
    {
        return defaultLength * ratio;
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

        // 해당 애니메이션 종료 시점까지 대기
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        };
        anim.gameObject.SetActive(false);
    }

    public void Restart()
    {
        if (null != RestartEvent) RestartEvent();
    }
}
