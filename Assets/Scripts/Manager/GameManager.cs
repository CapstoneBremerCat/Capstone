using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAMEMODE
{
    MORNING = 8,        // 낮 - 생존 게임(수집, 사냥, 건축)
    NIGHT = 20       // 밤 - 디펜스 게임(웨이브 방어)
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
        DontDestroyOnLoad(this.gameObject);

        // DelayedUpdate가 1초 후부터(Start이후) 1초마다 반복해서 실행
        InvokeRepeating("DelayedUpdate", 1.0f, 1.0f);
    }
    #endregion

    [SerializeField] private Player player;     // 플레이어
    [SerializeField] private PartnerAI partner;     // 플레이어
    [SerializeField] private Spawner spawner; // 스포너
    [SerializeField] private Transform startPoint; // 스테이지 시작 지점

    public int Wave { get; private set; }  // 현재 Wave 카운트.
    [SerializeField] private int maxWave = 10;  // 최대 Wave 카운트.

    // 이번 Wave에 출현할 Enemy의 수.
    public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

    private int spawnCount = 0; // 필드에 존재하는 Enemy의 수.

    [SerializeField] private float maxFatigue = 100f;   // 최대 피로도
    [SerializeField] private float recoverFatiguePerHour = 10f;   // 수면 시간당 회복하는 피로도 수치
    public float CurFatigue { get; private set; }   // 현재 피로도

    [SerializeField] private GAMEMODE gameMode;
    private int score = 0;

    private Timer playTime;
    private int lastSavedHour;   // 마지막으로 저장된 시간

    private const float sleepWaitPeriod = 0.5f;    // 수면 시 매 시간이 변하는 간격
    public bool isGameOver { get; private set; }    // 게임오버 여부
    public bool isGameStart { get; private set; }    // 게임시작 여부

    // Start is called before the first frame update
    private void Start()
    {
        isGameStart = false;
        isGameOver = false;

        if (player) player.gameObject.SetActive(false);
        if (partner) partner.gameObject.SetActive(false);

        //InitNewStage();
    }

    // Update is called once per frame
/*    private void Update()
    {     
        //playerStatus.Updates();

    }*/

    private void DelayedUpdate()
    {
        // 게임이 시작됐을 경우에만 실행
        if (!isGameStart) return;

        playTime.UpdateTime();
        Debug.Log(playTime.Hour);

        /* 
         * 시간의 변화에 따라 밝기 변화, 하늘 회전
        */

        // 시간이 변했을 경우에만 동작한다.
        if (lastSavedHour != playTime.Hour)
        {
            switch (playTime.Hour)
            {
                case (int)GAMEMODE.MORNING:
                    gameMode = GAMEMODE.MORNING;
                    //EndWave();
                    NextDay();
                    // 조명 off
                    break;
                case (int)GAMEMODE.NIGHT:
                    gameMode = GAMEMODE.NIGHT;
                    StartCoroutine(StartWave());
                    // 조명 On
                    break;
            }
        }
        // 마지막 시간 저장
        lastSavedHour = playTime.Hour;
    }

    // 스테이지 시작 세팅
    public void InitNewStage()
    {
        // 시작지점 가져오기
        startPoint = GameObject.FindWithTag("Start").transform;
        spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        if (startPoint && player)
        {
            player.gameObject.SetActive(false);
            // 플레이어 위치를 시작지점으로 변경
            if (player) player.transform.position = startPoint.position;
            player.gameObject.SetActive(true);
            player.InitStatus();
            // 파트너 위치도 변경
            if (partner)
            {
                partner.gameObject.SetActive(false);
                partner.transform.position = startPoint.position + new Vector3(3, 0, 0);
                partner.gameObject.SetActive(true);
            }
        }
        //CurFatigue = maxFatigue;

        // 플레이 시작 시간 저장.
        playTime = new Timer();
        // 현재 날짜 UI 정보를 갱신.
        UIMgr.Instance.UpdateDateText(playTime.Day);
        // 웨이브 UI 비활성화
        UIMgr.Instance.DisableWaveText();

        // 게임모드 초기화
        gameMode = GAMEMODE.MORNING;
        // 웨이브 수 초기화.
        Wave = 0;
        // 게임 시작 신호 활성화.
        isGameStart = true;

    }

    public void DecreaseSpawnCount()
    {
        // spawnCount를 감소하고 UI정보를 갱신한다.
        UIMgr.Instance.UpdateWaveText(Wave, --spawnCount);

        // 소환된 웨이브 적을 모두 처치하였을 경우 웨이브 종료
        if(spawnCount <= 0)
        {
            StartCoroutine(EndWave());
        }
    }

    public IEnumerator StartWave()
    {
        // 웨이브 시작 연출 실행
        UIMgr.Instance.WaveStart();
        // 연출이 끝날 때 까지 대기
        yield return null;

        // 웨이브에 맞춰 적 스폰
        Wave++;
        spawnCount += EnemySpawnCount;
        spawner.SpawnEnemy(spawnCount);

        // 웨이브 UI 활성화
        UIMgr.Instance.EnableWaveText();
        UIMgr.Instance.UpdateWaveText(Wave, spawnCount);
        //NextWave();
    }

    public IEnumerator EndWave()
    {
        // 웨이브 클리어 연출 실행
        UIMgr.Instance.WaveClear();
        // 연출이 끝날 때 까지 대기
        yield return null;

        // 웨이브 UI 비활성화
        UIMgr.Instance.DisableWaveText();
    }

    public void NextDay()
    {
        // 다음 날 연출 실행
        UIMgr.Instance.DisplayNextDay(playTime.Day);
        UIMgr.Instance.UpdateDateText(playTime.Day);
    }

    // 지정한 시간동안 대기(수면)
    public IEnumerator SleepHours(int hours)
    {
        for (int repeat = 0; repeat < hours; repeat++)
        {
            // UIMgr.Instance.DisplaySleepUI(hours);
            yield return new WaitForSeconds(sleepWaitPeriod);
            // 매 루프마다 1시간씩 추가
            playTime.AddTime(0, 1, 0, 0);
            // 대기한 시간 당 피로 수치 감소. 최소치 0
            CurFatigue = Mathf.Max(0.0f, CurFatigue - recoverFatiguePerHour);
        }
    }
    
    public void OnPause()
    {
        Time.timeScale = 0;
    }
    public void OffPause()
    {
        Time.timeScale = 1;
    }

    public void StageClear()
    {
        UIMgr.Instance.StageClear();
        player.OnGodMode();
        isGameStart = false;
    }

    public void GameOver()
    {
        isGameOver = true;
        isGameStart = false;
    }

    public void RestartGame()
    {
        isGameStart = true;
    }

    public void AddScore(int value)
    {
        score += value;
        // UI의 ScoreText를 갱신.
        UIMgr.Instance.UpdateScoreText(score);
    }

    public void ResetGame()
    {
        CurFatigue = maxFatigue;
        spawnCount = 0;
        Wave = 0;
    }
}
