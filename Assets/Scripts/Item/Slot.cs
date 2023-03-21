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
        private Vector3 originPos;

        [SerializeField] private Item item; // 획득 아이템
        [SerializeField] private Image itemImage; // 아이템의 이미지.
        public int itemCount { get; private set; } // 획득 아이템 개수


        // 필요한 컴포넌트
        [SerializeField] private Text text_Count;

        // 마우스 드래그가 끝났을 때 발생하는 이벤트
        private Rect baseRect;  // Inventory_Base 이미지의 Rect 정보 받아 옴.
        private Transform player;  // 아이템을 떨어트릴 위치.

        public Item Item { get { return item; } }


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
        public void AddItem(Item _item, int _count = 1)
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
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("if1완료");
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
            if (item != null && item.itemType != Item.ItemType.Weapon)
            {
                DragSlot.instance.dragSlot = this.gameObject;
                DragSlot.instance.DragSetImage(itemImage);
                DragSlot.instance.transform.position = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (item != null && item.itemType != Item.ItemType.Weapon)
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
                var slot = DragSlot.instance.dragSlot.GetComponent<Slot>();
                Instantiate(slot.item.itemPrefab, player.position + player.forward * 2 + new Vector3(0, 0.5f, 0),
                    Quaternion.identity);
                slot.ClearSlot();

            }
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
            Item _tempItem = item;
            int _tempItemCount = itemCount;
            var slot = DragSlot.instance.dragSlot.GetComponent<Slot>();
            if (!slot) return;
            AddItem(slot.item, slot.itemCount);

            if (_tempItem != null)
            {
                slot.AddItem(_tempItem, _tempItemCount);
            }
            else
            {
                slot.ClearSlot();
            }
        }

    }
}