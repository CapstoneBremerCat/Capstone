using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;
using BlockChain;
using Game;
namespace BlockChain
{
    public class shop : MonoBehaviour
    {
        public GameObject itemButtonPrefab;
        public Transform itemPanelContainer;
        public GameObject shopPanel;
        public GameObject itemDetailsPanel;
        public TextMeshProUGUI shopKlay;
        public Button buyButton;
        public Button sellButton;
        public Button sellingButton;

        public InputField priceInput;
        public Button checkButton;
        public Button backButton;
        public Button allButton;
        public Button myNFTButton;
        public Button openShopButton;

        private int selectedItemId = 0;

        long Firsttime = 0;   // 첫번째 클릭시간

        private void Start()
        {
            openShopButton.onClick.AddListener(() => {
                UIManager.Instance.OpenPopup(shopPanel);
                RefreshStore();
            });
        }

        private bool One_Click()
        {
            long CurrentTime = DateTime.Now.Ticks;
            if (CurrentTime - Firsttime < 4000000) // 0.4초 ( MS에서는 더블클릭 평균 시간을 0.4초로 보는거 같다.)
            {
                Firsttime = CurrentTime;   // 더블클릭 또는 2회(2회, 3회 4회...)클릭 시 실행되지 않도록 함
                return false;   // 더블클릭 됨
            }
            else
            {
                Firsttime = CurrentTime;   // 1번만 실행되도록 함
                return true;   // 더블클릭 아님
            }
        }

        public void RefreshStore()
        {
            if (!GameManager.Instance.isOnNFT) return;
            StartCoroutine(refreshStoreRoutine());
        }

        private IEnumerator refreshStoreRoutine()
        {
            yield return StartCoroutine(NFTManager.Instance.RefreshInfo());
            yield return StartCoroutine(NFTManager.Instance.LoadItems());
            ALLButtonOnClick();
        }

        public void ALLButtonOnClick()
        {
            if (!GameManager.Instance.isOnNFT) return;
            //StartCoroutine(NFTManager.Instance.LoadItems());
            DisplayItems();
        }
        public void MyButtonOnClick()
        {
            if (!GameManager.Instance.isOnNFT) return;
            //StartCoroutine(NFTManager.Instance.LoadItems());
            DisplayMyItems();
        }


        private void DisplayItems()
        {
            // itemPanelContainer의 자식 오브젝트를 모두 삭제
            foreach (Transform child in itemPanelContainer.transform)
            {
                Destroy(child.gameObject);
            }

            var allItems = NFTManager.Instance.GetAllItems();
            if (allItems == null) return;
            foreach (Item item in allItems)
            {
                GameObject itemButton = Instantiate(itemButtonPrefab, itemPanelContainer.transform);
                itemButton.GetComponent<ItemSlot>().SetSlot(item);

                int itemId = item.tokenId;

                itemButton.GetComponent<Button>().onClick.AddListener(() => ShowItemDetails(itemId));
            }
        }

        private void DisplayMyItems()
        {
            // itemPanelContainer의 자식 오브젝트를 모두 삭제
            foreach (Transform child in itemPanelContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Item item in NFTManager.Instance.GetMyItems())
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

            Item selectedItem = NFTManager.Instance.GetAllItems().Find(item => item.tokenId == selectedItemId);
            itemDetailsPanel.GetComponent<ItemDetailsPanel>().SetItemDetails(selectedItem);
            UIManager.Instance.OpenPopup(itemDetailsPanel);

            var preOwnedTokens = OwnedTokens.GetOwnedTokens();
            if (Array.Exists(preOwnedTokens, x => x == selectedItem.tokenId))
            {
                if (selectedItem.isSelling)
                {
                    sellingButton.gameObject.SetActive(true);
                }
                else
                {
                    checkButton.onClick.RemoveAllListeners();
                    checkButton.onClick.AddListener(() =>
                    {
                        if (One_Click()) // 한번만 터치되도록 한다. ( 이중 터치 실행 방지 )
                        {
                            if (priceInput.text != "")
                            {
                                NFTManager.Instance.SellNFTItem(selectedItem.tokenId, float.Parse(priceInput.text));
                                // 숫자 초기화하는 코드 만들어야함 근데 귀찮아서 아직 안만듦
                                HideItemDetails();

                                priceInput.text = "";
                                RefreshStore();
                            }
                        }
                        sellButton.gameObject.SetActive(false);
                    });
                    if (selectedItem.tokenId != 1)
                    {
                        sellButton.gameObject.SetActive(true);
                    }
                }
            }
            else if (selectedItem.isSelling)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    var addr = LoginManager.Instance.GetAddr();
                    //Application.OpenURL($"http://localhost:3000/nfts/buy/{selectedItem.tokenId}/{selectedItem.price}/{addr}");

                    Application.OpenURL($"https://aeong-psi.vercel.app/nfts/buy/{selectedItem.tokenId}/{selectedItem.price}/{addr}");
                    HideItemDetails();
                });
                buyButton.gameObject.SetActive(true);
            }

            backButton.onClick.AddListener(() =>
            {
                RefreshStore();
            });

        }

        public void HideItemDetails()
        {
            selectedItemId = 0;
            UIManager.Instance.RemovePopup(itemDetailsPanel);
        }

        public void destroyItemSlot()
        {

        }
        //1. getKlay 가져오고 이제 float로 다 바꿔서 결제 가능하도록 해야함.
        //2. buy 끝나면, 계산하고 확인 버튼 만들어서 다시 돌아가게 해야함.
        //3. sell 버튼 누르면 얼마에 팔건지 입력해야함.
    }
}