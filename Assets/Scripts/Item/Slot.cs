using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour , IPointerClickHandler , IBeginDragHandler , IDragHandler , IEndDragHandler , IDropHandler
{
    private Vector3 originPos;

    [SerializeField] private Item item; // ȹ�� ������
    [SerializeField] private Image itemImage; // �������� �̹���.
    [SerializeField] private Image equippedImage; // ���� ���� �������� ǥ���ϱ� ���� �̹���
    public int itemCount { get; private set; } // ȹ�� ������ ����


    // �ʿ��� ������Ʈ
    [SerializeField] private Text text_Count;

    // ���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
    private Rect baseRect;  // Inventory_Base �̹����� Rect ���� �޾� ��.
    private Transform player;  // �������� ����Ʈ�� ��ġ.

    public Item Item { get { return item; } }


    // ���� ����
    //private WeaponManager theWeaponManager;

    void Start()
    {
        originPos = transform.position;
        //theWeaponManager = FindObjectOfType<WeaponManager>();
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        player = GameObject.FindWithTag("Player").transform;
    }


    // �̹����� ���� ����
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

        //text_Count.text = itemCount.ToString();

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
        Debug.Log("click�Ϸ�");
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("if1�Ϸ�");
            if (item != null)
            {
                UseItem(item);
            }
        }
    }

    public void UseItem(Item item)
    {
        switch (item.itemType)
        {
            case Item.ItemType.Weapon:
                EquipManager.Instance.EquipWeapon(item);
                break;
            case Item.ItemType.Used:
                // Use recovery item
                break;
            case Item.ItemType.Ammo:
                // Add ammo to inventory
                break;
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

    public void SetEquipped(bool isEquipped)
    {
        if(equippedImage) equippedImage.enabled = isEquipped;
    }
}
