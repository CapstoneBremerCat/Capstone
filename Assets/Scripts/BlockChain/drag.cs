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
        [SerializeField] private RectTransform dragRectTransform; //움직일대상지정
        [SerializeField] private GameObject window; // 창 오브젝트
        [SerializeField] private Canvas canvas;//캔버스지정

        public void OnPointerDown(PointerEventData eventData)
        {
            UIManager.Instance.RemovePopup(window);
            UIManager.Instance.OpenPopup(window);
        }

        public void OnDrag(PointerEventData eventData)
        {
            dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;//드래그
            canvas.sortingOrder++;
        }
    }
}