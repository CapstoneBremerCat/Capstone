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

    // Start is called before the first frame update
    private void Start()
    {
        curFatigue = maxFatigue;
    }

    // Update is called once per frame
    private void Update()
    {
        //playerStatus.Updates();

    }

    public void ResetGameMgr()
    {
        curFatigue = maxFatigue;
    }
}
