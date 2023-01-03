using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    #region instance
    private static GameMgr instance = null;
    public static GameMgr Instance { get { return instance; } }

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
    }
    #endregion

    [SerializeField] private GameObject player;

    [SerializeField] private float maxFatigue = 100f;   // �ִ� �Ƿε�
    public float curFatigue { get; private set; }   // ���� �Ƿε�
    public int wave { get; private set; } = 0;  // ���� Wave ī��Ʈ.
    [SerializeField] private int maxWave = 10;  // �ִ� Wave ī��Ʈ.
    // �̹� Wave�� ������ Enemy�� ��.
    public int enemySpawnCount { get { return Mathf.RoundToInt(wave * 1.5f); } }

    [SerializeField] private int spwanCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.
    public void DecreaseSpawnCount()
    {
        // spawnCount�� �����ϰ� UI������ �����Ѵ�.
        UIMgr2.Instance.UpdateWaveText(wave, --spwanCount);
    }

    private int score = 0;

    // Start is called before the first frame update
    private void Start()
    {
        curFatigue = maxFatigue;
        // ���� wave UI ������ ����.
        UIMgr2.Instance.UpdateWaveText(GameMgr.instance.wave, spwanCount);
    }

    // Update is called once per frame
    private void Update()
    {
        //playerStatus.Updates();

    }
    public bool NextWave()
    {
        if (maxWave > wave)
        {
            wave++;
            return true;
        }
        return false;
    }
    public void AddScore(int value)
    {
        score += value;
        // UI�� ScoreText�� ����.
        UIMgr2.Instance.UpdateScoreText(score);
    }

    public void ResetGameMgr()
    {
        curFatigue = maxFatigue;
    }
}
