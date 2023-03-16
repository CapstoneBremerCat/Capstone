using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
namespace Game
{
    public class PartnerHUD : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;

        public void SetHealthBar(float ratio)
        {
            if (healthSlider) healthSlider.value = ratio;
        }
    }
}