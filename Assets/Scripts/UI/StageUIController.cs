using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StageUIController : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar;   // ü�� ��
    [SerializeField] private float defaultLength = 400f;    // ü�� �� ����
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
