using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class DataMgr : MonoBehaviour
    {
        #region instance
        private static DataMgr instance = null;
        public static DataMgr Instance { get { return instance; } }

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

        private StatusObject[] itemStatArr;
        private Dictionary<int, ItemData> itemDataMap = new Dictionary<int, ItemData>();
        private Dictionary<int, StatusObject> itemStatMap = new Dictionary<int, StatusObject>();

        // 두개가 동일한 로직인데.. 합칠 방법은 없는걸까 ㅜ
        private void LoadItemData()
        {
            List<ItemData> itemDataList = XML<ItemData>.Read("ItemTable");
            for (int i = 0; i < itemDataList.Count; i++)
            {
                itemDataMap.Add(itemDataList[i].ItemCode, itemDataList[i]);
            }

            itemStatArr = Resources.LoadAll<StatusObject>("ScriptableObjects/ItemStats");
            for (int i = 0; i < itemStatArr.Length; i++)
            {
                itemStatMap.Add(itemStatArr[i].idCode, itemStatArr[i]);
            }
        }

        public ItemData GetItemData(int itemCode)
        {
            return itemDataMap[itemCode];
        }

        public StatusData GetItemStat(int statCode)
        {
            return itemStatMap[statCode].status;
        }

    }
}