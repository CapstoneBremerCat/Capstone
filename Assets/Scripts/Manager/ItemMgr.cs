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
