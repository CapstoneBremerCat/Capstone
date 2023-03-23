using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;
using BlockChain;
namespace Game
{
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

        [SerializeField] private List<string> sceneList;     // 씬 이름 리스트

        [Header("Component")]
        [SerializeField] private DayAndNight sun; // 태양
        [SerializeField] private GameObject playerPrefab;     // 플레이어 프리팹
        [SerializeField] private Player player;     // 플레이어

        [SerializeField] private GameObject partnerPrefab;     // 파트너 프리팹
        [SerializeField] private PartnerAI partner;     // 파트너
        [SerializeField] private Spawner spawner; // 스포너

        [SerializeField] private Transform startPoint; // 스테이지 시작 지점

        [Header("Game")]
        [SerializeField] private int maxWave = 10;  // 최대 Wave 카운트.
        public int Wave { get; private set; }  // 현재 Wave 카운트.
                                               // 이번 Wave에 출현할 Enemy의 수.
        public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

        private int spawnCount = 0; // 필드에 존재하는 Enemy의 수.

        [SerializeField] private float maxFatigue = 100f;   // 최대 피로도
        [SerializeField] private float recoverFatiguePerHour = 10f;   // 수면 시간당 회복하는 피로도 수치
        public float CurFatigue { get; private set; }   // 현재 피로도

        [SerializeField] private GAMEMODE gameMode;
        private int score = 0;

        private Timer playTime;
        [SerializeField] private float timeScale;
        private int lastSavedHour;   // 마지막으로 저장된 시간

        private const float sleepWaitPeriod = 0.5f;    // 수면 시 매 시간이 변하는 간격
        private const int MAX_EQUIPPED_PASSIVE_SKILLS = 4;
        public bool isGameOver { get; private set; }    // 게임오버 여부
        public bool isGameStart { get; private set; }    // 게임시작 여부

        // Start is called before the first frame update
        private void Start()
        {
            isGameStart = false;
            isGameOver = false;

            if (player) player.gameObject.SetActive(false);
            if (partner) partner.gameObject.SetActive(false);
            UIManager.Instance.SwitchCanvas(CavasIndex.Main);
        }

        // Update is called once per frame
        private void Update()
        {
            // 게임이 시작됐을 경우, 게임오버되지 않았을 경우에만 실행
            if (isGameStart && !isGameOver) stageUpdate();
            if (player) player.OnInputUpdated();
        }

        private void stageUpdate()
        {
            if (sun) sun.UpdateSun();
        }

        private void DelayedUpdate()
        {
            // 게임이 시작됐을 경우, 게임오버되지 않았을 경우에만 실행
            if (!isGameStart || isGameOver) return;

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
        public void DebugPlayer()
        {
            player.Init(startPoint.position);
        }
        // 스테이지 시작 세팅
        public void InitNewStage()
        {
            // 태양, 시작지점, 스포너 가져오기
            if(!sun) sun = GameObject.FindWithTag("Sun").GetComponent<DayAndNight>();
            if(!startPoint) startPoint = GameObject.FindWithTag("Start").transform;
            if(!spawner) spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

            player = FindObjectOfType<Player>();
            if(!player) player = LoadCharacter(playerPrefab).GetComponent<Player>();
            if (startPoint && player)
            {
                // 플레이어 위치를 시작지점으로 변경
                player.Init(startPoint.position);
                player.gameObject.SetActive(true);
            }
            // 파트너 위치도 변경
            if (!partner) partner = LoadCharacter(partnerPrefab).GetComponent<PartnerAI>();
            if (partner)
            {
                partner.transform.position = startPoint.position + new Vector3(3, 0, 0);
                partner.gameObject.SetActive(true);
            }
            //CurFatigue = maxFatigue;

            // 게임모드 초기화
            gameMode = GAMEMODE.MORNING;
            // 웨이브 수 초기화.
            Wave = 0;
            // 플레이 시작 시간 저장.
            playTime = new Timer();
            playTime.SetTimeScale(timeScale);
            // 태양 초기화
            sun.InitSun();

            // 현재 날짜 UI 정보를 갱신.
            UIManager.Instance.UpdateDateUI(playTime.Day);
            // 웨이브 UI 비활성화
            UIManager.Instance.DisableWaveUI();

            // 게임 시작 신호 활성화.
            isGameStart = true;

            Mediator.Instance.Notify(this, GameEvent.REFRESH_STATUS, player);
        }
        private GameObject LoadCharacter(GameObject prefab)
        {
            // Check if the character prefab is assigned
            if (prefab == null)
            {
                Debug.LogError("playerPrefab is not assigned.");
                return null;
            }

            // Load the character prefab
            GameObject character = Instantiate(prefab);

            return character;
        }
        public void EquipSkill(Skill skill)
        {
            if (skill != null && skill.skillType == SkillType.Passive && player.GetEquippedPassiveSkillCount() < MAX_EQUIPPED_PASSIVE_SKILLS)
            {
                PassiveSkill passiveSkill = skill as PassiveSkill;
                if (passiveSkill != null)
                {
                    player.EquipPassiveSkill(passiveSkill);
                }
            }
            else if (skill != null && skill.skillType == SkillType.Active && player.equippedActiveSkill == null)
            {
                ActiveSkill activeSkill = skill as ActiveSkill;
                if (activeSkill != null)
                {
                    player.EquipActiveSkill(activeSkill);
                }
            }
        }
        public void UnEquipSkill(Skill skill)
        {
            if (skill != null && skill.skillType == SkillType.Passive)
            {
                PassiveSkill passiveSkill = skill as PassiveSkill;
                if (passiveSkill != null)
                {
                    player.UnequipPassiveSkill(passiveSkill);
                }
            }
            else if (skill != null && skill.skillType == SkillType.Active)
            {
                ActiveSkill activeSkill = skill as ActiveSkill;
                if (activeSkill != null && player.equippedActiveSkill == activeSkill)
                {
                    player.UnequipActiveSkill();
                }
            }
        }

        public void DecreaseSpawnCount()
        {
            // spawnCount를 감소하고 UI정보를 갱신한다.
            UIManager.Instance.UpdateWaveUI(Wave, --spawnCount);

            // 소환된 웨이브 적을 모두 처치하였을 경우 웨이브 종료
            if (spawnCount <= 0)
            {
                StartCoroutine(EndWave());
            }
        }

        public IEnumerator StartWave()
        {
            // 웨이브 시작 연출 실행
            UIManager.Instance.PlayWaveStartUI();

            /* 연출이 끝날 때 까지 대기 */
            yield return null;

            // 웨이브에 맞춰 적 스폰
            Wave++;
            spawnCount += EnemySpawnCount;
            spawner.SpawnEnemy(spawnCount, Wave);

            // 웨이브 UI 활성화
            UIManager.Instance.EnableWaveUI();
            UIManager.Instance.UpdateWaveUI(Wave, spawnCount);
        }

        public IEnumerator EndWave()
        {
            // 웨이브 클리어 연출 실행
            UIManager.Instance.PlayWaveClearUI();
            // 웨이브 UI 비활성화
            UIManager.Instance.DisableWaveUI();
            // 연출이 끝날 때 까지 대기
            yield return null;
        }

        public void NextDay()
        {
            // 다음 날 연출 실행
            UIManager.Instance.PlayNextDayUI(playTime.Day);
            UIManager.Instance.UpdateDateUI(playTime.Day);
        }

        // 지정한 시간동안 대기(수면)
        public IEnumerator SleepHours(int hours)
        {
            for (int repeat = 0; repeat < hours; repeat++)
            {
                // UIManager.Instance.DisplaySleepUI(hours);
                yield return new WaitForSeconds(sleepWaitPeriod);
                // 매 루프마다 1시간씩 추가
                playTime.AddTime(0, 1, 0, 0);
                // 대기한 시간 당 피로 수치 감소. 최소치 0
                CurFatigue = Mathf.Max(0.0f, CurFatigue - recoverFatiguePerHour);
            }
        }

        public void KillEnemy(bool isWaveEnemy)
        {
            AddScore(100); // enemy 처치 시, 100 score 상승.
            if (isWaveEnemy) DecreaseSpawnCount(); // enemy 처치 시, Spawn Count 감소.
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
            player.OnGodMode();
            isGameStart = false;
            UIManager.Instance.PlayStageClearUI();
        }

        public void GameOver()
        {
            isGameOver = true;
            isGameStart = false;
            UIManager.Instance.EnableGameOverUI();
        }


        public void AddScore(int value)
        {
            score += value;
            // UI의 ScoreText를 갱신.
            UIManager.Instance.UpdateScoreText(score);
        }
        public void RestartGame()
        {
            StartCoroutine(RestartScene());
        }

        public IEnumerator RestartScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

            // 씬 로드가 완료될 때까지 대기
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // 로드 완료 후 실행할 코드
            InitNewStage();
            UIManager.Instance.RestartGame();
            Mediator.Instance.Notify(this, GameEvent.RESTART, null);
        }

        public void MextScene()
        {
            // 씬 명명 규칙 1: 앞자리 두 문자는 00~99 두 자리 숫자로 시작해야한다.
            // 씬 명명 규칙 2: 인게임(스테이지) 의 경우에는 씬 이름에 반드시 Stage가 포함되어야 한다.
            // 현재 씬의 숫자를 참조하여 다음 씬으로 이동한다.
            if (int.TryParse(SceneManager.GetActiveScene().name.Substring(0, 2), out int currSceneIdx))
                StartCoroutine(MoveScene(currSceneIdx + 1));
        }

        public IEnumerator MoveScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneList[sceneIndex]);
            var async = SceneManager.LoadSceneAsync(sceneList[sceneIndex]);

            // 씬 이동이 끝날 때 까지 대기
            while (!async.isDone)
            {
                yield return null;
            }

            // 이동한 씬이 Main 면 스테이지 초기화
            if (sceneList[sceneIndex].Contains("Main"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Main);
            }
            // 이동한 씬이 Stage 면 스테이지 초기화
            if (sceneList[sceneIndex].Contains("Stage"))
            {
                InitNewStage();
                UIManager.Instance.SwitchCanvas(CavasIndex.Stage);
            }
            // 이동한 씬이 Cinema 면 스테이지 초기화
            if (sceneList[sceneIndex].Contains("Cinema"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Cinema);
            }
        }

        public void LoadScene()
        {
            int sceneIndex = 1;
            /* 저장된 인덱스(씬 위치) 불러오기 */
            StartCoroutine(MoveScene(sceneIndex));
        }

        public void ResetGame()
        {
            CurFatigue = maxFatigue;
            spawnCount = 0;
            Wave = 0;
        }
    }
}