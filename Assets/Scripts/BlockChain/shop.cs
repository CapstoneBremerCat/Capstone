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
    public class shop : MonoBehaviour
    {
        public GameObject itemButtonPrefab;
        public Transform itemPanelContainer;
        public GameObject itemDetailsPanel;
        public TextMeshProUGUI shopKlay;
        public Button buyButton;
        public Button sellButton;
        public InputField priceInput;
        public Button checkButton;

        private int selectedItemId = 0;

        public void StoreButtonOnClick()
        {
            Debug.Log("LoadItems");
            StartCoroutine(NFTManager.Instance.LoadItems());
            Debug.Log("DisplayItems");
            DisplayItems();
        }

        private void DisplayItems()
        {
            // itemPanelContainer의 자식 오브젝트를 모두 삭제
            foreach (Transform child in itemPanelContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Item item in NFTManager.Instance.GetAllItems())
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
            itemDetailsPanel.SetActive(true);

            var preOwnedTokens = OwnedTokens.GetOwnedTokens();
            if (Array.Exists(preOwnedTokens, x => x == selectedItem.tokenId))
            {

                checkButton.onClick.AddListener(() =>
                {
                    if (priceInput.text != "")
                    {
                        NFTManager.Instance.SellNFTItem(selectedItem.tokenId, float.Parse(priceInput.text));
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
        //1. getKlay 가져오고 이제 float로 다 바꿔서 결제 가능하도록 해야함.
        //2. buy 끝나면, 계산하고 확인 버튼 만들어서 다시 돌아가게 해야함.
        //3. sell 버튼 누르면 얼마에 팔건지 입력해야함.
    }
}