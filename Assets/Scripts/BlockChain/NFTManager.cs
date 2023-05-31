using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
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
    // 받아와서 json에 넣을 거임
    public class reward_req_upload // 업적이랑 리워드티켓 갯수 확인하기용 and 랜덤아이템 까기용
    {
        public string addr;
    }

    public class reward_res_upload : packet
    {
        public string code;
        public string message;
        public int achievements; // 내가 달성했던 업적들
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
        public int achievementIndex;  // 지금 달성한 업적 인덱스
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

        private float _balanceOfKlay;
        private List<Item> items = new List<Item>();    // 모든 아이템들을 저장하는 배열
        private List<Item> notSellingItems = new List<Item>(); // isSelling이 false인 모든 아이템들을 저장하는 배열
        private List<Item> myItems = new List<Item>();
        private List<SkillInfo> skillInfoList = new List<SkillInfo>();
        private Dictionary<int, SkillInfo> skillInfoDictionary = new Dictionary<int, SkillInfo>();
        private bool isLoaded = false; // 모든 아이템이 로딩되었는지 여부를 저장할 변수
        private bool isLoadWinner = false; // 모든 아이템이 로딩되었는지 여부를 저장할 변수
        private bool isLoadAhievement = false; // 모든 아이템이 로딩되었는지 여부를 저장할 변수
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

            // totalSupply 받아왔으니 이걸로 이제 for문 돌려서 데이터 받아옴.
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
            // 모든 아이템이 로딩되었다는 플래그가 true일 경우,
            Debug.Log("Done");
            LoadingPanel.SetActive(false);
            // 이후 코드 실행
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
            // totalSupply 받아왔으니 이걸로 이제 for문 돌려서 데이터 받아옴.
            for (int i = pre_totalSupply + 1; i <= totalSupply; i++)
            {
                yield return StartCoroutine(LoadNFT(i));
            }
            yield return null;

            while (isLoaded == false)
            {
                yield return null;
            }
            // 모든 아이템이 로딩되었다는 플래그가 true일 경우,
            LoadingPanel.SetActive(false);
        }
        IEnumerator LoadTotalSupply()
        {
            // totalSupply와 ownedTokens을 post로 받아와서 전역으로 저장함.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // 결과를 기다리는 yield return 추가
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

            while (!File.Exists(filePath)) // 스크린샷 파일이 생성될 때까지 대기
            {
                yield return null;
            }

            var bytesImage = File.ReadAllBytes(filePath);
            OnCompleteCapture(bytesImage);

            // 파일 삭제
            File.Delete(filePath);
        }

        public void AddRewardTickets() // 업적 기록이랑 티켓 수 확인 //로그인하고 처음에 받아오고 그후로는 구매했을 때 받아오면 됨
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

        public void GetAchivements() // 업적 기록이랑 티켓 수 확인 //로그인하고 처음에 받아오고 그후로는 구매했을 때 받아오면 됨
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
        public void UseRewardTicket() //티켓 사용하면 아이템 발행되고 발행된 아이템 정보 알려줌
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
                        resultSprite = sprite;;
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

            // 기존의 아이템은 가격과 판매중인지 업데이트
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
                // 새로운 아이템은 추가하고,
                for (int i = preTotalSupply + 1; i <= _totalSupply; i++)
                {
                    LoadNFT(i);
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
            // notSellingItems 리스트를 반환.
            return notSellingItems;
        }

        public List<Item> GetMyItems()
        {
            var _OwnedTokens = OwnedTokens.GetOwnedTokens();
            // notSellingItems 리스트가 생성되지 않은 경우,
            // 새로운 리스트를 생성하고 모든 아이템 중에서 isSelling 값이 false인 아이템들을 추가.
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