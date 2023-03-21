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

        [SerializeField] private GameObject partnerPrefab;     // ��Ʈ�� ������
        [SerializeField] private PartnerAI partner;     // ��Ʈ��
        [SerializeField] private Spawner spawner; // ������

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
        private const int MAX_EQUIPPED_PASSIVE_SKILLS = 4;
        public bool isGameOver { get; private set; }    // ���ӿ��� ����
        public bool isGameStart { get; private set; }    // ���ӽ��� ����

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
        public void DebugPlayer()
        {
            player.Init(startPoint.position);
        }
        // �������� ���� ����
        public void InitNewStage()
        {
            // �¾�, ��������, ������ ��������
            sun = GameObject.FindWithTag("Sun").GetComponent<DayAndNight>();
            startPoint = GameObject.FindWithTag("Start").transform;
            spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

            player = FindObjectOfType<Player>();
            if(!player) player = LoadCharacter(playerPrefab).GetComponent<Player>();
            if (startPoint && player)
            {
                // �÷��̾� ��ġ�� ������������ ����
                //player.transform.position = startPoint.position;
                player.gameObject.SetActive(true);
                player.Init(startPoint.position);
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

            // ���� ��¥ UI ������ ����.
            UIManager.Instance.UpdateDateUI(playTime.Day);
            // ���̺� UI ��Ȱ��ȭ
            UIManager.Instance.DisableWaveUI();

            UIManager.Instance.RestartEvent += () =>
            {
                UIManager.Instance.UpdateHealthBar(player.GetHpRatio());
                UIManager.Instance.UpdateStaminaBar(player.GetStaminaRatio());
            };
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
            // ���̺� UI Ȱ��ȭ
            UIManager.Instance.EnableWaveUI();
            UIManager.Instance.UpdateWaveUI(Wave, --spawnCount);

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
            // ���̺� Ŭ���� ���� ����
            UIManager.Instance.PlayWaveClearUI();
            // ���̺� UI ��Ȱ��ȭ
            UIManager.Instance.DisableWaveUI();
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
            UIManager.Instance.PlayStageClearUI();
        }

        public void GameOver()
        {
            isGameOver = true;
            isGameStart = false;
            UIManager.Instance.EnableGameOverUI();
        }

        public void RestartGame()
        {
            InitNewStage();
            Mediator.Instance.Notify(this, GameEvent.RESTART, null) ;
        }

        public void AddScore(int value)
        {
            score += value;
            // UI�� ScoreText�� ����.
            UIManager.Instance.UpdateScoreText(score);
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

            // �̵��� ���� Main �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Main"))
            {
                UIManager.Instance.SwitchCanvas(CavasIndex.Main);
            }
            // �̵��� ���� Stage �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Stage"))
            {
                InitNewStage();
                UIManager.Instance.SwitchCanvas(CavasIndex.Stage);
            }
            // �̵��� ���� Cinema �� �������� �ʱ�ȭ
            if (sceneList[sceneIndex].Contains("Cinema"))
            {
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
            CurFatigue = maxFatigue;
            spawnCount = 0;
            Wave = 0;
        }
    }
}