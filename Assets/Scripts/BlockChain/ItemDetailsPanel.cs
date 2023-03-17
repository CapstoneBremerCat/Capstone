using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlockChain;
namespace BlockChain
{
    public class ItemDetailsPanel : MonoBehaviour
    {
        [SerializeField] private Image itemDetailsImage;
        [SerializeField] private TextMeshProUGUI itemDetailsName;
        [SerializeField] private TextMeshProUGUI itemDetailsPrice;
        [SerializeField] private TextMeshProUGUI itemDetailsDescription;

        public void SetItemDetails(Item selectedItem)
        {
            itemDetailsImage.sprite = selectedItem.image;
            itemDetailsName.text = selectedItem.name;
            itemDetailsDescription.text = selectedItem.description;
            itemDetailsPrice.text = selectedItem.price.ToString() + " KLAY";
        }
    }
}