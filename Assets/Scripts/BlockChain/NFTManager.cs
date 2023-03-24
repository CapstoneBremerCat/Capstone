using System.Collections;
using System.Collections.Generic;
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
    public class _packet
    {
        public int errorno;
    }
    public class Test_req_upload
    {
        public string req; // totalsupply, ownedTokens
    }
    // ���õ� nft �� ������ �����ϰ� �ִ� nft����Ʈ�� ��ƴٰ� ����Ʈ���̿� ����;
    public class Test_res_upload : _packet
    {
        public string code;
        public string message;
    }
    public class UpdateNFT_req_upload
    {
        public string addr; // totalsupply, ownedTokens
    }
    // ���õ� nft �� ������ �����ϰ� �ִ� nft����Ʈ�� ��ƴٰ� ����Ʈ���̿� ����;
    public class UpdateNFT_res_upload : _packet
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
    public class getNFT_res_upload : _packet
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
    public class sellNFT_res_upload : _packet
    {
        public string code;
        public string message;
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
    public class getRing_res_upload : _packet
    {
        public string code;
        public string message;
        public string ringOwner;
        public int ringScore;
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
        private List<Skill> skillList = new List<Skill>();
        private Dictionary<int, Skill> skillDictionary = new Dictionary<int, Skill>();
        private bool isLoaded = false; // ��� �������� �ε��Ǿ����� ���θ� ������ ����
        public float balanceOfKlay { get { return _balanceOfKlay; } }
        [SerializeField] private TextMeshProUGUI ringOwner;
        [SerializeField] private  TextMeshProUGUI ringScore;

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
            // ���� �ڵ� ����
            // ��� �������� �ε��Ǿ��ٴ� �÷��׸� true�� ����
            isLoaded = true;
            LoadNFTSkills();

        }

        IEnumerator LoadTotalSupply()
        {
            // totalSupply�� ownedTokens�� post�� �޾ƿͼ� �������� ������.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            //UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            UpdateNFTrequpload.addr = "";
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // ����� ��ٸ��� yield return �߰�
            yield return StartCoroutine(Upload("http://localhost:5000/updateNFT", json, (result) =>
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

        // �����͸� �޾Ƽ� item���� ���� items�� ����
        private IEnumerator LoadNFT(int tokenId)
        {
            Debug.Log("----------" + tokenId + "-----");

            var getNFTrequpload = new getNFT_req_upload();
            getNFTrequpload.tokenId = tokenId;
            var json2 = JsonConvert.SerializeObject(getNFTrequpload);
            Item itemTemp = new Item();
            using (UnityWebRequest request = UnityWebRequest.Post($"http://localhost:5000/getNFT", json2))
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

                    StartCoroutine(Upload("http://localhost:5000/getNFT", json, (result) =>
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

        private void LoadNFTSkills()
        {
            // Create Skill objects from the filtered skill items and add them to the skill list and dictionary
            foreach (Item skillItem in items)
            {
                // Check if the item is in the items list
                if (items.Contains(skillItem))
                {
                    continue; // Skip adding this item as a skill
                }
                // Create a new SkillInfo object with the item's information
                SkillInfo skillInfo = new SkillInfo(skillItem.tokenId, skillItem.name, skillItem.description, skillItem.image);

                // Create a new Skill object with the SkillInfo and add it to the skillList and skillDictionary
                Skill skill = new Skill(skillInfo);
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

        public List<Skill> GetOwnedSkills()
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
        public void SellNFTItem(int tokenId, float price)
        {
            var sellNFTrequpload = new sellNFT_req_upload();
            sellNFTrequpload.tokenId = tokenId;
            sellNFTrequpload.price = price;
            var json = JsonConvert.SerializeObject(sellNFTrequpload);

            // ����� ��ٸ��� yield return �߰�
            StartCoroutine(Upload("http://localhost:5000/sellNFT", json, (result) =>
            {
                var sellNFTresponseResult = JsonConvert.DeserializeObject<sellNFT_res_upload>(result);
                Debug.Log("result: " + sellNFTresponseResult);
            }));
        }
        public void BuyNFTItem(int tokenId, float price)
        {

        }
    }
}