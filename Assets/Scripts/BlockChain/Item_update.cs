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
using BlockChain;
namespace BlockChain
{
    public class Item_update : MonoBehaviour
    {
        /*
        [SerializeField]
        public GameObject shop;//�����θ� ������Ʈ
        public Item I_D;
        public Button button;
        public TMP_Text text;
        private float x;
        private float y;
        private float z;


        public Item Item { set { I_D = value; } }
        void Start()
        {
            shop = this.transform.parent.parent.parent.parent.gameObject;
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>(I_D.tokenId.ToString());
            this.GetComponent<Button>().onClick.AddListener(printitem_info);
            this.GetComponentInChildren<TMP_Text>().text = I_D.name;
            x = this.gameObject.transform.position.x;
            y = this.gameObject.transform.position.y;
            z = this.gameObject.transform.position.z;
            this.transform.position = new Vector3(x, y - 30, z);//�������� ��ġ�� �������� õ�忡 �����κ��� ���ֱ����� ����
        }
        public void printitem_info()
        {
            shop.GetComponent<shop>().sel_item_image.sprite = Resources.Load<Sprite>(I_D.tokenId.ToString());//������ �̹����� ���
            shop.GetComponent<shop>().sel_item_price.text = "price:" + I_D.price;
            //shop.GetComponent<shop>().sel_item_price.text = "price:" + I_D.Price.ToString();//�������� ������ ���
        }*/
    }
}