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
        private float range; // ���� ������ �ִ�Ÿ�.
        private bool pickupActivated = false; // ���� ������ �� true.
        private RaycastHit hitInfo; // �浹ü ���� ����.

        // ������ ���̾�� �����ϵ��� ���̾� ����ũ�� ����.
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
                    Mediator.Instance.Notify(this, GameEvent.ITEM_PICKED_UP, hitInfo.transform.GetComponent<ItemPickUp>().Item);
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

            string itemName = hitInfo.transform.GetComponent<ItemPickUp>().Item.name;
            string formattedString = string.Format("{0} ȹ�� <color=yellow>(E)</color>", itemName);

            UIManager.Instance.DisplayItemInfoText(formattedString);
        }

        private void InfoDisappear()
        {
            pickupActivated = false;
            UIManager.Instance.DisableItemInfoText();
        }
    }
}