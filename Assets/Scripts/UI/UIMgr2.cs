using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIMgr2 : MonoBehaviour
{
    public static UIMgr2 Instance { get; private set; }

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private TextMeshProUGUI ammoText; // ź�� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private Text scoreText; // ���� ǥ�ÿ� �ؽ�Ʈ..
    [SerializeField] private Text dayText; // 
    [SerializeField] private Text waveText; // �� ���̺� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private GameObject gameoverUI; // ���� ������ Ȱ��ȭ�� UI.
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
        waveText.text = string.Format("Wave : {0}\nEnemy Left : {1}", waves, count);
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
