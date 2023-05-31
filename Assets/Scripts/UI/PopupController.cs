using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

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
        // 마우스 활성화
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
            else if (GameManager.Instance.isGameStart)
            {
                // 창이 모두 닫혀있고, 게임 중일 경우 마우스 비활성화
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
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
            else if (GameManager.Instance.isGameStart)
            {
                // 창이 모두 닫혀있고, 게임 중일 경우 마우스 비활성화
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}
