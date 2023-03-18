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

        private StatusObject[] itemStatArr;
        private Dictionary<int, ItemData> itemDataMap = new Dictionary<int, ItemData>();
        private Dictionary<int, StatusObject> itemStatMap = new Dictionary<int, StatusObject>();

        // �ΰ��� ������ �����ε�.. ��ĥ ����� ���°ɱ� ��
        private void LoadItemData()
        {
            List<ItemData> itemDataList = XML<ItemData>.Read("ItemTable");
            for (int i = 0; i < itemDataList.Count; i++)
            {
                itemDataMap.Add(itemDataList[i].ItemCode, itemDataList[i]);
            }

            itemStatArr = Resources.LoadAll<StatusObject>("ScriptableObjects/NFT_Passive");
            for (int i = 0; i < itemStatArr.Length; i++)
            {
                itemStatMap.Add(itemStatArr[i].id, itemStatArr[i]);
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