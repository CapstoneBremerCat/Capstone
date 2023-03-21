using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Inventory : MonoBehaviour
    {
        public static bool inventoryActivated = false;

        // �ʿ��� ������Ʈ
        [SerializeField]
        private GameObject go_InventoryBase;
        [SerializeField]
        private GameObject go_SlotsParent;

        // ���Ե�
        [SerializeField] private Slot[] slots;

        // Start is called before the first frame update
        void Start()
        {
            slots = go_SlotsParent.GetComponentsInChildren<Slot>();
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_WEAPON, RemoveItemFronInventory);
            Mediator.Instance.RegisterEventHandler(GameEvent.ITEM_PICKED_UP, AcquireItem);
        }

        public void InitInventory()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].ClearSlot();
            }
        }

        private void AcquireItem(object itemObject)
        {
            AcquireItem(itemObject as Item);
        }

        public void AcquireItem(Item _item, int _count = 1)
        {
            //if the acquired item is not a weapon, looks for an existing slot that contains the same item.
            if (_item.itemType != Item.ItemType.Weapon)
            {
                Slot sameItemSlot = GetSameItemSlot(_item);
                if (sameItemSlot != null)
                {
                    // Increase the count of the item in the slot by the acquired count
                    sameItemSlot.SetSlotCount(_count);
                }
            }
            // find an empty slot to add it
            Slot emptySlot = GetEmptySlot();
            if (emptySlot == null) return;
            // Add the acquired item with the acquired count to the empty slot
            emptySlot.AddItem(_item, _count);
            // If the acquired item is a weapon and there is no equipped weapon, equip the weapon
            if (_item.itemType == Item.ItemType.Weapon && EquipManager.Instance.EquippedWeapon == null)
            {
                EquipManager.Instance.Equip(_item);
            }
        }

        private Slot GetEmptySlot()
        {
            foreach (Slot slot in slots)
            {
                if (slot.Item == null)
                {
                    return slot;
                }
            }
            return null;
        }

        private Slot GetSameItemSlot(Item _item)
        {
            foreach (Slot slot in slots)
            {
                if (slot.Item != null && slot.Item.name == _item.name)
                {
                    return slot;
                }
            }
            return null;
        }
        public void RemoveItemFronInventory(object itemObject)
        {
            // If there is no equipped weapon, return.
            if (!EquipManager.Instance.EquippedWeapon) return;
            // Otherwise, search for the item in the inventory slots and clear the slot.
            Item item = itemObject as Item;
            for (int i = 0; i < slots.Length; i++)
            {
                Slot slot = slots[i];

                if (slot.Item == item)
                {
                    slot.ClearSlot();
                }
            }
        }
        private void OnDisable()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_WEAPON, RemoveItemFronInventory);
            Mediator.Instance.UnregisterEventHandler(GameEvent.ITEM_PICKED_UP, AcquireItem);
        }
    }
}