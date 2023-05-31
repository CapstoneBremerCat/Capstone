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
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;

            // Scene �̵� �� ���� ���� �ʵ��� ó��
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion
        private const string EQUIPPED_WEAPON_KEY = "equipped_weapon_id";

        private Transform weaponSocket;

        private Weapon equippedWeapon;
        public Weapon EquippedWeapon { get { return equippedWeapon; } }
        public int weaponUpgrade { get; private set; }
        public void InitUpgrade()
        {
            weaponUpgrade = 0;
            UIManager.Instance.SetDamageBuff(weaponUpgrade);
        }

        public void EquipLoadedWeapon()
        {
            // PlayerPrefs���� ����� �������� ���� ������ �ҷ���
            if (PlayerPrefs.HasKey(EQUIPPED_WEAPON_KEY))
            {
                int weaponId = PlayerPrefs.GetInt(EQUIPPED_WEAPON_KEY);
                EquipWeapon(ItemMgr.Instance.GetItemById(weaponId));
            }
        }

        public void UpgradeWeapon()
        {
            weaponUpgrade++;
            UIManager.Instance.SetDamageBuff(weaponUpgrade);
            Mediator.Instance.Notify(this, GameEvent.UPGRADED_WEAPON, this);
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
                // PlayerPrefs�� �������� ���� ���� ����
                PlayerPrefs.SetInt(EQUIPPED_WEAPON_KEY, weaponItem.itemCode);
                PlayerPrefs.Save();
            }
            SoundManager.Instance.OnPlaySFX("Equip");
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