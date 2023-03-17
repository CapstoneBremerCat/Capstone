using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game;
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
        private PlayerInput playerInput;

        [SerializeField] private GameObject partnerPrefab;     // ��Ʈ�� ������
        [SerializeField] private PartnerAI partner;     // ��Ʈ��
        [SerializeField] private Spawner spawner; // ������
        [SerializeField] private StageUIController stageUI; // �������� UI

        [SerializeField] private Transform startPoint; // �������� ���� ����

        [Header("Game")]
        [SerializeField] private int maxWave = 10;  // �ִ� Wave ī��Ʈ.
        public int Wave { get; private set; }  // ���� Wave ī��Ʈ.
                                               // �̹� Wave�� ������ Enemy�� ��.
        public int EnemySpawnCount { get { return Mathf.RoundToInt(10.0f + Wave * 2.0f); } }

        private int spawnCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.

        [SerializeField] private float maxFatigue = 100f;   // �ִ� �Ƿε�
        [SerializeField] private float recoverFatiguePerHour = 10f;   // ���� �ð��� ȸ���ϴ� �Ƿε� ��ġ
        public float CurFatigue { get; private set; }   // ���� �Ƿε�

        [SerializeField] private GAMEMODE gameMode;
        private int score = 0;

        private Timer playTime;
        [SerializeField] private float timeScale;
        private int lastSavedHour;   // ���������� ����� �ð�

        private const float sleepWaitPeriod = 0.5f;    // ���� �� �� �ð��� ���ϴ� ����

        public bool isGameOver { get; private set; }    // ���ӿ��� ����
        public bool isGameStart { get; private set; }    // ���ӽ��� ����

        // Start is called before the first frame update
        private void Start()
        {
            isGameStart = false;
            isGameOver = false;

            if (player) player.gameObject.SetActive(false);
            if (partner) partner.gameObject.SetActive(false);

            //InitNewStage();

        }

        // ���� ���� �ֱ⿡ ���� ȸ��, �̵� ����.  
        private void FixedUpdate()
        {
            // ������ ���۵��� ���, ���ӿ������� �ʾ��� ��쿡�� ����
            if (!isGameStart || isGameOver) return;
            if (player) player.UpdateMovement();
        }

        // Update is called once per frame
        private void Update()
        {
            // ������ ���۵��� ���, ���ӿ������� �ʾ��� ��쿡�� ����
            if (isGameStart && !isGameOver) stageUpdate();

        }

        private void stageUpdate()
        {
            if (player) player.UpdateAttack();
            if (sun) sun.UpdateSun();
            if (playerInput.skillSlot1)
            {
/*                SkillData SkillData = partner.GetPartnerSkill();
                UseSkill(SkillData);*/
            }
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

        // �������� ���� ����
        public void InitNewStage()
        {
            // �¾�, ��������, ������ ��������
            sun = GameObject.FindWithTag("Sun").GetComponent<DayAndNight>();
            startPoint = GameObject.FindWithTag("Start").transform;
            spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
            stageUI = StageUIController.Instance;

            player = LoadCharacter(playerPrefab).GetComponent<Player>();
            if (startPoint && player)
            {
                // �÷��̾� ��ġ�� ������������ ����
                player.transform.position = startPoint.position;
                player.gameObject.SetActive(true);
                player.Init();
                playerInput = player.GetPlayerInput();
                // ��Ʈ�� ��ġ�� ����
                partner = LoadCharacter(partnerPrefab).GetComponent<PartnerAI>();
                if (partner)
                {
                    partner.transform.position = startPoint.position + new Vector3(3, 0, 0);
                    partner.gameObject.SetActive(true);
                }
            }

            //CurFatigue = maxFatigue;

            // ���Ӹ�� �ʱ�ȭ
            gameMode = GAMEMODE.MORNING;
            // ���̺� �� �ʱ�ȭ.
            Wave = 0;
            // �÷��� ���� �ð� ����.
            playTime = new Timer();
            playTime.SetTimeScale(timeScale);
            // �¾� �ʱ�ȭ
            sun.InitSun();

            // UI �ʱ�ȭ
            if (stageUI)
            {
                stageUI.Init();
                // ���� ��¥ UI ������ ����.
                stageUI.UpdateDateText(playTime.Day);
                // ���̺� UI ��Ȱ��ȭ
                stageUI.DisableWaveText();
                //stageUI.DisplayCooltime(10);

                stageUI.RestartEvent += () =>
                {
                    InitNewStage();
                    stageUI.SetHealthBar(player.GetHpRatio());
                    stageUI.SetStaminaBar(player.GetStaminaRatio());
                };
            }

            // ���� ���� ��ȣ Ȱ��ȭ.
            isGameStart = true;

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
        public void UseSkill(ActiveSkill activeSkill)
        {
            if (stageUI.DisplayCooltime(activeSkill.cooldown))
            {
                /*��ų ���*/
                Debug.Log("Use SkillData");
            }
        }

        public void DecreaseSpawnCount()
        {
            // spawnCount�� �����ϰ� UI������ �����Ѵ�.
            if (stageUI) stageUI.UpdateWaveText(Wave, --spawnCount);

            // ��ȯ�� ���̺� ���� ��� óġ�Ͽ��� ��� ���̺� ����
            if (spawnCount <= 0)
            {
                StartCoroutine(EndWave());
            }
        }

        public IEnumerator StartWave()
        {
            if (stageUI)
            {
                // ���̺� ���� ���� ����
                if (stageUI) stageUI.WaveStart();
                // ���̺� UI Ȱ��ȭ
                stageUI.EnableWaveText();
                stageUI.UpdateWaveText(Wave, spawnCount);
            }

            /* ������ ���� �� ���� ��� */
            yield return null;

            // ���̺꿡 ���� �� ����
            Wave++;
            spawnCount += EnemySpawnCount;
            spawner.SpawnEnemy(spawnCount);

            //NextWave();
        }

        public IEnumerator EndWave()
        {
            if (stageUI)
            {
                // ���̺� Ŭ���� ���� ����
                stageUI.WaveClear();
                // ���̺� UI ��Ȱ��ȭ
                stageUI.DisableWaveText();
            }

            // ������ ���� �� ���� ���
            yield return null;
        }

        public void NextDay()
        {
            if (stageUI)
            {
                // ���� �� ���� ����
                stageUI.DisplayNextDay(playTime.Day);
                stageUI.UpdateDateText(playTime.Day);
            }
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

        public void KillEnemy(bool isWaveEnemy)
        {
            AddScore(100); // enemy óġ ��, 100 score ���.
            if (isWaveEnemy) DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
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
            if (stageUI) stageUI.StageClear();
        }

        public void GameOver()
        {
            isGameOver = true;
            isGameStart = false;
            if (stageUI) stageUI.GameOver();
        }

        public void RestartGame()
        {
            isGameStart = true;
        }

        public void AddScore(int value)
        {
            score += value;
            // UI�� ScoreText�� ����.
            if (stageUI) stageUI.UpdateScoreText(score);
        }

        public void MextScene()
        {
            // �� ��� ��Ģ 1: ���ڸ� �� ���ڴ� 00~99 �� �ڸ� ���ڷ� �����ؾ��Ѵ�.
            // �� ��� ��Ģ 2: �ΰ���(��������) �� ��쿡�� �� �̸��� �ݵ�� Stage�� ���ԵǾ�� �Ѵ�.
            // ���� ���� ���ڸ� �����Ͽ� ���� ������ �̵��Ѵ�.
            if (int.TryParse(SceneManager.GetActiveScene().name.Substring(0, 2), out int currSceneIdx))
                StartCoroutine(MoveScene(currSceneIdx + 1));
        }

        public IEnumerator MoveScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneList[sceneIndex]);
            var async = SceneManager.LoadSceneAsync(sceneList[sceneIndex]);

            // �� �̵��� ���� �� ���� ���
            while (!async.isDone)
            {
                yield return null;
            }

            // �̵��� ���� Stage �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Stage"))
            {
                InitNewStage();
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
            CurFatigue = maxFatigue;
            spawnCount = 0;
            Wave = 0;
        }
    }
}