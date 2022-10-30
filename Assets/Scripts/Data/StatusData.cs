using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float attackPower;   // ATK
    public float defense;       // DEF

    public float damageIncrease;   // amount of damage increased.
    public float damageReduction;  // amount of damage you receive.

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

        moveSpeed += target.moveSpeed;
        attackSpeed += target.attackSpeed;
        attackPower += target.attackPower;
        defense += target.defense;
        damageIncrease += target.damageIncrease;
        damageReduction += target.damageReduction;
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

        moveSpeed -= target.moveSpeed;
        attackSpeed -= target.attackSpeed;
        attackPower -= target.attackPower;
        defense -= target.defense;
        damageIncrease -= target.damageIncrease;
        damageReduction -= target.damageReduction;
    }

}