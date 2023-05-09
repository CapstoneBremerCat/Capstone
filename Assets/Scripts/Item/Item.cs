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
        public ItemType itemType; // 아이템의 유형.
        public Sprite itemImage; // 아이템의 이미지.
        public GameObject itemPrefab; // 아이템의 프리팹.

        public enum ItemType
        {
            Weapon,
            Used,
            ETC,
            Partner
        }
    }
}
