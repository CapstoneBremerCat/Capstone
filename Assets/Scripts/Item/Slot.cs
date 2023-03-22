using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Game;
namespace Game
{
    public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Item item; // ȹ�� ������
        [SerializeField] private Image itemImage; // �������� �̹���.
        public int itemCount { get; private set; } // ȹ�� ������ ����
        [SerializeField] private SlotType slotType;    // Slot type

        // �ʿ��� ������Ʈ
        [SerializeField] private Text text_Count;

        // ���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
        private Rect baseRect;  // Inventory_Base �̹����� Rect ���� �޾� ��.
        private Transform player;  // �������� ����Ʈ�� ��ġ.

        public Item Item { get { return item; } }

        void Start()
        {
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
        public void AddItem(Item _item, int _count = 1)
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
            if (eventData.button == PointerEventData.InputButton.Right)
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
                DragSlot.instance.dragSlot = this.gameObject;
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
/*            if (DragSlot.instance.transform.position.x < baseRect.xMin
        || DragSlot.instance.transform.position.x > baseRect.xMax
        || DragSlot.instance.transform.position.y < baseRect.yMin
        || DragSlot.instance.transform.position.y > baseRect.yMax)
            {
                var slot = DragSlot.instance.dragSlot.GetComponent<Slot>();
                Instantiate(slot.item.itemPrefab, player.position + player.forward * 2 + new Vector3(0, 0.5f, 0),
                    Quaternion.identity);
                slot.ClearSlot();

            }*/
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (DragSlot.instance.dragSlot != null)
            {
                ChangeSlot();
            }
        }

        private void ChangeSlot()
        {
            Item tempItem = item;
            int tempItemCount = itemCount;
            var slot = DragSlot.instance.dragSlot.GetComponent<Slot>();
            if (!slot) return;

            // When the slot types are not the same,
            // or when the dragged item is not a weapon,
            // or when an existing item exists and it is not a weapon, return
            if (this.slotType != slot.slotType &&
                (slot.item.itemType != Item.ItemType.Weapon || tempItem != null && tempItem.itemType != Item.ItemType.Weapon))
            {
                return;
            }

            AddItem(slot.item, slot.itemCount);

            // When an existing item exists
            if (tempItem != null)
            {
                // When the item is a weapon
                if (tempItem.itemType == Item.ItemType.Weapon && slot.item.itemType == Item.ItemType.Weapon)
                {
                    // Inventory -> Equipment window
                    if (slot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                    {
                        EquipManager.Instance.UnequipWeapon();
                        EquipManager.Instance.EquipWeapon(slot.item);
                    }
                    // Equipment window -> Inventory
                    else if (slot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                    {
                        EquipManager.Instance.UnequipWeapon();
                        EquipManager.Instance.EquipWeapon(tempItem);
                    }
                }
                slot.AddItem(tempItem, tempItemCount);
            }
            // When an existing item does not exist
            else
            {
                // When the item is a weapon
                if (slot.item.itemType == Item.ItemType.Weapon)
                {
                    // Inventory -> Equipment window
                    if (slot.slotType == SlotType.Inventory && slotType == SlotType.EquipWindow)
                    {
                        EquipManager.Instance.EquipWeapon(slot.item);
                    }
                    // Equipment window -> Inventory
                    else if (slot.slotType == SlotType.EquipWindow && slotType == SlotType.Inventory)
                    {
                        EquipManager.Instance.UnequipWeapon();
                    }
                }
                slot.ClearSlot();
            }
        }

    }
}