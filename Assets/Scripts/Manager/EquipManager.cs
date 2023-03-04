using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance { get; private set; }

    [SerializeField]
    private Gun[] guns;  // an arrangement with all kinds of guns as elements

    // Use a Dictionary data structure to provide easy access to weapons by name from a management perspective.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();

    [SerializeField] private Transform weaponSocket;
    // The inventory to update when equipping/unequipping weapons
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerShooter playerShooter;

    private Gun equippedWeapon;
    public Gun EquippedWeapon { get { return equippedWeapon; } }

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

    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].GunName, guns[i]);
        }
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
        Gun weaponPrefab = gunDictionary[weaponItem.itemName];
        if(!weaponPrefab)
        {
            Debug.Log("Can't found weapon from Dictionary");
            return;
        }
        if (equippedWeapon && weaponPrefab.GunName == equippedWeapon.GunName)
        {
            // Clicked on the currently equipped weapon, so unequip it
            if(inventory) inventory.SetEquippedItem(null);
            UnequipWeapon();
            playerShooter.SetGun(null);
        }
        else
        {
            // Unequip the previous one  and equip the new weapon
            UnequipWeapon();
            equippedWeapon = weaponPrefab;
            equippedWeapon = Instantiate(weaponPrefab, weaponSocket);
            equippedWeapon.SetOriginPos(weaponPrefab.WeaponOffset.localPosition);
            equippedWeapon.transform.localPosition = weaponPrefab.WeaponOffset.localPosition;
            equippedWeapon.transform.localRotation = weaponPrefab.WeaponOffset.localRotation;
            if (inventory) inventory.SetEquippedItem(weaponItem);
            playerShooter.SetGun(equippedWeapon);
        }
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