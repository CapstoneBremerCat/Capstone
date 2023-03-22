using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [System.Serializable]
    public class StatusData
    {
        public StatusData()
        {
            healthGauge = new Gauge();
            manaGauge = new Gauge();
            staminaGauge = new Gauge();
        }
        public Gauge healthGauge;    // hp Gauge
        public Gauge manaGauge;      // mp Gauge
        public Gauge staminaGauge;   // sp Gauge

        public float moveSpeed;     // move speed
        public float attackSpeed;   // attack speed

        public float damage;   // ATK
        public float armor;       // DEF

        public float coolTimeReduce;  // skill coolTime Reduce

        //***
        public float runSpeedMult = 1.5f;   // �޸��� �� �ӷ� ���
        public float runStamina = 0.2f;   // �޸��⿡ �Ҹ�Ǵ� ������ ��

        public float jumpPower = 2.5f;  // ���� ����

        public int exp = 0; // ����ġ

        public void AddStat(StatusData target)
        {
            if (target == null)
            {
                Debug.LogWarning("AddStat : Null");
                return;
            }

            healthGauge.maxValue += target.healthGauge.maxValue;
            healthGauge.regenValue += target.healthGauge.regenValue;
            manaGauge.maxValue += target.manaGauge.maxValue;
            manaGauge.regenValue += target.manaGauge.regenValue;
            staminaGauge.maxValue += target.staminaGauge.maxValue;
            staminaGauge.regenValue += target.staminaGauge.regenValue;
            moveSpeed += target.moveSpeed;
            attackSpeed += target.attackSpeed;
            damage += target.damage;
            armor += target.armor;
            coolTimeReduce += target.coolTimeReduce;
        }

        public void SubStat(StatusData target)
        {
            if (target == null)
            {
                Debug.LogWarning("SubStat : Null");
                return;
            }

            healthGauge.maxValue -= target.healthGauge.maxValue;
            healthGauge.regenValue -= target.healthGauge.regenValue;
            manaGauge.maxValue -= target.manaGauge.maxValue;
            manaGauge.regenValue -= target.manaGauge.regenValue;
            staminaGauge.maxValue -= target.staminaGauge.maxValue;
            staminaGauge.regenValue -= target.staminaGauge.regenValue;
            moveSpeed -= target.moveSpeed;
            attackSpeed -= target.attackSpeed;
            damage -= target.damage;
            armor -= target.armor;
            coolTimeReduce -= target.coolTimeReduce;
        }

    }
}