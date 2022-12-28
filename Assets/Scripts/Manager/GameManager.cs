using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    DAY,        // �� - ���� ����(����, ���, ����)
    NIGHT       // �� - ���潺 ����(���̺� ���)
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

        playTime = new PlayTime();

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

    private int spwanCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.


    [SerializeField] private float maxFatigue = 100f;   // �ִ� �Ƿε�
    public float CurFatigue { get; private set; }   // ���� �Ƿε�

    [SerializeField] private GAMEMODE gameMode;
    private int score = 0;
    private PlayTime playTime;
    private TimeData currTime;
    private const int MorningHour = 8;  // �� ���� �ð�. 8�ú���
    private const int NightHour = 20;   // �� ���� �ð�. 20�ú���

    // Start is called before the first frame update
    private void Start()
    {
        CurFatigue = maxFatigue;
        gameMode = GAMEMODE.DAY;
        Wave = 0;

        // ���� Wave UI ������ ����.
        UIMgr2.Instance.UpdateWaveText(GameManager.instance.Wave, spwanCount);
    }

    // Update is called once per frame
    private void Update()
    {     
        //playerStatus.Updates();

    }

    private int lastHour;
    private void DelayedUpdate()
    {
        currTime = playTime.GetTime();
        Debug.Log(currTime.Hour);

        // �ð��� ������ ��쿡�� �����Ѵ�.
        if (lastHour != currTime.Hour)
        {
            switch (currTime.Hour)
            {
                case MorningHour:
                    gameMode = GAMEMODE.DAY;
                    //EndWave();
                    break;
                case NightHour:
                    gameMode = GAMEMODE.NIGHT;
                    StartWave();
                    break;
            }
        }
        // ������ �ð� ����
        lastHour = currTime.Hour;
    }

    public void DecreaseSpawnCount()
    {
        // spawnCount�� �����ϰ� UI������ �����Ѵ�.
        UIMgr2.Instance.UpdateWaveText(Wave, --spwanCount);
    }

    public void StartWave()
    {
        Wave++;
        spwanCount = EnemySpawnCount;
        spawner.SpawnEnemy(spwanCount);
        UIMgr2.Instance.UpdateWaveText(Wave, spwanCount);
        //NextWave();
    }

/*    public bool NextWave()
    {
        if (maxWave >= Wave)
        {
            Wave++;
            return true;
        }
        return false;
    }*/

    public void EndWave()
    {

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
        Wave = 0;
    }
}
