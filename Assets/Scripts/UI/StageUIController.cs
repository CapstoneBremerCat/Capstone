using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StageUIController : MonoBehaviour
{
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

    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        //var strBuilder = new System.Text.StringBuilder();
        //strBuilder.Append(magAmmo);
        //strBuilder.Append(" / ");
        //strBuilder.Append(remainAmmo);
        ammoText.text = string.Format("{0} / {1}", magAmmo, remainAmmo);
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
        gameoverUI.SetActive(true);
    }

    public void Restart()
    {
        if (null != RestartEvent) RestartEvent();
    }
}
