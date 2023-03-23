using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Window : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectTransform; //움직일대상지정

    private void OnEnable()
    {
        dragRectTransform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
    }
}
