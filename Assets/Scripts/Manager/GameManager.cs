using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    DAY,        // 낮 - 생존 게임(수집, 사냥, 건축)
    NIGHT       // 밤 - 디펜스 게임(웨이브 방어)
}

public class GameManager : MonoBehaviour
{
    #region instance
    private static GameManager instance = null;
    public static GameManager Instance { get { return instance; } }

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

        // Scene 이동 시 삭제 되지 않도록 처리
        // DontDestroyOnLoad(this.gameObject);

        playTime = new PlayTime();

        // DelayedUpdate가 1초 후부터(Start이후) 1초마다 반복해서 실행
        InvokeRepeating("DelayedUpdate", 1.0f, 1.0f);
    }
    #endregion

    [SerializeField] private GameObject player;     // 플레이어
    [SerializeField] private Spawner spawner; // 스포너

    public int Wave { get; private set; }  // 현재 Wave 카운트.
    [SerializeField] private int maxWave = 10;  // 최대 Wave 카운트.

    // 이번 Wave에 출현할 Enemy의 수.
    public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

    private int spwanCount = 0; // 필드에 존재하는 Enemy의 수.


    [SerializeField] private float maxFatigue = 100f;   // 최대 피로도
    public float CurFatigue { get; private set; }   // 현재 피로도

    [SerializeField] private GAMEMODE gameMode;
    private int score = 0;
    private PlayTime playTime;
    private TimeData currTime;
    private const int MorningHour = 8;  // 낮 시작 시간. 8시부터
    private const int NightHour = 20;   // 밤 시작 시간. 20시부터

    // Start is called before the first frame update
    private void Start()
    {
        CurFatigue = maxFatigue;
        gameMode = GAMEMODE.DAY;
        Wave = 0;

        // 현재 Wave UI 정보를 갱신.
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

        // 시간이 변했을 경우에만 동작한다.
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
        // 마지막 시간 저장
        lastHour = currTime.Hour;
    }

    public void DecreaseSpawnCount()
    {
        // spawnCount를 감소하고 UI정보를 갱신한다.
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
        // UI의 ScoreText를 갱신.
        UIMgr2.Instance.UpdateScoreText(score);
    }

    public void ResetGame()
    {
        CurFatigue = maxFatigue;
        Wave = 0;
    }
}
