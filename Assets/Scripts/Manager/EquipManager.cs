using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{

    public class EquipManager : MonoBehaviour
    {
        private const string EQUIPPED_WEAPON_KEY = "equipped_weapon_id";
        public static EquipManager Instance { get; private set; }

        private Transform weaponSocket;

        private Weapon equippedWeapon;
        public Weapon EquippedWeapon { get { return equippedWeapon; } }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // PlayerPrefs에서 저장된 장착중인 무기 정보를 불러옴
            if (PlayerPrefs.HasKey(EQUIPPED_WEAPON_KEY))
            {
                int weaponId = PlayerPrefs.GetInt(EQUIPPED_WEAPON_KEY);
                EquipWeapon(ItemMgr.Instance.GetItemById(weaponId));
            }
        }

        public void SetWeaponSocket(Transform socketTransform)
        {
            weaponSocket = socketTransform;
        }

        public void Equip(Item item)
        {
            if (item.itemType == Item.ItemType.Weapon)
            {
                EquipWeapon(item);

            }
        }

        public void EquipWeapon(Item weaponItem)
        {
            Weapon weaponPrefab = ItemMgr.Instance.GetWeaponById(weaponItem.itemCode);
            if (!weaponPrefab)
            {
                Debug.Log("Can't found weapon from Dictionary");
                return;
            }
            if (equippedWeapon && weaponPrefab.weaponId == equippedWeapon.weaponId)
            {
                // Clicked on the currently equipped weapon, so unequip it
                UnequipWeapon();
            }
            else
            {
                // Unequip the previous one  and equip the new weapon
                UnequipWeapon();
                equippedWeapon = Instantiate(weaponPrefab, weaponSocket);
                // PlayerPrefs에 장착중인 무기 정보 저장
                PlayerPrefs.SetInt(EQUIPPED_WEAPON_KEY, weaponItem.itemCode);
                PlayerPrefs.Save();
            }
            Mediator.Instance.Notify(this, GameEvent.EQUIPPED_WEAPON, weaponItem);
        }

        public void UnequipWeapon()
        {
            // Destroy the current weapon, if any
            if (equippedWeapon != null)
            {
                Destroy(equippedWeapon.gameObject);
                //equippedWeapon.gameObject.SetActive(false);
                equippedWeapon = null;
            }
        }

    }
}