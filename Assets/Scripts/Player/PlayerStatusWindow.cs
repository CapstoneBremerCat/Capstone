using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game;
namespace Game
{
    public class PlayerStatusWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] texts;
        // Start is called before the first frame update

        private void OnEnable()
        {
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_PASSIVE, RefreshStatusUI);
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshStatusUI);
            Mediator.Instance.RegisterEventHandler(GameEvent.REFRESH_STATUS, RefreshStatusUI);
        }

        public void RefreshStatusUI(object playerObject)
        {
            Player player = playerObject as Player;
            texts[0].text = player.maxHealth.ToString();
            texts[1].text = string.Format("{0}/sec", player.regenHealth);
            texts[2].text = player.maxStamina.ToString();
            texts[3].text = string.Format("{0}/sec", player.regenStamina);
            texts[4].text = player.totalDamage.ToString();
            texts[5].text = string.Format("{0} (-{1:F1}%)", player.totalStat.armor, player.damageReduction);
            texts[6].text = string.Format("+{0:F1}%", player.attackSpeed);
            texts[7].text = string.Format("-{0:F1}%", player.coolTimeReduce);
        }

        private void OnDisable()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_PASSIVE, RefreshStatusUI);
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshStatusUI);
            Mediator.Instance.UnregisterEventHandler(GameEvent.REFRESH_STATUS, RefreshStatusUI);
        }
    }
}