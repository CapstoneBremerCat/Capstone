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
