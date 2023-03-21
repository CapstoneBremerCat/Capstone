using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IPointerDownHandler
{
    private List<RectTransform> dragRectTransforms = new List<RectTransform>();

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform clickedRectTransform = eventData.pointerPress.GetComponent<RectTransform>();
        if (clickedRectTransform != null && !dragRectTransforms.Contains(clickedRectTransform))
        {
            dragRectTransforms.Add(clickedRectTransform);
            BringToFront(clickedRectTransform);
        }
    }

    private void BringToFront(RectTransform rectTransform)
    {
        rectTransform.SetAsLastSibling();
    }
}
