using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerClickHandler , IBeginDragHandler , IDragHandler , IEndDragHandler , IDropHandler
{

    private Vector3 originPos;

    public Item item; // ȹ�� ������
    public int itemCount; // ȹ�� ������ ����
    public Image itemImage; // �������� �̹���.

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text text_Count;

    // ���� ����
    //private WeaponManager theWeaponManager;
    
    void Start()
    {
        originPos = transform.position;
        //theWeaponManager = FindObjectOfType<WeaponManager>();
    }


    // �̹����� ������ ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // ������ ȹ��
    public void AddItem(Item _item,int _count = 1)
    {
        item = _item;
        itemCount = _count;

        itemImage.sprite = item.itemImage;

        text_Count.text = itemCount.ToString();

        SetColor(1);

    }

    // ������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();
        if (itemCount <= 0)
            ClearSlot();
    }

    // ���� �ʱ�ȭ
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("click�Ϸ�");
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("if1�Ϸ�");
            if (item != null)
            {
                Debug.Log("if2�Ϸ�");
                if (item.itemType == Item.ItemType.Equipment)
                {
                    // ����
                    //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                    Debug.Log(item.itemName + " �� �����߽��ϴ�.");
                }
                else
                {
                    // �Ҹ�
                    Debug.Log(item.itemName + " �� ����߽��ϴ�.");
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