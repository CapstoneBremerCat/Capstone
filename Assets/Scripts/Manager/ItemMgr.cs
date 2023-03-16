using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ItemMgr : MonoBehaviour
    {
        #region instance
        private static ItemMgr instance = null;
        public static ItemMgr Instance { get { return instance; } }

        private void Awake()
        {
            // Check if the scene already contains an instance of this object
            // If so, destroy this object and return
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // If not, assign this object to the instance variable
            instance = this;

            // Don't destroy this object when changing scenes
            //DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        // The item pool to search in
        [SerializeField] private List<Item> itemList;
        [SerializeField] private List<GameObject> itemObjList = new List<GameObject>();  // The list of item prefabs
        [SerializeField] private List<GameObject> itemPool = new List<GameObject>();  // The list of item objects in the pool

        [SerializeField] private Weapon[] weapons;  // an arrangement with all kinds of weapons as elements

        // Use a Dictionary data structure to provide easy access to weapons by name from a management perspective.
        private Dictionary<string, Weapon> weaponDictionary = new Dictionary<string, Weapon>();


        void Start()
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                weaponDictionary.Add(weapons[i].WeaponName, weapons[i]);
            }
        }

        public Weapon GetWeaponByName(string weaponName)
        {
            return weaponDictionary[weaponName];
        }

        // Get the total stats of a list of items
        public StatusData GetTotalItemStats(List<int> itemList)
        {
            StatusData statusData = new StatusData();
            for (int i = 0; i < itemList.Count; i++)
            {
                int code = DataMgr.Instance.GetItemData(itemList[i]).StatCode;
                StatusData stat = DataMgr.Instance.GetItemStat(code);

                statusData.AddStat(stat);
            }
            return statusData;
        }


        // Find an item identical to the given item in the item pool
        public bool FindIdenticalItem(Item poolItem)
        {
            // Loop through each item in the item pool
            foreach (Item item in itemList)
            {
                // Check if the item names and types match
                if (poolItem.name == item.name && poolItem.itemType == item.itemType)
                {
                    return true;
                }
            }
            return false;
        }

        private GameObject CreateItem(GameObject itemPrefab)
        {
            // Find a usable item object in the item pool
            /*        foreach (GameObject item in itemPool)
                    {
                        var found = FindIdenticalItem(item.GetComponent<ItemPickUp>().Item);
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

        // Get a random item prefab
        public GameObject GetRandomItemPrefab()
        {
            return itemObjList[Random.Range(0, itemObjList.Count)];
        }

        // Spawn an item
        public bool SpawnItem(Vector3 position)
        {
            var item = CreateItem(GetRandomItemPrefab());
            if (!item) return false;
            item.transform.position = position;
            return true;
        }
    }
}