using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class ItemPickUp : MonoBehaviour
    {
        [SerializeField] private Item item;
        public Item Item { get { return item; } }
        [SerializeField] private int remainingTime;
        public bool isPickable { get; private set; }
        public event Action OnPickup; 

        void OnEnable()
        {
            isPickable = true;
            if (remainingTime != 0) StartCoroutine(SpawnRemainingRoutine());
        }

        private IEnumerator SpawnRemainingRoutine()
        {
            yield return new WaitForSeconds(remainingTime);
            this.gameObject.SetActive(false);
        }

        public void SetPickable(bool value)
        {
            isPickable = value;
        }
        public void Pickup()
        {
            if (null != OnPickup) OnPickup();
        }
    }
}