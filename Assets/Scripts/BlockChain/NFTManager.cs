using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
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
        public string nftType;
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
        public string nftType;
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
    public class reward_req_upload // �����̶� ������Ƽ�� ���� Ȯ���ϱ�� and ���������� ����
    {
        public string addr;
    }

    public class reward_res_upload : packet
    {
        public string code;
        public string message;
        public int achievements; // ���� �޼��ߴ� ������
        public int rewardTickets;
    }

    public class clear_req_upload
    {
        public string addr;
        public string mode;
        public int score;
        public int timestamp;
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

    public class achivements_req_upload
    {
        public string addr;
        public int achievementIndex;  // ���� �޼��� ���� �ε���
    }

    public class randomItem_res_upload : packet
    {
        public string code;
        public string message;
        public string ipfs;
        public string name;
        public string description;
        public string nftType;
        public int rewardTickets;
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
        }
        #endregion

        private float _balanceOfKlay;
        private List<Item> items = new List<Item>();    // ��� �����۵��� �����ϴ� �迭
        private List<Item> notSellingItems = new List<Item>(); // isSelling�� false�� ��� �����۵��� �����ϴ� �迭
        private List<Item> myItems = new List<Item>();
        private List<SkillInfo> skillInfoList = new List<SkillInfo>();
        private Dictionary<int, SkillInfo> skillInfoDictionary = new Dictionary<int, SkillInfo>();
        private bool isLoaded = false; // ��� �������� �ε��Ǿ����� ���θ� ������ ����
        private bool isLoadWinner = false; // ��� �������� �ε��Ǿ����� ���θ� ������ ����
        private bool isLoadAhievement = false; // ��� �������� �ε��Ǿ����� ���θ� ������ ����
        public float balanceOfKlay { get { return _balanceOfKlay; } }
        public bool isLoadAll { get { return isLoaded & isLoadWinner & isLoadAhievement; } }

        [SerializeField] private RankController rankController;
        [SerializeField] private AchievementController achievementController;
        [SerializeField] private GameObject LoadingPanel;
        private int nftImgCount = 0;
        private int totalSupply = 0;
        
        public int rewardTickets { get; private set; }
        public int achievementsCode { get; private set; }
        public void InitNFT()
        {
            if (!GameManager.Instance.isOnNFT) return;
            StartCoroutine(FirstLoadItems());
        }
        public void RefreshNFT()
        {
            if (!GameManager.Instance.isOnNFT) return;
            StartCoroutine(RefreshInfo());
        }

        private IEnumerator FirstLoadItems()
        {
            LoadingPanel.SetActive(true);
            isLoaded = false;
            Debug.Log("FirstLoadItems Start");
            yield return StartCoroutine(LoadTotalSupply());

            totalSupply = TotalSupply.GetTotalSupply();

            // totalSupply �޾ƿ����� �̰ɷ� ���� for�� ������ ������ �޾ƿ�.
            for (int i = 1; i <= totalSupply; i++)
            {
                yield return StartCoroutine(LoadNFT(i));
            }
            GetWinner();
            GetAchivements();

            yield return null;

            while (isLoadAll == false)
            {
                yield return null;
            }
            // ��� �������� �ε��Ǿ��ٴ� �÷��װ� true�� ���,
            Debug.Log("Done");
            LoadingPanel.SetActive(false);
            // ���� �ڵ� ����
            LoadNFTSkillInfos();
            SkillManager.Instance.LoadOwnedSkills(GetOwnedNFTSkillInfos());
            achievementController.initAchievement();
            StartCoroutine(LoadItems());
        }
        public IEnumerator RefreshInfo()
        {
            LoadingPanel.SetActive(true);
            isLoaded = false;
            yield return StartCoroutine(LoadTotalSupply());
            var pre_totalSupply = totalSupply;
            totalSupply = TotalSupply.GetTotalSupply();
            if (pre_totalSupply == totalSupply)
            {
                isLoaded = true;
                LoadingPanel.SetActive(false);
                yield break;
            }
            // totalSupply �޾ƿ����� �̰ɷ� ���� for�� ������ ������ �޾ƿ�.
            for (int i = pre_totalSupply + 1; i <= totalSupply; i++)
            {
                yield return StartCoroutine(LoadNFT(i));
            }
            yield return null;

            while (isLoaded == false)
            {
                yield return null;
            }
            // ��� �������� �ε��Ǿ��ٴ� �÷��װ� true�� ���,
            LoadingPanel.SetActive(false);
        }
        IEnumerator LoadTotalSupply()
        {
            // totalSupply�� ownedTokens�� post�� �޾ƿͼ� �������� ������.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // ����� ��ٸ��� yield return �߰�
            yield return StartCoroutine(Upload("http://localhost:5000/users-data", json, (result) =>
            {
                var updateNFTresponseResult = JsonConvert.DeserializeObject<UpdateNFT_res_upload>(result);
                //Debug.Log("result: " + updateNFTresponseResult);
                Debug.Log("totalSupply : " + updateNFTresponseResult.totalSupply);
                Debug.Log("ownedTokens : " + updateNFTresponseResult.ownedTokens);

                TotalSupply.SetTotalSupply(updateNFTresponseResult.totalSupply);
                OwnedTokens.SetOwnedTokens(updateNFTresponseResult.ownedTokens);
                BalanceOfKlay.SetBalanceOfKlay(updateNFTresponseResult.balanceOfKlay);
                Debug.Log("BalanceOfKlay : " + updateNFTresponseResult.balanceOfKlay);
                Debug.Log(BalanceOfKlay.GetBalanceOfKlay());

                _balanceOfKlay = updateNFTresponseResult.balanceOfKlay;

            }));
        }
        
        public void MintRewardNFT(int _achievementIndex)
        {
            string name = AchievementDatabase.Instance.GetAchievementDataByCode(_achievementIndex).name;
            if (name == null)
            {
                Debug.LogError(string.Format("Failed to retrieve achievement name for index {0}.", _achievementIndex));
                return;
            }
            // Remove any whitespace characters from the name
            name = string.Join("", name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            string addr = LoginManager.Instance.GetAddr();

            name = string.Format("{0}_{1}", name, addr);

            //byte[] _imageBytes = TakeScreenshot(1920,1080);
            StartCoroutine(TakeScreenshot(1920, 1080, (_imageBytes) => {
                StartCoroutine(ImageUpload("http://localhost:5000/ipfs-image", _imageBytes, name, (result) =>
                {
                    var achivementsReqUpload = new achivements_req_upload();
                    achivementsReqUpload.addr = addr;
                    achivementsReqUpload.achievementIndex = _achievementIndex;
                    var json = JsonConvert.SerializeObject(achivementsReqUpload);

                    StartCoroutine(Upload("http://localhost:5000/nfts/rewardNft", json, (result) =>
                    {
                        var rewardResUpload = JsonConvert.DeserializeObject<reward_res_upload>(result);

                        Debug.Log("achivements : " + rewardResUpload.achievements);
                    }));
                }));
            }));
        }

        private IEnumerator TakeScreenshot(int _width, int _height, System.Action<byte[]> OnCompleteCapture)
        {
            string fileName = "Capture" + ".png";
            string folderPath = Application.persistentDataPath + "/screenshots/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = folderPath + fileName;
            ScreenCapture.CaptureScreenshot(filePath);

            while (!File.Exists(filePath)) // ��ũ���� ������ ������ ������ ���
            {
                yield return null;
            }

            var bytesImage = File.ReadAllBytes(filePath);
            OnCompleteCapture(bytesImage);

            // ���� ����
            File.Delete(filePath);
        }

        public void AddRewardTickets() // ���� ����̶� Ƽ�� �� Ȯ�� //�α����ϰ� ó���� �޾ƿ��� ���ķδ� �������� �� �޾ƿ��� ��
        {
            var rewardReqUpload = new reward_req_upload();
            rewardReqUpload.addr = LoginManager.Instance.GetAddr();

            var json = JsonConvert.SerializeObject(rewardReqUpload);

            StartCoroutine(Upload("http://localhost:5000/rewardTickets", json, (result) =>
            {
                var rewardResUpload = JsonConvert.DeserializeObject<reward_res_upload>(result);

                Debug.Log("tickets : " + rewardResUpload.rewardTickets);

                OwnedRewardTickets.SetOwnedRewardTickets(rewardResUpload.rewardTickets);
                Mediator.Instance.Notify(this, GameEvent.NFTTICKET_EARNED, rewardResUpload.rewardTickets);
            }));
        }

        public void GetAchivements() // ���� ����̶� Ƽ�� �� Ȯ�� //�α����ϰ� ó���� �޾ƿ��� ���ķδ� �������� �� �޾ƿ��� ��
        {
            isLoadAhievement = false;
            var rewardReqUpload = new reward_req_upload();
            rewardReqUpload.addr = LoginManager.Instance.GetAddr();

            var json = JsonConvert.SerializeObject(rewardReqUpload);

            StartCoroutine(Upload("http://localhost:5000/users-achievements", json, (result) =>
            {
                var rewardResUpload = JsonConvert.DeserializeObject<reward_res_upload>(result);

                Debug.Log("tickets : " + rewardResUpload.rewardTickets);
                Debug.Log("achivements : " + rewardResUpload.achievements);

                OwnedRewardTickets.SetOwnedRewardTickets(rewardResUpload.rewardTickets);
                achievementsCode = rewardResUpload.achievements;
                isLoadAhievement = true;
                Mediator.Instance.Notify(this, GameEvent.NFTTICKET_EARNED, rewardResUpload.rewardTickets);
            }));
        }
        [SerializeField] private NFTItemPopup itemPopup;
        public void UseRewardTicket() //Ƽ�� ����ϸ� ������ ����ǰ� ����� ������ ���� �˷���
        {
            if (!GameManager.Instance.isOnNFT) return;
            LoadingPanel.SetActive(true);
            Sprite resultSprite = null;
            var rewardReqUpload = new reward_req_upload();
            rewardReqUpload.addr = LoginManager.Instance.GetAddr();

            var json = JsonConvert.SerializeObject(rewardReqUpload);
            
            StartCoroutine(Upload("http://localhost:5000/nfts/randomReward", json, (result) =>
            {
                var randomItemResUpload = JsonConvert.DeserializeObject<randomItem_res_upload>(result);

                Debug.Log("randomItemIpfs : " + randomItemResUpload.ipfs);
                Debug.Log("randomItemName : " + randomItemResUpload.name);
                Debug.Log("randomItemNfttype : " + randomItemResUpload.nftType);
                Debug.Log("rewardTickets : " + randomItemResUpload.rewardTickets);


                OwnedRewardTickets.SetOwnedRewardTickets(randomItemResUpload.rewardTickets);
                Mediator.Instance.Notify(this, GameEvent.NFTTICKET_EARNED, randomItemResUpload.rewardTickets);

                var _tokenURI = randomItemResUpload.ipfs;
                var url = $"https://aeong.infura-ipfs.io/ipfs/{_tokenURI.Substring(7)}";

                StartCoroutine(GetImgCID2(url, (result2) =>
                {
                    var getJsonResult = JsonConvert.DeserializeObject<JsonCID>(result2);
                    var imgCID = getJsonResult.image;
                    var url2 = $"https://aeong.infura-ipfs.io/ipfs/{imgCID.Substring(7)}";

                    StartCoroutine(GetTexture(url2, (sprite) =>
                    {
                        LoadingPanel.SetActive(false);
                        resultSprite = sprite; ;
                        itemPopup.SetNFTItem(randomItemResUpload.name, randomItemResUpload.description, resultSprite);
                        itemPopup.OpenPopup();
                    }));
                }));

            }));

        }

        public void RecordClearResult(string _mode, int _score, int _timestamp)
        {
            var clearReqUpload = new clear_req_upload();
            clearReqUpload.addr = LoginManager.Instance.GetAddr();
            clearReqUpload.mode = _mode;
            clearReqUpload.score = _score;
            clearReqUpload.timestamp = _timestamp;

            var json = JsonConvert.SerializeObject(clearReqUpload);

            StartCoroutine(Upload2("http://localhost:5000/users-clearResults", json));
        }

        public void newWinner(int highScore)
        {
            var UpdateRingrequpload = new UpdateRing_req_upload();
            //UpdateRingrequpload.addr = "";
            UpdateRingrequpload.addr= LoginManager.Instance.GetAddr();
            UpdateRingrequpload.score = highScore;

            var json = JsonConvert.SerializeObject(UpdateRingrequpload);
            StartCoroutine(Upload2("http://localhost:5000/ring-score/update", json));

            if (rankController) rankController.SetRank(UpdateRingrequpload.addr, highScore);
            Mediator.Instance.Notify(this, GameEvent.ACHIEVEMENT_UNLOCKED, Achievement.RingOwner);
        }

        public int GetWinner()
        {
            isLoadWinner = false;
            var getRingrequpload = new getRing_req_upload();
            getRingrequpload.message = "";
            var json2 = JsonConvert.SerializeObject(getRingrequpload);
            var highscore = 0;
            StartCoroutine(Upload("http://localhost:5000/ring-data", json2, (result) =>
            {
                var getRingresupload = JsonConvert.DeserializeObject<getRing_res_upload>(result);
                highscore = getRingresupload.ringScore;
                rankController.SetRank(getRingresupload.ringOwner, highscore);
                isLoadWinner = true;
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
                        var url = $"https://aeong.infura-ipfs.io/ipfs/{_tokenURI.Substring(7)}";

                        StartCoroutine(GetImgCID2(url, (result2) =>
                        {
                            var getJsonResult = JsonConvert.DeserializeObject<JsonCID>(result2);
                            var imgCID = getJsonResult.image;
                            var url2 = $"https://aeong.infura-ipfs.io/ipfs/{imgCID.Substring(7)}";

                            StartCoroutine(GetTexture(url2, (sprite) =>
                            {
                                itemTemp.image = sprite;
                                nftImgCount++;
                                if (nftImgCount == TotalSupply.GetTotalSupply())
                                {
                                    isLoaded = true;
                                }

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

            // ������ �������� ���ݰ� �Ǹ������� ������Ʈ
            for (int j = 2; j <= preTotalSupply; j++)
            {
                var getNFTrequpload = new getNFT_req_upload();
                getNFTrequpload.tokenId = j;
                var json = JsonConvert.SerializeObject(getNFTrequpload);
                var index = j-1;
                StartCoroutine(Upload("http://localhost:5000/nfts", json, (result) =>
                {
                    var getNFTresponseResult = JsonConvert.DeserializeObject<getNFT_res_upload>(result);
                    //Debug.Log("result: " + getNFTresponseResult);
                    Debug.Log("preTotalSupply: " + preTotalSupply);
                    Debug.Log(index);
                    //Debug.Log(j + "~~~" + items[j - 1].tokenId);
                    //Debug.Log("Count: " + items.Count);
                    //Debug.Log(items[j - 1].price+ "price: " + getNFTresponseResult.price);
                    //Debug.Log(items[j - 1].isSelling+ "isSelling: " + getNFTresponseResult.isSelling);

                    items[index].price = getNFTresponseResult.price;
                    items[index].isSelling = getNFTresponseResult.isSelling;
                }));
            }

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
            /*if (notSellingItems == null)
            {
                notSellingItems = new List<Item>();
                foreach (var item in GetAllItems())
                {
                    if (!item.isSelling)
                    {
                        notSellingItems.Add(item);
                    }
                }
            }*/
            if (notSellingItems == null)
            {
                notSellingItems = new List<Item>();

            }
            notSellingItems.Clear();

            foreach (var item in GetAllItems())
            {
                if (!item.isSelling)
                {
                    notSellingItems.Add(item);
                }
            }
            // notSellingItems ����Ʈ�� ��ȯ.
            return notSellingItems;
        }

        public List<Item> GetMyItems()
        {
            var _OwnedTokens = OwnedTokens.GetOwnedTokens();
            // notSellingItems ����Ʈ�� �������� ���� ���,
            // ���ο� ����Ʈ�� �����ϰ� ��� ������ �߿��� isSelling ���� false�� �����۵��� �߰�.
            if (myItems == null)
            {
                myItems = new List<Item>();

            }
            myItems.Clear();

            for (int id = 1; id < _OwnedTokens.Length; id++)
            {
                foreach (var item in GetAllItems())
                {
                    if (item.tokenId == _OwnedTokens[id])
                    {
                        myItems.Add(item);
                    }
                }
            }

            return myItems;
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
                if (skillItem.nftType[0] == 'R') continue;
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
                        Debug.Log(request.downloadHandler.text);
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

        public IEnumerator Upload2(string URL, string json)
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


        IEnumerator ImageUpload(string URL, byte[] bytes, string _name, System.Action<string> OnCompleteUpload)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("imgfile", bytes, _name + ".png", "image/png"));

            UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            var result = webRequest.downloadHandler.text;
            OnCompleteUpload(result);
            webRequest.Dispose();
        }

/*        IEnumerator GetImgCID(string URL, System.Action<string> OnCompleteUpload)
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
        }*/
        IEnumerator GetImgCID2(string URL, System.Action<string> OnCompleteUpload)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                try
                {
                    yield return www.SendWebRequest();
                    var result2 = www.downloadHandler.text;

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