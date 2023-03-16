using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;
using BlockChain;
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

    // 민팅된 nft 총 갯수와 소유하고 있는 nft리스트를 모아다가 돈디스트로이에 넣자;
    public class Test_res_upload : _packet
    {
        public string code;
        public string message;
    }

    public class UpdateNFT_req_upload
    {
        public string addr; // totalsupply, ownedTokens
    }

    // 민팅된 nft 총 갯수와 소유하고 있는 nft리스트를 모아다가 돈디스트로이에 넣자;
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


    // 받아와서 json에 넣을 거임
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


    // 받아와서 json에 넣을 거임
    public class sellNFT_res_upload : _packet
    {
        public string code;
        public string message;
    }

    public class shop : MonoBehaviour
    {
        public GameObject itemButtonPrefab;
        public Transform itemPanelContainer;
        public GameObject itemDetailsPanel;
        public Image itemDetailsImage;
        public TextMeshProUGUI itemDetailsName;
        public TextMeshProUGUI itemDetailsPrice;
        public TextMeshProUGUI itemDetailsDescription;
        public TextMeshProUGUI shopKlay;
        public Button buyButton;
        public Button sellButton;
        public InputField priceInput;
        public Button checkButton;

        private List<Item> items = new List<Item>();
        private int selectedItemId = 0;

        //public List<ItemSlot> itemSlots;

        private void Start()
        {
            Debug.Log("FirstLoadItems");
            StartCoroutine(FirstLoadItems());
            //StartCoroutine(Test());
        }

        public void StoreButtonOnClick()
        {
            Debug.Log("LoadItems");
            StartCoroutine(LoadItems());
            Debug.Log("DisplayItems");
            DisplayItems();
        }

        private IEnumerator FirstLoadItems()
        {
            Debug.Log("FirstLoadItems Start");
            yield return StartCoroutine(LoadTotalSupply());

            var _totalSupply = TotalSupply.GetTotalSupply();

            //Debug.Log("_totalSupply getNFT" + _totalSupply);
            //yield return StartCoroutine(LoadNFT(1));
            // totalSupply 받아왔으니 이걸로 이제 for문 돌려서 데이터 받아옴.
            for (int i = 1; i <= _totalSupply; i++)
            {
                //Debug.Log("start - LoadNFT");
                yield return StartCoroutine(LoadNFT(i));
                //Debug.Log("end - LoadNFT");
            }

            // 이후 코드 실행
        }

        private IEnumerator Test()
        {
            var Testrequpload = new Test_req_upload();
            //UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            Testrequpload.req = "sibul";
            var json = JsonConvert.SerializeObject(Testrequpload);

            // 결과를 기다리는 yield return 추가
            yield return StartCoroutine(Upload222("http://localhost:5000/Test", json, (result) =>
            {
                var Testresupload = JsonConvert.DeserializeObject<Test_res_upload>(result);

                Debug.Log(Testresupload.code);
                Debug.Log(Testresupload.message);

            }));
        }

        IEnumerator LoadTotalSupply()
        {
            // totalSupply와 ownedTokens을 post로 받아와서 전역으로 저장함.
            var UpdateNFTrequpload = new UpdateNFT_req_upload();
            //UpdateNFTrequpload.addr = LoginManager.Instance.GetAddr();
            UpdateNFTrequpload.addr = "";
            var json = JsonConvert.SerializeObject(UpdateNFTrequpload);

            // 결과를 기다리는 yield return 추가
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

                shopKlay.text = updateNFTresponseResult.balanceOfKlay.ToString() + " KLAY";

            }));
            //Debug.Log("LoadTotalSupply");
        }

        // 데이터를 받아서 item으로 만들어서 items에 넣음
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
        IEnumerator LoadItems()
        {
            var preTotalSupply = TotalSupply.GetTotalSupply();
            var preOwnedTokens = OwnedTokens.GetOwnedTokens();

            yield return StartCoroutine(LoadTotalSupply());

            var _totalSupply = TotalSupply.GetTotalSupply();
            var _OwnedTokens = OwnedTokens.GetOwnedTokens();
            var _balanceOfKlay = BalanceOfKlay.GetBalanceOfKlay();

            shopKlay.text = _balanceOfKlay.ToString() + " KLAY";


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

        private void DisplayItems()
        {
            foreach (Item item in items)
            {
                GameObject itemButton = Instantiate(itemButtonPrefab, itemPanelContainer.transform);
                itemButton.GetComponent<ItemSlot>().SetSlot(item);

                int itemId = item.tokenId;

                itemButton.GetComponent<Button>().onClick.AddListener(() => ShowItemDetails(itemId));
            }
        }


        private void ShowItemDetails(int itemId)
        {
            selectedItemId = itemId;

            Item selectedItem = items.Find(item => item.tokenId == selectedItemId);
            itemDetailsImage.sprite = selectedItem.image;
            itemDetailsName.text = selectedItem.name;
            itemDetailsDescription.text = selectedItem.description;
            itemDetailsPrice.text = selectedItem.price.ToString() + " KLAY";

            itemDetailsPanel.SetActive(true);

            var preOwnedTokens = OwnedTokens.GetOwnedTokens();
            if (Array.Exists(preOwnedTokens, x => x == selectedItem.tokenId))
            {

                checkButton.onClick.AddListener(() =>
                {

                    if (priceInput.text != "")
                    {
                        Debug.Log(priceInput.text);

                        var sellNFTrequpload = new sellNFT_req_upload();
                        sellNFTrequpload.tokenId = selectedItem.tokenId;
                        sellNFTrequpload.price = float.Parse(priceInput.text);
                        var json = JsonConvert.SerializeObject(sellNFTrequpload);

                        // 결과를 기다리는 yield return 추가
                        StartCoroutine(Upload("http://localhost:5000/sellNFT", json, (result) =>
                        {
                            var sellNFTresponseResult = JsonConvert.DeserializeObject<sellNFT_res_upload>(result);
                            Debug.Log("result: " + sellNFTresponseResult);
                        }));

                        // 숫자 초기화하는 코드 만들어야함 근데 귀찮아서 아직 안만듦
                        HideItemDetails();
                    }
                });
                if (selectedItem.tokenId != 1)
                {
                    sellButton.gameObject.SetActive(true);
                }
            }
            else if (selectedItem.isSelling)
            {
                buyButton.onClick.AddListener(() =>
                {
                    //var addr = LoginManager.Instance.GetAddr();
                    var addr = "0xd026F9247E982f087F8CcB4FD334C7d78039c37A";
                    Application.OpenURL($"http://localhost:3000/buy/{selectedItem.tokenId}/{selectedItem.price}/{addr}");
                    HideItemDetails();
                });
                buyButton.gameObject.SetActive(true);
            }
        }

        public void HideItemDetails()
        {
            selectedItemId = 0;
            itemDetailsPanel.SetActive(false);
        }

        public void destroyItemSlot()
        {

        }

        IEnumerator Upload(string URL, string json, System.Action<string> OnCompleteUpload)
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

        IEnumerator Upload222(string URL, string json, System.Action<string> OnCompleteUpload)
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
                        request.Dispose();
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



        //1. getKlay 가져오고 이제 float로 다 바꿔서 결제 가능하도록 해야함.
        //2. buy 끝나면, 계산하고 확인 버튼 만들어서 다시 돌아가게 해야함.
        //3. sell 버튼 누르면 얼마에 팔건지 입력해야함.

    }
}