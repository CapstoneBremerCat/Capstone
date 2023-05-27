using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    private Stack<GameObject> popupStack;

    private void Start()
    {
        popupStack = new Stack<GameObject>();
    }
    public bool IsPopupEmpty()
    {
        return (popupStack.Count == 0);
    }


    public void ClearPopup()
    {
        popupStack.Clear();
    }

    public void OpenPopup(GameObject popup)
    {
        popup.SetActive(true);
        popup.transform.SetAsLastSibling();
        popupStack.Push(popup);
    }

    public void ClosePopup()
    {
        if (popupStack.Count > 0)
        {
            GameObject currentPopup = popupStack.Pop();
            currentPopup.SetActive(false);

            if (popupStack.Count > 0)
            {
                GameObject previousPopup = popupStack.Peek();
                previousPopup.SetActive(true);
            }
        }
    }

    public void RemovePopup(GameObject popup)
    {
        if (popupStack.Contains(popup))
        {
            Stack<GameObject> tempStack = new Stack<GameObject>();

            while (popupStack.Peek() != popup)
            {
                tempStack.Push(popupStack.Pop());
            }

            popupStack.Pop();
            popup.SetActive(false);

            while (tempStack.Count > 0)
            {
                GameObject tempPopup = tempStack.Pop();
                popupStack.Push(tempPopup);
            }

            if (popupStack.Count > 0)
            {
                GameObject previousPopup = popupStack.Peek();
                previousPopup.SetActive(true);
            }
        }
    }
}
