using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMgr : MonoBehaviour
{
    #region instance
    private static ItemMgr instance = null;
    public static ItemMgr Instance { get { return instance; } }

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


    public StatusData GetTotalItemStats(List<int> itemList)
    {
        StatusData statusData = new StatusData();
        for(int i=0;i<itemList.Count;i++)
        {
            int code = DataMgr.Instance.GetItemData(itemList[i]).StatCode;
            StatusData stat = DataMgr.Instance.GetItemStat(code);

            statusData.AddStat(stat);
        }
        return statusData;
    }


}
