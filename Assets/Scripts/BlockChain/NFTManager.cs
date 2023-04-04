using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;	// UnityWebRequest����� ���ؼ� �����ش�.
using Newtonsoft.Json;
using TMPro;
using BlockChain;
using Game;
namespace BlockChain
{
    public class Item
    {
        public int tokenId;
        public string tokenURI;
        public string name;
        public string description;
        public int nftType;
        public float price;
        public bool isSelling;
        public Sprite image;
    }
    public class packet
    {
        public int errorno;
    }

    public class res_upload : packet
    {
        public string code;
        public string message;
    }

    public class UpdateNFT_req_upload
    {
        public string addr; // totalsupply, ownedTokens
    }
    // ���õ� nft �� ������ �����ϰ� �ִ� nft����Ʈ�� ��ƴٰ� ����Ʈ���̿� ����;
    public class UpdateNFT_res_upload : packet
    {
        public string code;
        public string message;
        public int totalSupply;
        public int[] ownedTokens;
        public float balanceOfKlay;
    }
    public class getNFT_req_upload
    {
        public int tokenId;
    }
    // �޾ƿͼ� json�� ���� ����
    public class getNFT_res_upload : packet
    {
        public string code;
        public string message;
        public string tokenURI;
        public string name;
        public string description;
        public int nftType;
        public float price;
        public bool isSelling;
    }
    public class JsonCID
    {
        public string name;
        public string description;
        public string image;

    }
    public class sellNFT_req_upload
    {
        public int tokenId;
        public float price;
    }
    // �޾ƿͼ� json�� ���� ����

    public class reward_req_upload
    {
        public string name;
    }

    public class reward_res_upload : packet
    {
        public string code;
        public string message;
        public string cid;
    }

    public class mint_req_upload
    {
        public string addr;
        public string ipfs;
        public string name;
        public string description;
        public string nftType;

    }
    public class UpdateRing_req_upload
    {
        public string addr;
        public int score;
    }
    public class getRing_req_upload
    {
        public string message;
    }
    public class getRing_res_upload : packet
    {
        public string code;
        public string message;
        public string ringOwner;
        public int ringScore;
    }

    [System.Serializable]
    public class SaveData
    {
        public SaveData(string _name, string _description, string _image)
        {
            name = _name;
            description = _description;
            image = _image;
        }

        public string name;
        public string description;
        public string image;
    }
    public static class SaveSystem
    {
        private static string SavePath => Application.persistentDataPath + "/saves/";

        public static void Save(SaveData saveData, string saveFileName)
        {
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }

            string saveJson = JsonUtility.ToJson(saveData);

            string saveFilePath = SavePath + saveFileName + ".json";
            File.WriteAllText(saveFilePath, saveJson);
            Debug.Log("Save Success: " + saveFilePath);
        }

        public static SaveData Load(string saveFileName)
        {
            string saveFilePath = SavePath + saveFileName + ".json";

            if (!File.Exists(saveFilePath))
            {
                Debug.LogError("No such saveFile exists");
                return null;
            }

            string saveFile = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(saveFile);
            return saveData;
        }
    }
    public class NFTManager : MonoBehaviour
    {
        #region instance
        private static NFTManager instance = null;
        public static NFTManager Instance { get { return instance; } }

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

            StartCoroutine(FirstLoadItems());
        }
        #endregion
        private float _balanceOfKlay;
        private List<Item> items = new List<Item>();    // ��� �����۵��� �����ϴ� �迭
        private List<Item> notSellingItems = new List<Item>(); // isSelling�� false�� ��� �����۵��� �����ϴ� �迭
        private List<SkillInfo> skillInfoList = new List<SkillInfo>();
        private Dictionary<int, SkillInfo> skillInfoDictionary = new Dictionary<int, SkillInfo>();
        private bool isLoaded = false; // ��� �������� �ε��Ǿ����� ���θ� ������ ����
        public float balanceOfKlay { get { return _balanceOfKlay; } }

        [SerializeField] private RankController rankController;
        private IEnumerator FirstLoadItems()
        {
            Debug.Log("FirstLoadItems Start");
            yield return StartCoroutine(LoadTotalSupply());

            var _totalSupply = TotalSupply.GetTotalSupply();

            // totalSupply �޾ƿ����� �̰ɷ� ���� for�� ������ ������ �޾ƿ�.
            for (int i = 1; i <= _totalSupply; i++)
            {
                yield return StartCoroutine(LoadNFT(i));
            }
            GetWinner();
            // ���� �ڵ� ����
            // ��� �������� �ε��Ǿ��ٴ� �÷��׸� true�� ����
            yield return new WaitForSeconds(8.0f);
            isLoaded = true;
            LoadNFTSkillInfos();
            SkillManager.Instance.LoadOwnedSkills(GetOwnedNFTSkillInfos());
        }

        IEnumerator LoadTotalSupply()
        {
            // totalSupply�� ownedTokens�� post�� �޾ƿͼ� �������� ������.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            //UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            UpdateNFTrequpload.addr = "";
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // ����� ��ٸ��� yield return �߰�
            yield return StartCoroutine(Upload("http://localhost:5000/users-data", json, (result) =>
            {
                var updateNFTresponseResult = JsonConvert.DeserializeObject<UpdateNFT_res_upload>(result);
                //Debug.Log("result: " + updateNFTresponseResult);
                Debug.Log("totalSupply : " + updateNFTresponseResult.totalSupply);
                //Debug.Log("ownedTokens : " + updateNFTresponseResult.ownedTokens);

                TotalSupply.SetTotalSupply(updateNFTresponseResult.totalSupply);
                OwnedTokens.SetOwnedTokens(updateNFTresponseResult.ownedTokens);
                BalanceOfKlay.SetBalanceOfKlay(updateNFTresponseResult.balanceOfKlay);
                Debug.Log("BalanceOfKlay : " + updateNFTresponseResult.balanceOfKlay);
                Debug.Log(BalanceOfKlay.GetBalanceOfKlay());

                _balanceOfKlay = updateNFTresponseResult.balanceOfKlay;

            }));
            //Debug.Log("LoadTotalSupply");
        }
        async public void mintRewardNFT(string _name, string _description)
        {
            var name = _name + "-" + LoginManager.Instance.GetAddr();
            var description = _description;
            //var name = "rewardNFTName";
            //var description = "rewardNFTDescription";

            // 1. �̹��� ��ũ���� �� ipfs ���ε� cid ��ȯ 

            byte[] _imageBytes = Screenshot.TakeScreenshot_(1920, 1080, name);

            var reqUploadImage = new reward_req_upload();
            reqUploadImage.name = "imgfile";
            var json = JsonConvert.SerializeObject(reqUploadImage);

            StartCoroutine(ImageUpload("http://localhost:5000/ipfs-image", _imageBytes, json, name, (result) =>
            {
                var responseResult = JsonConvert.DeserializeObject<reward_res_upload>(result);
                Debug.Log("result: " + responseResult);
                Debug.Log("image cid : " + responseResult.cid);

                // 2. �̹��� cid�� �̿��Ͽ� metadata.json ������ �� cid �޾ƿ���
                SaveData metaData = new SaveData(name, description, "ipfs://" + responseResult.cid);

                SaveSystem.Save(metaData, name);

                byte[] _fileBytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/saves/" + name + ".json");

                var reqUploadFile = new reward_req_upload();
                reqUploadFile.name = "jsonFile";
                var json2 = JsonConvert.SerializeObject(reqUploadFile);

                StartCoroutine(JsonUpload("http://localhost:5000/ipfs-json", _fileBytes, json2, name, (cidResult) =>
                {
                    Debug.Log("last json cid check: " + cidResult);
                    var MintReqUpload = new mint_req_upload();
                    MintReqUpload.addr = LoginManager.Instance.GetAddr();
                    MintReqUpload.ipfs = cidResult;
                    MintReqUpload.name = name;
                    MintReqUpload.description = description;
                    MintReqUpload.nftType = "3";

                    var json = JsonConvert.SerializeObject(MintReqUpload);

                    StartCoroutine(Upload($"http://localhost:5000/nfts/mint", json, null));
                }
                ));

            }));
        }

        public void newWinner(int highScore)
        {
            var UpdateRingrequpload = new UpdateRing_req_upload();
            //UpdateRingrequpload.addr = "";
            UpdateRingrequpload.addr= LoginManager.Instance.GetAddr();
            UpdateRingrequpload.score = highScore;
            var json = JsonConvert.SerializeObject(UpdateRingrequpload);

            StartCoroutine(Upload("http://localhost:5000/ring-score/update", json, null));
        }

        public int GetWinner()
        {
            var getRingrequpload = new getRing_req_upload();
            getRingrequpload.message = "";
            var json2 = JsonConvert.SerializeObject(getRingrequpload);
            var highscore = 0;
            StartCoroutine(Upload("http://localhost:5000/ring-data", json2, (result) =>
            {
                var getRingresupload = JsonConvert.DeserializeObject<getRing_res_upload>(result);
                highscore = getRingresupload.ringScore;
                rankController.SetRank(getRingresupload.ringOwner, highscore);
            }));
            return highscore;
        }


        // �����͸� �޾Ƽ� item���� ���� items�� ����
        private IEnumerator LoadNFT(int tokenId)
        {
            Debug.Log("----------" + tokenId + "-----");

            var getNFTrequpload = new getNFT_req_upload();
            getNFTrequpload.tokenId = tokenId;
            var json2 = JsonConvert.SerializeObject(getNFTrequpload);
            Item itemTemp = new Item();
            using (UnityWebRequest request = UnityWebRequest.Post($"http://localhost:5000/nfts", json2))
            {
                try
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json2);
                    request.uploadHandler.Dispose();
                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Error uploading data: " + request.error);
                    }
                    else
                    {
                        var result = request.downloadHandler.text;
                        var getNFTresponseResult = JsonConvert.DeserializeObject<getNFT_res_upload>(result);

                        itemTemp.tokenId = tokenId;
                        itemTemp.tokenURI = getNFTresponseResult.tokenURI;
                        itemTemp.name = getNFTresponseResult.name;
                        itemTemp.description = getNFTresponseResult.description;
                        itemTemp.nftType = getNFTresponseResult.nftType;
                        itemTemp.price = getNFTresponseResult.price;
                        itemTemp.isSelling = getNFTresponseResult.isSelling;

                        var _tokenURI = itemTemp.tokenURI;
                        var url = "";
                        if (itemTemp.nftType == 1)
                        {
                            url = $"https://gateway.pinata.cloud/ipfs/{_tokenURI.Substring(7)}";
                        }
                        else
                        {
                            url = $"https://aeong.infura-ipfs.io/ipfs/{_tokenURI.Substring(7)}";
                        }
                        StartCoroutine(GetImgCID(url, (result2) =>
                        {
                            var getJsonResult = JsonConvert.DeserializeObject<JsonCID>(result2);
                            var imgCID = getJsonResult.image;
                            var url2 = $"https://gateway.pinata.cloud/ipfs/{imgCID.Substring(7)}";
                            if (itemTemp.nftType != 1)
                            {
                                url2 = $"https://aeong.infura-ipfs.io/ipfs/{imgCID.Substring(7)}";
                            }

                            StartCoroutine(GetTexture(url2, (sprite) =>
                            {
                                itemTemp.image = sprite;
                                if (tokenId == TotalSupply.GetTotalSupply())
                                    Debug.Log("Done");

                            }));
                        }));
                    }
                }
                finally
                {
                    items.Add(itemTemp);
                    Debug.Log(tokenId + " == " + items.Count);

                    if (request != null)
                    {
                        request.Dispose();
                    }
                }
            }
        }
        public IEnumerator LoadItems()
        {
            var preTotalSupply = TotalSupply.GetTotalSupply();
            var preOwnedTokens = OwnedTokens.GetOwnedTokens();

            yield return StartCoroutine(LoadTotalSupply());

            var _totalSupply = TotalSupply.GetTotalSupply();
            var _OwnedTokens = OwnedTokens.GetOwnedTokens();
            _balanceOfKlay = BalanceOfKlay.GetBalanceOfKlay();

            if (_totalSupply > preTotalSupply)
            {
                // ���ο� �������� �߰��ϰ�,
                for (int i = preTotalSupply + 1; i <= _totalSupply; i++)
                {
                    LoadNFT(i);
                }

                // ������ �������� ���ݰ� �Ǹ������� ������Ʈ
                for (int j = 1; j <= preTotalSupply; j++)
                {
                    var getNFTrequpload = new getNFT_req_upload();
                    getNFTrequpload.tokenId = j;
                    var json = JsonConvert.SerializeObject(getNFTrequpload);

                    StartCoroutine(Upload("http://localhost:5000/nfts", json, (result) =>
                    {
                        var getNFTresponseResult = JsonConvert.DeserializeObject<getNFT_res_upload>(result);
                        Debug.Log("result: " + getNFTresponseResult);

                        items[j - 1].price = getNFTresponseResult.price;
                        items[j - 1].isSelling = getNFTresponseResult.isSelling;
                    }));
                }
            }
        }

        // ��� �������� �������� getter
        public List<Item> GetAllItems()
        {
            if (!isLoaded) // ��� �������� �ε����� �ʾҴٸ�,
            {
                Debug.LogWarning("��� �������� �ε����� �ʾҽ��ϴ�.");
                return null;
            }
            return items;
        }

        // isSelling ���� false�� ��� �����۵��� �����´�.
        public List<Item> GetNotSellingItems()
        {
            // notSellingItems ����Ʈ�� �������� ���� ���,
            // ���ο� ����Ʈ�� �����ϰ� ��� ������ �߿��� isSelling ���� false�� �����۵��� �߰�.
            if (notSellingItems == null)
            {
                notSellingItems = new List<Item>();
                foreach (var item in GetAllItems())
                {
                    if (!item.isSelling)
                    {
                        notSellingItems.Add(item);
                    }
                }
            }
            // notSellingItems ����Ʈ�� ��ȯ.
            return notSellingItems;
        }

        private void LoadNFTSkillInfos()
        {
            // Create Skill objects from the filtered skill items and add them to the skill list and dictionary
            foreach (Item skillItem in items)
            {
                //skillItem.nftType
/*                // Check if the item is in the items list
                if (items.Contains(skillItem))
                {
                    continue; // Skip adding this item as a skill
                }*/
                // Create a new SkillInfo object with the item's information
                SkillInfo skillInfo = new SkillInfo(skillItem.tokenId, skillItem.name, skillItem.description, skillItem.image, skillItem.nftType);
                skillInfoList.Add(skillInfo);
                skillInfoDictionary.Add(skillInfo.skillId, skillInfo);
            }
        }
        public SkillInfo GetNFTSkillInfoByID(int skillID)
        {
            // Check if the skillDictionary contains a Skill with the given skillID
            if (skillInfoDictionary.ContainsKey(skillID))
            {
                // Return the Skill object with the given skillID
                return skillInfoDictionary[skillID];
            }
            else
            {
                // If the skill is not found, log a warning and return null
                Debug.LogWarning($"NFTManager: Skill with ID {skillID} not found.");
                return null;
            }
        }

        public List<SkillInfo> GetOwnedNFTSkillInfos()
        {
            int[] ownedSkillids = OwnedTokens.GetOwnedTokens();
            List<SkillInfo> skillInfoList = new List<SkillInfo>();
            if (ownedSkillids == null)
            {
                Debug.LogWarning("Failed to get owned skill IDs from OwnedTokens");
                return null;
            }

            foreach (int skillID in ownedSkillids)
            {
                SkillInfo skillInfo = GetNFTSkillInfoByID(skillID);
                if (skillInfo != null) skillInfoList.Add(skillInfo);
            }
            return skillInfoList;
        }

        public void SellNFTItem(int tokenId, float price)
        {
            var sellNFTrequpload = new sellNFT_req_upload();
            sellNFTrequpload.tokenId = tokenId;
            sellNFTrequpload.price = price;
            var json = JsonConvert.SerializeObject(sellNFTrequpload);

            // ����� ��ٸ��� yield return �߰�
            StartCoroutine(Upload("http://localhost:5000/nfts/sell", json, (result) =>
            {
                var sellNFTresponseResult = JsonConvert.DeserializeObject<res_upload>(result);
                Debug.Log("result: " + sellNFTresponseResult);
            }));
        }
        public IEnumerator Upload(string URL, string json, System.Action<string> OnCompleteUpload)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(URL, json))
            {
                try
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
                    request.uploadHandler.Dispose();
                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.Log("Error uploading data: " + request.error);
                    }
                    else
                    {
                        OnCompleteUpload(request.downloadHandler.text);
                    }
                }
                finally
                {
                    if (request != null)
                    {
                        request.Dispose();
                    }
                }
            }
        }

        IEnumerator ImageUpload(string URL, byte[] bytes, string data, string _name, System.Action<string> OnCompleteUpload)
        {
            var byteArr = Encoding.UTF8.GetBytes(data);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("imgfile", bytes, _name + ".png", "image/png"));

            UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            var result = webRequest.downloadHandler.text;
            OnCompleteUpload(result);
            webRequest.Dispose();
        }

        IEnumerator JsonUpload(string URL, byte[] bytes, string data, string _name, System.Action<string> OnCompleteUpload)
        {
            var byteArr = Encoding.UTF8.GetBytes(data);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("jsonfile", bytes, _name + ".json", " application/json"));

            UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            try
            {
                yield return webRequest.SendWebRequest();
                var result = webRequest.downloadHandler.text;
                reward_res_upload responseResult = JsonConvert.DeserializeObject<reward_res_upload>(result);
                Debug.Log(result);
                Debug.Log("cid : " + responseResult.cid);
                string cidResult = responseResult.cid;
                OnCompleteUpload(cidResult);
            }
            finally
            {
                if (webRequest != null)
                {
                    webRequest.Dispose();
                }
            }
        }


        IEnumerator GetImgCID(string URL, System.Action<string> OnCompleteUpload)
        {
            using (WWW www = new WWW(URL))
            {
                try
                {
                    yield return www;
                    var result2 = www.text;

                    OnCompleteUpload(result2);
                }
                finally
                {
                    if (www != null)
                    {
                        www.Dispose();

                    }
                }
            }
        }
        IEnumerator GetTexture(string URL, System.Action<Sprite> OnCompleteUpload)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL))
            {
                try
                {
                    yield return www.SendWebRequest();
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture == null)
                    {
                        Debug.Log("texture null");
                        yield break;
                    }
                    Rect rect = new Rect(0, 0, texture.width, texture.height);
                    Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                    Debug.Log(sprite);

                    OnCompleteUpload(sprite);
                }
                finally
                {
                    if (www != null)
                    {
                        www.Dispose();
                    }
                }
            }
        }

    }
}