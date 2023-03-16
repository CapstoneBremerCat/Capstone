using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
    public class Item : ScriptableObject
    {
        public int itemCode;
        public ItemType itemType; // �������� ����.
        public Sprite itemImage; // �������� �̹���.
        public GameObject itemPrefab; // �������� ������.

        public enum ItemType
        {
            Weapon,
            Used,
            Ammo,
            ETC
        }
    }
}
