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
    }
    #endregion

    [SerializeField] private GameObject player;

    [SerializeField] private float maxFatigue = 100f;   // 최대 피로도
    public float curFatigue { get; private set; }   // 현재 피로도
    public int wave { get; private set; } = 0;  // 현재 Wave 카운트.
    [SerializeField] private int maxWave = 10;  // 최대 Wave 카운트.
    // 이번 Wave에 출현할 Enemy의 수.
    public int enemySpawnCount { get { return Mathf.RoundToInt(wave * 1.5f); } }

    [SerializeField] private int spwanCount = 0; // 필드에 존재하는 Enemy의 수.
    public void DecreaseSpawnCount()
    {
        // spawnCount를 감소하고 UI정보를 갱신한다.
        UIMgr2.Instance.UpdateWaveText(wave, --spwanCount);
    }

    private int score = 0;

    // Start is called before the first frame update
    private void Start()
    {
        curFatigue = maxFatigue;
        // 현재 wave UI 정보를 갱신.
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
        // UI의 ScoreText를 갱신.
        UIMgr2.Instance.UpdateScoreText(score);
    }

    public void ResetGameMgr()
    {
        curFatigue = maxFatigue;
    }
}
