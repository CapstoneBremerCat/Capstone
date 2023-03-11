using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance { get; private set; }

    [SerializeField]
    private Weapon[] weapons;  // an arrangement with all kinds of weapons as elements

    // Use a Dictionary data structure to provide easy access to weapons by name from a management perspective.
    private Dictionary<string, Weapon> weaponDictionary = new Dictionary<string, Weapon>();

    [SerializeField] private Transform weaponSocket;
    // The inventory to update when equipping/unequipping weapons
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerShooter playerShooter;

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

    void Start()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weaponDictionary.Add(weapons[i].WeaponName, weapons[i]);
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
        Weapon weaponPrefab = weaponDictionary[weaponItem.name];
        if(!weaponPrefab)
        {
            Debug.Log("Can't found weapon from Dictionary");
            return;
        }
        if (equippedWeapon && weaponPrefab.WeaponName == equippedWeapon.WeaponName)
        {
            // Clicked on the currently equipped weapon, so unequip it
            if(inventory) inventory.SetEquippedItem(null);
            UnequipWeapon();
            playerShooter.SetWeapon(null);
        }
        else
        {
            // Unequip the previous one  and equip the new weapon
            UnequipWeapon();
            //equippedWeapon = weaponPrefab;
            equippedWeapon = Instantiate(weaponPrefab, weaponSocket);
            if (inventory) inventory.SetEquippedItem(weaponItem);
            playerShooter.SetWeapon(equippedWeapon);
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