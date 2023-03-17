using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlockChain;
namespace BlockChain
{
    public class ItemSlot : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemPrice;
        public Item item;
        public void SetSlot(Item item)
        {
            if (item.image) itemImage.sprite = item.image;
            itemName.text = item.name;
            itemPrice.text = item.price.ToString() + " KLAY";
        }

        /*public void SetSlotData(Item itemTemp)
        {
            item.tokenId = itemTemp.tokenId;
            item.tokenURI = itemTemp.tokenURI;
            item.name = itemTemp.name;
            item.description = itemTemp.description;
            item.nftType = itemTemp.nftType;
            item.price = itemTemp.price;
            item.isSelling = itemTemp.isSelling;
            item.image = itemTemp.image;

        }*/

        /*
        public int tokenId;
        public string tokenURI;
        public string name;
        public string description;
        public int nftType;
        public string price;
        public bool isSelling;
        public Sprite image;
        */
    }
}
