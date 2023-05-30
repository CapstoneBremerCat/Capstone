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
        private bool IsAll = true;

        long Firsttime = 0;   // ù��° Ŭ���ð�

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
            if (CurrentTime - Firsttime < 4000000) // 0.4�� ( MS������ ����Ŭ�� ��� �ð��� 0.4�ʷ� ���°� ����.)
            {
                Firsttime = CurrentTime;   // ����Ŭ�� �Ǵ� 2ȸ(2ȸ, 3ȸ 4ȸ...)Ŭ�� �� ������� �ʵ��� ��
                return false;   // ����Ŭ�� ��
            }
            else
            {
                Firsttime = CurrentTime;   // 1���� ����ǵ��� ��
                return true;   // ����Ŭ�� �ƴ�
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
            IsAll = true;
            //StartCoroutine(NFTManager.Instance.LoadItems());
            DisplayItems();
        }
        public void MyButtonOnClick()
        {
            if (!GameManager.Instance.isOnNFT) return;
            IsAll = false;
            //StartCoroutine(NFTManager.Instance.LoadItems());
            DisplayMyItems();
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

        private void DisplayMyItems()
        {
            // itemPanelContainer�� �ڽ� ������Ʈ�� ��� ����
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
                        if (One_Click()) // �ѹ��� ��ġ�ǵ��� �Ѵ�. ( ���� ��ġ ���� ���� )
                        {
                            if (priceInput.text != "")
                            {
                                NFTManager.Instance.SellNFTItem(selectedItem.tokenId, float.Parse(priceInput.text));
                                // ���� �ʱ�ȭ�ϴ� �ڵ� �������� �ٵ� �����Ƽ� ���� �ȸ���
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
        //1. getKlay �������� ���� float�� �� �ٲ㼭 ���� �����ϵ��� �ؾ���.
        //2. buy ������, ����ϰ� Ȯ�� ��ư ���� �ٽ� ���ư��� �ؾ���.
        //3. sell ��ư ������ �󸶿� �Ȱ��� �Է��ؾ���.
    }
}