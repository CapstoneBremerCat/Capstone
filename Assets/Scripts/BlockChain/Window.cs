using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Window : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectTransform; //�����ϴ������

    private void OnEnable()
    {
        dragRectTransform.SetAsLastSibling();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        dragRectTransform.SetAsLastSibling();
    }
}
