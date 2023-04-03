using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;	// UnityWebRequest����� ���ؼ� �����ش�.
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
            // itemPanelContainer�� �ڽ� ������Ʈ�� ��� ����
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
                        // ���� �ʱ�ȭ�ϴ� �ڵ� �������� �ٵ� �����Ƽ� ���� �ȸ���
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
        //1. getKlay �������� ���� float�� �� �ٲ㼭 ���� �����ϵ��� �ؾ���.
        //2. buy ������, ����ϰ� Ȯ�� ��ư ���� �ٽ� ���ư��� �ؾ���.
        //3. sell ��ư ������ �󸶿� �Ȱ��� �Է��ؾ���.
    }
}