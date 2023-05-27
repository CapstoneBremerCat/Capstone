using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
namespace Game
{
    public class ActionController : MonoBehaviour
    {
        [SerializeField]
        private float range; // 습득 가능한 최대거리.
        private bool pickupActivated = false; // 습득 가능할 시 true.
        private RaycastHit hitInfo; // 충돌체 정보 저장.

        // 아이템 레이어에만 반응하도록 레이어 마스크를 설정.
        [SerializeField]
        private LayerMask layerMask;

        // Update is called once per frame
        void Update()
        {
            CheckItem();
            TryAction();
        }

        private void TryAction()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CheckItem();
                CanPickUp();
            }
        }

        private void CanPickUp()
        {
            if (pickupActivated)
            {
                if (hitInfo.transform != null)
                {
                    var pickup = hitInfo.transform.GetComponent<ItemPickUp>();
                    pickup.Pickup();
                    Mediator.Instance.Notify(this, GameEvent.ITEM_PICKED_UP, pickup.Item);
                    Destroy(hitInfo.transform.gameObject);
                    InfoDisappear();

                }
            }
        }

        private void CheckItem()
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
            {
                if (hitInfo.transform.tag == "Item")
                {
                    ItemInfoAppear();
                }
            }
            else
                InfoDisappear();
        }

        private void ItemInfoAppear()
        {
            pickupActivated = true;
            var pickup = hitInfo.transform.GetComponent<ItemPickUp>();
            string itemName = pickup.Item.name;
            string formattedString = string.Format("{0} 획득 <color=yellow>(E)</color>", itemName);

            UIManager.Instance.DisplayItemInfoText(formattedString);
        }

        private void InfoDisappear()
        {
            pickupActivated = false;
            UIManager.Instance.DisableItemInfoText();
        }
    }
}