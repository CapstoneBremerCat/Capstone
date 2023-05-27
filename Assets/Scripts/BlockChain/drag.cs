using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BlockChain;
using Game;
namespace BlockChain
{
    public class Drag : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform dragRectTransform; //�����ϴ������
        [SerializeField] private GameObject window; // â ������Ʈ
        [SerializeField] private Canvas canvas;//ĵ��������

        public void OnPointerDown(PointerEventData eventData)
        {
            UIManager.Instance.RemovePopup(window);
            UIManager.Instance.OpenPopup(window);
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;//�巡��
            canvas.sortingOrder++;
        }
    }
}