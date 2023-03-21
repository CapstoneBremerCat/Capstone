using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BlockChain;
namespace BlockChain
{
    public class Drag : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform dragRectTransform; //�����ϴ������
        [SerializeField] private Canvas canvas;//ĵ��������

        public void OnPointerDown(PointerEventData eventData)
        {
            dragRectTransform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;//�巡��
            canvas.sortingOrder++;
        }
    }
}