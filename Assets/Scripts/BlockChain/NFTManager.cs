using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
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
    // 민팅된 nft 총 갯수와 소유하고 있는 nft리스트를 모아다가 돈디스트로이에 넣자;
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
    // 받아와서 json에 넣을 거임
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
    // 받아와서 json에 넣을 거임

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

            StartCoroutine(FirstLoadItems());
        }
        #endregion
        private float _balanceOfKlay;
        private List<Item> items = new List<Item>();    // 모든 아이템들을 저장하는 배열
        private List<Item> notSellingItems = new List<Item>(); // isSelling이 false인 모든 아이템들을 저장하는 배열
        private List<Skill> skillList = new List<Skill>();
        private Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();
        private bool isLoaded = false; // 모든 아이템이 로딩되었는지 여부를 저장할 변수
        public float balanceOfKlay { get { return _balanceOfKlay; } }
        [SerializeField] private TextMeshProUGUI ringOwner;
        [SerializeField] private TextMeshProUGUI ringScore;

        private IEnumerator FirstLoadItems()
        {
            Debug.Log("FirstLoadItems Start");
            yield return StartCoroutine(LoadTotalSupply());

            var _totalSupply = TotalSupply.GetTotalSupply();

            // totalSupply 받아왔으니 이걸로 이제 for문 돌려서 데이터 받아옴.
            for (int i = 1; i <= _totalSupply; i++)
            {
                yield return StartCoroutine(LoadNFT(i));
            }
            // 이후 코드 실행
            // 모든 아이템이 로딩되었다는 플래그를 true로 변경
            yield return new WaitForSeconds(5.0f);
            isLoaded = true;
            LoadNFTSkills();
            SkillManager.Instance.LoadOwnedSkills(GetOwnedNFTSkills());
        }

        IEnumerator LoadTotalSupply()
        {
            // totalSupply와 ownedTokens을 post로 받아와서 전역으로 저장함.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            //UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            UpdateNFTrequpload.addr = "";
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // 결과를 기다리는 yield return 추가
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

            // 1. 이미지 스크린샷 후 ipfs 업로드 cid 반환 

            byte[] _imageBytes = Screenshot.TakeScreenshot_(1920, 1080, name);

            var reqUploadImage = new reward_req_upload();
            reqUploadImage.name = "imgfile";
            var json = JsonConvert.SerializeObject(reqUploadImage);

            StartCoroutine(ImageUpload("http://localhost:5000/ipfs-image", _imageBytes, json, name, (result) =>
            {
                var responseResult = JsonConvert.DeserializeObject<reward_res_upload>(result);
                Debug.Log("result: " + responseResult);
                Debug.Log("image cid : " + responseResult.cid);

                // 2. 이미지 cid를 이용하여 metadata.json 생성한 후 cid 받아오기
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

        public void newWinner()
        {
            var UpdateRingrequpload = new UpdateRing_req_upload();
            //UpdateRingrequpload.addr= LoginManager.Instance.GetAddr();
            UpdateRingrequpload.addr = "";
            UpdateRingrequpload.score = 0;
            var json = JsonConvert.SerializeObject(UpdateRingrequpload);

            StartCoroutine(Upload("http://localhost:5000/ring-score/update", json, null));
        }

        public void GetWinner()
        {
            var getRingrequpload = new getRing_req_upload();
            getRingrequpload.message = "";
            var json2 = JsonConvert.SerializeObject(getRingrequpload);

            StartCoroutine(Upload("http://localhost:5000/ring-data", json2, (result) =>
            {
                var getRingresupload = JsonConvert.DeserializeObject<getRing_res_upload>(result);

                Debug.Log("ringOwner : " + getRingresupload.ringOwner);
                Debug.Log("ringScore : " + getRingresupload.ringScore);

                ringOwner.text = "RingOwner: " + getRingresupload.ringOwner;
                ringScore.text = "RingScore : " + getRingresupload.ringScore.ToString();

            }));
        }


        // 데이터를 받아서 item으로 만들어서 items에 넣음
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
                // 새로운 아이템은 추가하고,
                for (int i = preTotalSupply + 1; i <= _totalSupply; i++)
                {
                    LoadNFT(i);
                }

                // 기존의 아이템은 가격과 판매중인지 업데이트
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

        // 모든 아이템을 가져오는 getter
        public List<Item> GetAllItems()
        {
            if (!isLoaded) // 모든 아이템이 로딩되지 않았다면,
            {
                Debug.LogWarning("모든 아이템이 로딩되지 않았습니다.");
                return null;
            }
            return items;
        }

        // isSelling 값이 false인 모든 아이템들을 가져온다.
        public List<Item> GetNotSellingItems()
        {
            // notSellingItems 리스트가 생성되지 않은 경우,
            // 새로운 리스트를 생성하고 모든 아이템 중에서 isSelling 값이 false인 아이템들을 추가.
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
            // notSellingItems 리스트를 반환.
            return notSellingItems;
        }

        private void LoadNFTSkills()
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
                SkillInfo skillInfo = new SkillInfo(skillItem.tokenId, skillItem.name, skillItem.description, skillItem.image);

                // Create a new Skill object with the SkillInfo and add it to the skillList and skillDictionary
                Skill skill = new Skill();
                skill.SetSkill(skillInfo, skillItem.nftType);
                skillList.Add(skill);
                skillDictionary.Add(skill.skillInfo.skillId, skill);
            }
        }
        public Skill GetNFTSkillByID(int skillID)
        {
            // Check if the skillDictionary contains a Skill with the given skillID
            if (skillDictionary.ContainsKey(skillID))
            {
                // Return the Skill object with the given skillID
                return skillDictionary[skillID];
            }
            else
            {
                // If the skill is not found, log a warning and return null
                Debug.LogWarning($"NFTManager: Skill with ID {skillID} not found.");
                return null;
            }
        }

        public List<Skill> GetOwnedNFTSkills()
        {
            int[] ownedSkillids = OwnedTokens.GetOwnedTokens();
            List<Skill> skillList = new List<Skill>();
            if (ownedSkillids == null)
            {
                Debug.LogWarning("Failed to get owned skill IDs from OwnedTokens");
                return null;
            }

            foreach (int skillID in ownedSkillids)
            {
                Skill skill = GetNFTSkillByID(skillID);
                if (skill) skillList.Add(skill);
            }
            return skillList;
        }

        public void SellNFTItem(int tokenId, float price)
        {
            var sellNFTrequpload = new sellNFT_req_upload();
            sellNFTrequpload.tokenId = tokenId;
            sellNFTrequpload.price = price;
            var json = JsonConvert.SerializeObject(sellNFTrequpload);

            // 결과를 기다리는 yield return 추가
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