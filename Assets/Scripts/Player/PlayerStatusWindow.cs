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

        private void Start()
        {
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_PASSIVE, RefreshStatusUI);
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshStatusUI);
        }

        public void RefreshStatusUI(object playerObject)
        {
            Player player = playerObject as Player;
            texts[0].text = player.totalStat.healthGauge.maxValue.ToString();
            texts[1].text = player.totalStat.healthGauge.regenValue.ToString();
            texts[2].text = player.totalStat.staminaGauge.maxValue.ToString();
            texts[3].text = player.totalStat.staminaGauge.regenValue.ToString();
            texts[4].text = player.totalStat.moveSpeed.ToString();
            texts[5].text = player.totalStat.attackSpeed.ToString();
            texts[6].text = player.totalStat.attackPower.ToString();
            texts[7].text = player.totalStat.defense.ToString();
        }

        private void OnDestroy()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_PASSIVE, RefreshStatusUI);
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_SKILL, RefreshStatusUI);
        }
    }
}