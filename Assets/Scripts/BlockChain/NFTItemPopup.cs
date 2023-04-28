using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using BlockChain;

public class NFTItemPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemImage;

    public void SetNFTItem(string name, string description, Sprite image)
    {
        itemName.text = name;
        itemDescription.text = description;
        itemImage.sprite = image;
    }

    public void OpenPopup()
    {
        gameObject.SetActive(true);
    }
}
