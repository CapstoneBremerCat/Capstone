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
        //DontDestroyOnLoad(this.gameObject);
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

    // The item pool to search in
    [SerializeField] private List<Item> itemList;
    [SerializeField] private List<GameObject> itemPool = new List<GameObject>();  // 생성된 아이템들을 담는 리스트

    // Find an item identical to the given item in the item pool
    public bool FindIdenticalItem(Item poolItem)
    {
        // Loop through each item in the item pool
/*        foreach (Item item in itemList)
        {
            // Check if the item names and types match
            if (poolItem.itemName == item.itemName && poolItem.itemType == item.itemType)
            {
                return true;
            }
        }*/
        return false;
    }

    private GameObject CreateItem(GameObject itemPrefab)
    {
        // 풀 안에 비활성화된 오브젝트가 있으면 재활용
/*        foreach (GameObject item in itemPool)
        {
            var found = FindIdenticalItem(item.GetComponent<Item>());
            if (found)
            {
                item.SetActive(true);
                return item;
            }
        }*/
        // If all objects in the pool are active, create a new one
        var newItem = Instantiate(itemPrefab);
        itemPool.Add(newItem);
        return newItem;
    }

    public GameObject GetRandomItemPrefab()
    {
        return itemPool[Random.Range(0, itemPool.Count)];
    }

    public bool SpawnItem(Vector3 position)
    {
        var item = CreateItem(GetRandomItemPrefab());
        if (!item) return false;
        item.transform.position = position;
        return true;
    }
}
