using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{

    public class EquipManager : MonoBehaviour
    {
        #region instance
        private static EquipManager instance = null;
        public static EquipManager Instance { get { return instance; } }

        private void Awake()
        {
            // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance를 유일 오브젝트로 만든다
            instance = this;

            // Scene 이동 시 삭제 되지 않도록 처리
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
        private const string EQUIPPED_WEAPON_KEY = "equipped_weapon_id";

        private Transform weaponSocket;

        private Weapon equippedWeapon;
        public Weapon EquippedWeapon { get { return equippedWeapon; } }

        public void EquipLoadedWeapon()
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
                equippedWeapon = null;
            }

        }
    }
}