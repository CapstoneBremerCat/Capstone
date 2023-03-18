using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Inventory : MonoBehaviour
    {
        // Singleton instance
        private static Inventory instance;
        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Inventory>();
                }
                return instance;
            }
        }

        public static bool inventoryActivated = false;

        // �ʿ��� ������Ʈ
        [SerializeField]
        private GameObject go_InventoryBase;
        [SerializeField]
        private GameObject go_SlotsParent;

        // The currently equipped item
        private Item equippedItem;
        // ���Ե�
        [SerializeField] private Slot[] slots;

        // Start is called before the first frame update
        void Start()
        {
            slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryActivated = !inventoryActivated;

                if (inventoryActivated)
                    OpenInventory();
                else
                    CloseInventory();
            }
        }

        private void OpenInventory()
        {
            go_InventoryBase.SetActive(true);
        }
        private void CloseInventory()
        {
            go_InventoryBase.SetActive(false);
        }

        public void AcquireItem(Item _item, int _count = 1)
        {
            // If the acquired item is not a weapon, look for existing slots with the same item
            if (Item.ItemType.Weapon != _item.itemType)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    // If the slot already contains an item
                    if (slots[i].Item != null)
                    {
                        // If the item in the slot has the same name as the acquired item
                        if (slots[i].Item.name == _item.name)
                        {
                            // Increase the count of the item in the slot by the acquired count
                            slots[i].SetSlotCount(_count);
                            return;
                        }
                    }
                }
            }
            // If there are no slots with the acquired item, find an empty slot to add it
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item == null)
                {
                    // Add the acquired item with the acquired count to the empty slot
                    slots[i].AddItem(_item, _count);

                    // If the acquired item is a weapon and there is no equipped weapon, equip the weapon
                    if (Item.ItemType.Weapon == _item.itemType && EquipManager.Instance.EquippedWeapon == null)
                    {
                        EquipManager.Instance.Equip(_item);
                    }
                    return;
                }
            }
        }

        // �κ��丮 �ʱ�ȭ
        public void InitInventory()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].ClearSlot();
            }
        }

        public void SetEquippedItem(Item item)
        {
            // Update the equipped item
            equippedItem = item;

            // Update the equipped image for the appropriate slot
            for (int i = 0; i < slots.Length; i++)
            {
                Slot slot = slots[i];

                if (slot.Item == item)
                {
                    slot.SetEquipped(true);
                }
                else
                {
                    slot.SetEquipped(false);
                }
            }
        }
    }
}