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
            DontDestroyOnLoad(this.gameObject);

            // DelayedUpdate�� 1�� �ĺ���(Start����) 1�ʸ��� �ݺ��ؼ� ����
            InvokeRepeating("DelayedUpdate", 1.0f, 1.0f);
        }
        #endregion

        [SerializeField] private List<string> sceneList;     // �� �̸� ����Ʈ

        [Header("Component")]
        [SerializeField] private DayAndNight sun; // �¾�
        [SerializeField] private GameObject playerPrefab;     // �÷��̾� ������
        [SerializeField] private Player player;     // �÷��̾�

        [SerializeField] private GameObject[] partnerPrefabs;     // ��Ʈ�� ������
        [SerializeField] private PartnerAI partner;     // ��Ʈ��
        [SerializeField] private Spawner spawner; // ������

        private Transform startPoint; // �������� ���� ����
        private StageArea stageGoal; // �������� ��ǥ

/*        [Header("Game")]
        [SerializeField] private int maxWave = 10;  // �ִ� Wave ī��Ʈ.*/
        public int Wave { get; private set; }  // ���� Wave ī��Ʈ.
                                               // �̹� Wave�� ������ Enemy�� ��.
        public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

        private int spawnCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.
        public int enemyKilledCount { get; private set; } // �ʵ忡 �����ϴ� Enemy�� ��.

        //[SerializeField] private float maxFatigue = 100f;   // �ִ� �Ƿε�
        //[SerializeField] private float recoverFatiguePerHour = 10f;   // ���� �ð��� ȸ���ϴ� �Ƿε� ��ġ
        //public float CurFatigue { get; private set; }   // ���� �Ƿε�

        private int score = 0;
        public int highScore { get; private set; }
        public Timer clearTime { get; private set; }
        public Timer playTime { get; private set; }
        [SerializeField] private float timeScale;
        //[SerializeField] private GAMEMODE gameMode;
        private int lastSavedHour;   // ���������� ����� �ð�

        private const float sleepWaitPeriod = 0.5f;    // ���� �� �� �ð��� ���ϴ� ����
        private const int MAX_EQUIPPED_PASSIVE_SKILLS = 4;
        public bool isGameOver { get; private set; }    // ���ӿ��� ����
        public bool isGameStart { get; private set; }    // ���ӽ��� ����
        public bool isRankStart { get; private set; }    // ���ӽ��� ����
        public bool isOnNFT { get; private set; }
        public void StartNFT()
        {
            isOnNFT = true;
        }
        // Start is called before the first frame update
        private void Start()
        {
            isOnNFT = false;
            isGameStart = false;
            isRankStart = false;
            isGameOver = false;

            if (player) player.gameObject.SetActive(false);
            if (partner) partner.gameObject.SetActive(false);
            UIManager.Instance.SwitchCanvas(CavasIndex.Main);
            SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyMain);
        }

        // Update is called once per frame
        private void Update()
        {
            // ������ ���۵��� ���, ���ӿ������� �ʾ��� ��쿡�� ����
            if (isGameStart && !isGameOver) stageUpdate();
            if (player) player.OnInputUpdated();
        }

        private void stageUpdate()
        {
            if (sun) sun.UpdateSun();
        }

        private void DelayedUpdate()
        {
            // ������ ���۵��� ���, ���ӿ������� �ʾ��� ��쿡�� ����
            if (!isGameStart || isGameOver) return;

            playTime.UpdateTime();
            Debug.Log(playTime.Hour);

            /* 
             * �ð��� ��ȭ�� ���� ��� ��ȭ, �ϴ� ȸ��
            */
            UIManager.Instance.UpdateHealthBar(player.GetHpRatio());

            // �ð��� ������ ��쿡�� �����Ѵ�.
            if (lastSavedHour != playTime.Hour)
            {
                switch (playTime.Hour)
                {
                    case (int)GAMEMODE.MORNING:
                        //gameMode = GAMEMODE.MORNING;
                        //EndWave();
                        NextDay();

                        // ���� off
                        break;
                    case (int)GAMEMODE.NIGHT:
                        //gameMode = GAMEMODE.NIGHT;
                        if(spawner) StartCoroutine(StartWave());
                        //SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyStageMoon);
                        // ���� On
                        break;
                }
            }
            // ������ �ð� ����
            lastSavedHour = playTime.Hour;
        }
        public void DebugPlayer()
        {
            player.Init(startPoint.position);
        }


        public void StartRankMode()
        {
            StartCoroutine(MoveScene("99_Stage_Rank"));
            isRankStart = true;
        }

        public void SetMovement(bool value)
        {
            player.SetInputState(value);
        }

        // �������� ���� ����
        public void InitNewStage()
        {
            // �¾�, ��������, ������ ��������
            var sunObj = GameObject.FindWithTag("Sun"); 
            if (!sun && sunObj) sun = sunObj.GetComponent<DayAndNight>();
            var spawnerObj = GameObject.FindWithTag("Spawner");
            if (!spawner && spawnerObj) spawner = spawnerObj.GetComponent<Spawner>();
            if (!startPoint) startPoint = GameObject.FindWithTag("Start").transform;
            if (!stageGoal) stageGoal = GameObject.FindWithTag("End")?.GetComponent<StageArea>();

            //player = FindObjectOfType<Player>();
            //if(!player) player = LoadCharacter(playerPrefab).GetComponent<Player>();
            player = LoadCharacter(playerPrefab).GetComponent<Player>();
            if (startPoint && player)
            {
                // �÷��̾� ��ġ�� ������������ ����
                player.Init(startPoint.position);
                player.gameObject.SetActive(true);
            }

            // ��ũ��忡�� ������ ���ᰡ �߰�
            if (isRankStart)
            {
                var index = Random.Range(0,partnerPrefabs.Length);
                SpawnPartner(partnerPrefabs[index]);
            }
            //CurFatigue = maxFatigue;

            EquipManager.Instance.InitUpgrade();

            // ���Ӹ�� �ʱ�ȭ
            //gameMode = GAMEMODE.MORNING;
            // ���̺� �� �ʱ�ȭ.
            Wave = 0;
            score = 0;
            // �� ���� �� �ʱ�ȭ.
            enemyKilledCount = 0;
            spawnCount = 0;
            UIManager.Instance.UpdateWaveUI(Wave, spawnCount);
            // �÷��� ���� �ð� ����.
            playTime = new Timer();
            playTime.SetTimeScale(timeScale);
            // �¾� �ʱ�ȭ
            if(sun) sun.InitSun();

            // UIManager �ʱ�ȭ
            UIManager.Instance.InitUI();
            // ���� ��¥ UI ������ ����.
            UIManager.Instance.UpdateDateUI(playTime.Day);
            UIManager.Instance.UpdateScoreText(score);
            if (stageGoal) stageGoal.SetGoal();

            // ���� ���� ��ȣ Ȱ��ȭ.
            isGameOver = false;
            isGameStart = true;
            SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyStageSun);
            Mediator.Instance.Notify(this, GameEvent.REFRESH_STATUS, player);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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

        public void SpawnPartner(GameObject partnerPrefab)
        {
            // ��Ʈ�ʰ� ���� ���¸� ��Ʈ�� �ʱ�ȭ.
            if (partner)
            {
                if (partner.isDead) partner = null;
                else return;
            }
            // ��Ʈ�� ��ġ�� ����
            if (!partner) partner = LoadCharacter(partnerPrefab).GetComponent<PartnerAI>();
            if (partner)
            {
                partner.SetPartnerPosition(player.GetCharacterPosition()+ new Vector3(1, 0, 0));
                partner.gameObject.SetActive(true);
            }
        }

        public void EquipSkill(Skill skill)
        {
            if (skill != null && skill.skillInfo.skillType == SkillType.Passive && player.GetEquippedPassiveSkillCount() < MAX_EQUIPPED_PASSIVE_SKILLS)
            {
                PassiveSkill passiveSkill = skill as PassiveSkill;
                if (passiveSkill != null)
                {
                    player.EquipPassiveSkill(passiveSkill);
                }
            }
            else if (skill != null && skill.skillInfo.skillType == SkillType.Active && player.equippedActiveSkill == null)
            {
                ActiveSkill activeSkill = skill as ActiveSkill;
                if (activeSkill != null)
                {
                    player.EquipActiveSkill(activeSkill);
                    UIManager.Instance.EnableActiveSkill(activeSkill.skillInfo.skillImage);
                }
            }
        }
        public void UnEquipSkill(Skill skill)
        {
            if (skill != null && skill.skillInfo.skillType == SkillType.Passive)
            {
                PassiveSkill passiveSkill = skill as PassiveSkill;
                if (passiveSkill != null)
                {
                    player.UnequipPassiveSkill(passiveSkill);
                }
            }
            else if (skill != null && skill.skillInfo.skillType == SkillType.Active)
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
            // spawnCount�� �����ϰ� UI������ �����Ѵ�.
            UIManager.Instance.UpdateWaveUI(Wave, --spawnCount);

            // ��ȯ�� ���̺� ���� ��� óġ�Ͽ��� ��� ���̺� ����
            if (spawnCount <= 0)
            {
                StartCoroutine(EndWave());
            }
        }

        public IEnumerator StartWave()
        {

            // ���̺� ���� ���� ����
            UIManager.Instance.PlayWaveStartUI();

            /* ������ ���� �� ���� ��� */
            yield return null;

            // ���̺꿡 ���� �� ����
            Wave++;
            spawnCount += EnemySpawnCount;
            if (spawner)
            {
                StartCoroutine(SpawnRoutine());
                //spawner.SpawnEnemy(EnemySpawnCount, Wave);
            }
            // ���̺� UI Ȱ��ȭ
            UIManager.Instance.EnableWaveUI();
            UIManager.Instance.UpdateWaveUI(Wave, spawnCount);
            SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyStageMoon);
        }

        public IEnumerator SpawnRoutine()
        {
            int count = EnemySpawnCount;
            while (count > 0)
            {
                if (spawnCount - EnemySpawnCount + count > 20)
                {
                    Debug.Log("Spawn Full");
                    yield return null;
                }
                else
                {
                    spawner.SpawnEnemy(1, Wave);
                    count--;
                }
            }
        }

        public IEnumerator EndWave()
        {
            // ���̺� Ŭ���� ���� ����
            UIManager.Instance.PlayWaveClearUI();
            // ���̺� UI ��Ȱ��ȭ
            UIManager.Instance.DisableWaveUI();
            SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyStageSun);
            spawnCount = 0;
            // ������ ���� �� ���� ���
            yield return null;
        }

        public void NextDay()
        {
            // ���� �� ���� ����
            UIManager.Instance.PlayNextDayUI(playTime.Day);
            UIManager.Instance.UpdateDateUI(playTime.Day);
        }

        // ������ �ð����� ���(����)
        public IEnumerator SleepHours(int hours)
        {
            for (int repeat = 0; repeat < hours; repeat++)
            {
                // UIManager.Instance.DisplaySleepUI(hours);
                yield return new WaitForSeconds(sleepWaitPeriod);
                // �� �������� 1�ð��� �߰�
                playTime.AddTime(0, 1, 0, 0);
                // ����� �ð� �� �Ƿ� ��ġ ����. �ּ�ġ 0
                //CurFatigue = Mathf.Max(0.0f, CurFatigue - recoverFatiguePerHour);
            }
        }

        public void KillEnemy(bool isWaveEnemy)
        {
            AddScore(100); // enemy óġ ��, 100 score ���.
            enemyKilledCount++;
            CheckKillAchievements();
            if (isWaveEnemy) DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
        }

        public void CheckKillAchievements()
        {
            if (enemyKilledCount >= 1000)
            {
                Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, Achievement.ZombieAnnihilator);
            }
            else if (enemyKilledCount >= 100)
            {
                Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, Achievement.ZombieExterminator);
            }
            else if (enemyKilledCount >= 10)
            {
                Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, Achievement.ZombieSlayer);
            }
        }

        public void SetPause(bool value)
        {
            if (isGameStart)
            {
                Cursor.visible = value;
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            }
            Time.timeScale = value ? 0 : 1;
        }

        public void StageClear()
        {
            player.OnGodMode();
            player.SetInputState(false);
            clearTime = playTime;
            isGameStart = false;
            UIManager.Instance.PlayStageClearUI(playTime, score);
            SoundManager.Instance.OnPlaySFX("Get_AbsoluteRing");
            SoundManager.Instance.StopBGM();
        }

        public void GameOver()
        {
            if (isGameOver) return;
            player.OnGodMode();
            player.SetInputState(false);
            isGameOver = true;
            isGameStart = false;
            var count = 0;
            if (isOnNFT)
            {
                foreach (BlockChain.Item item in NFTManager.Instance.GetMyItems())
                {
                    if (item.nftType[0].Equals("R"))
                    {
                        count++;
                    }
                }
            }
            score += count * 1000;

            if (score > highScore)
            {
                if (isOnNFT)
                {
                    if (NFTManager.Instance.GetWinner() < score)
                    {
                        NFTManager.Instance.newWinner(score);
                        isOwner = true;
                    }
                }
                highScore = score;
                // 스테이지 클리어 기록
                var mode = isRankStart ? "Rank" : "Story";
                NFTManager.Instance.RecordClearResult(mode, score, playTime);
            }

            UIManager.Instance.EnableGameOverUI();
            SoundManager.Instance.OnPlaySFX("GameOver");
        }


        public void AddScore(int value)
        {
            score += value;
            // UI�� ScoreText�� ����.
            UIManager.Instance.UpdateScoreText(score);
        }
        public void RestartGame()
        {
            StartCoroutine(RestartScene());
        }

        public IEnumerator RestartScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);

            yield return null;
            // �� �ε尡 �Ϸ�� ������ ���
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // �ε� �Ϸ� �� ������ �ڵ�
            InitNewStage();
            UIManager.Instance.RestartGame();
            UIManager.Instance.InitUI();
            Mediator.Instance.Notify(this, GameEvent.RESTART, null);
        }

        public void MextScene()
        {
            if(isRankStart){
/*                if (score > highScore)
                {
                    if (isOnNFT)
                    {
                        if (NFTManager.Instance.GetWinner() < score)
                        {
                            NFTManager.Instance.newWinner(score);
                            isOwner = true;
                        }
                    }
                    highScore = score;
                    // 스테이지 클리어 기록
                    NFTManager.Instance.RecordClearResult("Rank", score, playTime);
                }*/
                ToMain();
                
                return;
            }
            // �� ���� ��Ģ 1: ���ڸ� �� ���ڴ� 00~99 �� �ڸ� ���ڷ� �����ؾ��Ѵ�.
            // �� ���� ��Ģ 2: �ΰ���(��������) �� ��쿡�� �� �̸��� �ݵ�� Stage�� ���ԵǾ�� �Ѵ�.
            // ���� ���� ���ڸ� �����Ͽ� ���� ������ �̵��Ѵ�.
            if (int.TryParse(SceneManager.GetActiveScene().name.Substring(0, 2), out int currSceneIdx))
            {
                if (currSceneIdx + 1 == sceneList.Count)
                    ToMain();
                else
                    StartCoroutine(MoveScene(currSceneIdx + 1));
            }

        }

        private bool isOwner;
        public void ToMain()
        {
            //highScore = (score > highScore) ? score : highScore;
            if(score > highScore)
            {
                if (isOnNFT)
                {
                    if (NFTManager.Instance.GetWinner() < score)
                    {
                        NFTManager.Instance.newWinner(score);
                        isOwner = true;
                    }
                    // 스테이지 클리어 기록
                    var mode = isRankStart ? "Rank" : "Story";
                    NFTManager.Instance.RecordClearResult(mode, score, playTime);
                }
                highScore = score;
            }

            if (isOwner)
            {
                UIManager.Instance.ClearCanvas();
                StartCoroutine(MoveScene("GetRing"));
            }
            else {
                StartCoroutine(MoveScene(0));
            }
            isRankStart = false;
            isOwner = false;
            SetPause(false);
            ResetGame();
            SoundManager.Instance.OnPlayBGM(SoundManager.Instance.keyMain);
            if(isOnNFT) NFTManager.Instance.RefreshNFT();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public IEnumerator MoveScene(int sceneIndex)
        {
            UIManager.Instance.ClearPopup();
            SceneManager.LoadScene(sceneList[sceneIndex]);
            var async = SceneManager.LoadSceneAsync(sceneList[sceneIndex]);

            // �� �̵��� ���� �� ���� ���
            while (!async.isDone)
            {
                yield return null;
            }

            // �̵��� ���� Main �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Main"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Main);
            }
            // �̵��� ���� Stage �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Stage"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Stage);
                InitNewStage();
            }
            // �̵��� ���� Cinema �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Cinema"))
            {
                SoundManager.Instance.StopBGM();
                UIManager.Instance.SwitchCanvas(CavasIndex.Cinema);
            }
        }

        public IEnumerator MoveScene(string sceneName)
        {
            UIManager.Instance.ClearPopup();
            SceneManager.LoadScene(sceneName);
            var async = SceneManager.LoadSceneAsync(sceneName);

            // �� �̵��� ���� �� ���� ���
            while (!async.isDone)
            {
                yield return null;
            }

            // �̵��� ���� Main �� �������� �ʱ�ȭ
            if (sceneName.Contains("Main"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Main);
            }
            // �̵��� ���� Stage �� �������� �ʱ�ȭ
            if (sceneName.Contains("Stage"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Stage);
                InitNewStage();
            }
            // �̵��� ���� Cinema �� �������� �ʱ�ȭ
            if (sceneName.Contains("Cinema"))
            {
                SoundManager.Instance.StopBGM();
                UIManager.Instance.SwitchCanvas(CavasIndex.Cinema);
            }
        }

        public void LoadScene()
        {
            int sceneIndex = 1;
            /* ����� �ε���(�� ��ġ) �ҷ����� */
            StartCoroutine(MoveScene(sceneIndex));
        }

        public void ResetGame()
        {
            // ���� ���� �ʱ�ȭ
            isGameOver = false;
            isGameStart = false;
            isRankStart = false;
            enemyKilledCount = 0;
            spawnCount = 0;
            Wave = 0;
            score = 0;

            // �÷��̾� �ʱ�ȭ
            if (player)
            {
                player = null;
            }

            // ���� �ʱ�ȭ
            if (partner)
            {
                partner = null;
            }
            // �¾� �ʱ�ȭ
            if (sun)
            {
                sun = null;
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}