using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    MORNING = 8,        // �� - ���� ����(����, ���, ����)
    NIGHT = 20       // �� - ���潺 ����(���̺� ���)
}

public class GameManager : MonoBehaviour
{
    #region instance
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }

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
        // DontDestroyOnLoad(this.gameObject);

        // DelayedUpdate�� 1�� �ĺ���(Start����) 1�ʸ��� �ݺ��ؼ� ����
        InvokeRepeating("DelayedUpdate", 1.0f, 1.0f);
    }
    #endregion

    [SerializeField] private GameObject player;     // �÷��̾�
    [SerializeField] private Spawner spawner; // ������

    public int Wave { get; private set; }  // ���� Wave ī��Ʈ.
    [SerializeField] private int maxWave = 10;  // �ִ� Wave ī��Ʈ.

    // �̹� Wave�� ������ Enemy�� ��.
    public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

    private int spawnCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.


    [SerializeField] private float maxFatigue = 100f;   // �ִ� �Ƿε�
    [SerializeField] private float recoverFatiguePerHour = 10f;   // ���� �ð��� ȸ���ϴ� �Ƿε� ��ġ
    public float CurFatigue { get; private set; }   // ���� �Ƿε�

    [SerializeField] private GAMEMODE gameMode;
    private int score = 0;

    private Timer playTime;
    private int lastSavedHour;   // ���������� ����� �ð�

    private const float sleepWaitPeriod = 0.5f;    // ���� �� �� �ð��� ���ϴ� ����

    // Start is called before the first frame update
    private void Start()
    {
        playTime = new Timer();
        CurFatigue = maxFatigue;
        gameMode = GAMEMODE.MORNING;
        Wave = 0;

        // ���� ��¥ UI ������ ����.
        UIMgr2.Instance.UpdateDateText(playTime.Day);
        // ���̺� UI ��Ȱ��ȭ
        UIMgr2.Instance.DisableWaveText();
        // ���� Wave UI ������ ����.
        // UIMgr2.Instance.UpdateWaveText(Wave, spawnCount);
    }

    // Update is called once per frame
    private void Update()
    {     
        //playerStatus.Updates();

    }

    private void DelayedUpdate()
    {
        playTime.UpdateTime();
        Debug.Log(playTime.Hour);

        /* 
         * �ð��� ��ȭ�� ���� ��� ��ȭ, �ϴ� ȸ��
        */

        // �ð��� ������ ��쿡�� �����Ѵ�.
        if (lastSavedHour != playTime.Hour)
        {
            switch (playTime.Hour)
            {
                case (int)GAMEMODE.MORNING:
                    gameMode = GAMEMODE.MORNING;
                    //EndWave();
                    NextDay();
                    // ���� off
                    break;
                case (int)GAMEMODE.NIGHT:
                    gameMode = GAMEMODE.NIGHT;
                    StartCoroutine(StartWave());
                    // ���� On
                    break;
            }
        }
        // ������ �ð� ����
        lastSavedHour = playTime.Hour;
    }

    public void DecreaseSpawnCount()
    {
        // spawnCount�� �����ϰ� UI������ �����Ѵ�.
        UIMgr2.Instance.UpdateWaveText(Wave, --spawnCount);

        // ��ȯ�� ���̺� ���� ��� óġ�Ͽ��� ��� ���̺� ����
        if(spawnCount <= 0)
        {
            StartCoroutine(EndWave());
        }
    }

    public IEnumerator StartWave()
    {
        // ���̺� ���� ���� ����

        // ������ ���� �� ���� ���
        yield return new WaitForSeconds(1.0f);

        // ���̺꿡 ���� �� ����
        Wave++;
        spawnCount += EnemySpawnCount;
        spawner.SpawnEnemy(spawnCount);

        // ���̺� UI Ȱ��ȭ
        UIMgr2.Instance.EnableWaveText();
        UIMgr2.Instance.UpdateWaveText(Wave, spawnCount);
        //NextWave();
    }

    public IEnumerator EndWave()
    {
        // ������ ���� �� ���� ���
        yield return new WaitForSeconds(1.0f);

        // ���̺� UI ��Ȱ��ȭ
        UIMgr2.Instance.DisableWaveText();
    }

    public void NextDay()
    {
        UIMgr2.Instance.UpdateDateText(playTime.Day);
    }

    // ������ �ð����� ���(����)
    public IEnumerator SleepHours(int hours)
    {
        for (int repeat = 0; repeat < hours; repeat++)
        {
            // UIMgr.Instance.DisplaySleepUI(hours);
            yield return new WaitForSeconds(sleepWaitPeriod);
            // �� �������� 1�ð��� �߰�
            playTime.AddTime(0, 1, 0, 0);
            // ����� �ð� �� �Ƿ� ��ġ ����. �ּ�ġ 0
            CurFatigue = Mathf.Max(0.0f, CurFatigue - recoverFatiguePerHour);
        }
    }

    public void GameOver()
    {

    }

    public void AddScore(int value)
    {
        score += value;
        // UI�� ScoreText�� ����.
        UIMgr2.Instance.UpdateScoreText(score);
    }

    public void ResetGame()
    {
        CurFatigue = maxFatigue;
        spawnCount = 0;
        Wave = 0;
    }
}
