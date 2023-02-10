using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerClickHandler , IBeginDragHandler , IDragHandler , IEndDragHandler , IDropHandler
{

    private Vector3 originPos;

    public Item item; // 획득 아이템
    public int itemCount; // 획득 아이템 개수
    public Image itemImage; // 아이템의 이미지.

    // 필요한 컴포넌트
    [SerializeField]
    private Text text_Count;

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    private Rect baseRect;  // Inventory_Base 이미지의 Rect 정보 받아 옴.
    private Transform player;  // 아이템을 떨어트릴 위치.

    // 무기 관리
    //private WeaponManager theWeaponManager;

    void Start()
    {
        originPos = transform.position;
        //theWeaponManager = FindObjectOfType<WeaponManager>();
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        player = GameObject.FindWithTag("Player").transform;
    }


    // 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
    public void AddItem(Item _item,int _count = 1)
    {
        item = _item;
        itemCount = _count;

        itemImage.sprite = item.itemImage;

        //text_Count.text = itemCount.ToString();

        SetColor(1);

    }

    // 아이템 개수 조절
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        text_Count.text = "";
        itemImage.sprite = null;
        SetColor(0);

    }



    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click완료");
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("if1완료");
            if (item != null)
            {
                Debug.Log("if2완료");
                if (item.itemType == Item.ItemType.Equipment)
                {
                    // 장착
                    //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                    Debug.Log(item.itemName + " 을 장착했습니다.");
                }
                else
                {
                    // 소모
                    Debug.Log(item.itemName + " 을 사용했습니다.");
                    SetSlotCount(-1);

                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragSlot.instance.transform.localPosition.x < baseRect.xMin
    || DragSlot.instance.transform.localPosition.x > baseRect.xMax
    || DragSlot.instance.transform.localPosition.y < baseRect.yMin
    || DragSlot.instance.transform.localPosition.y > baseRect.yMax)
        {
            Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, player.position + player.forward * 2 + new Vector3(0, 0.5f, 0),
                Quaternion.identity);
            DragSlot.instance.dragSlot.ClearSlot();

        }
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if(_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

}
