using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Game;
namespace Game
{
    public class Window : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private RectTransform dragRectTransform; //�����ϴ������

        private void OnEnable()
        {
            SoundManager.Instance.OnPlaySFX("Window");
            dragRectTransform.SetAsLastSibling();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragRectTransform.SetAsLastSibling();
        }
    }
}
