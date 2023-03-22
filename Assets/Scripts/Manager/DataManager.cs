using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Game;
namespace Game
{
    public class DataManager : MonoBehaviour
    {
        #region instance
        private static DataManager instance = null;
        public static DataManager Instance { get { return instance; } }

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
        private Dictionary<int, StatusObject> itemStatMap = new Dictionary<int, StatusObject>();

        // �ΰ��� ������ �����ε�.. ��ĥ ����� ���°ɱ� ��
        private void LoadItemData()
        {
            itemStatArr = Resources.LoadAll<StatusObject>("ScriptableObjects/NFT_Passive");
            for (int i = 0; i < itemStatArr.Length; i++)
            {
                itemStatMap.Add(itemStatArr[i].id, itemStatArr[i]);
            }
        }

        public void SaveEquipmentsById(List<int> equipments)
        {
            if (equipments == null) return;
            BinaryFormatter formatter = new BinaryFormatter();
            string savePath = Application.persistentDataPath + "/equipments.sav";
            FileStream stream = new FileStream(savePath, FileMode.Create);

            formatter.Serialize(stream, equipments);
            stream.Close();
        }

        public List<int> LoadEquipmentsById()
        {
            string savePath = Application.persistentDataPath + "/equipments.sav";

            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(savePath, FileMode.Open);

                List<int> equipments = formatter.Deserialize(stream) as List<int>;
                stream.Close();

                return equipments;
            }
            else
            {
                Debug.Log("Save file not found in " + savePath);
                return null;
            }
        }

        public StatusData GetItemStat(int statCode)
        {
            return itemStatMap[statCode].status;
        }

    }
}