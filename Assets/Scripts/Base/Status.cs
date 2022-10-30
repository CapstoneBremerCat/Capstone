using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public string unitName { get; private set; } // name
    public int level { get; private set; }   // level
    public List<int> itemList { get; private set; } // 적용중인 아이템코드 리스트

    [SerializeField] private StatusObject baseStat; // default Stat
    private StatusData totalStat; // sum of all stats
    public float curHealth { get; private set; }    // current Hp
    public float curMana { get; private set; }      // current Mp
    public float curStamina { get; private set; }   // current Sp

    public float MaxHealth { get { return totalStat.healthGauge.maxValue; } }
    public float MaxMana { get { return totalStat.manaGauge.maxValue; } }
    public float MaxStamina { get { return totalStat.staminaGauge.maxValue; } }

    public float RegenHealth { get { return totalStat.healthGauge.regenValue; } }
    public float RegenMana { get { return totalStat.manaGauge.regenValue; } }
    public float RegenStamina { get { return totalStat.staminaGauge.regenValue; } }

    public bool IsHpFull { get { return totalStat.healthGauge.maxValue <= curHealth; } }
    public bool IsHpZero { get { return 0 >= curHealth; } }    // 죽음 상태 확인.
    public bool IsMpFull { get { return totalStat.manaGauge.maxValue <= curMana; } }
    public bool IsMpZero { get { return 0 >= curMana; } }
    public bool IsSpFull { get { return totalStat.staminaGauge.maxValue <= curStamina; } }
    public bool IsSpZero { get { return 0 >= curStamina; } }

    public float MoveSpeed { get { return totalStat.moveSpeed; } }
    public float AttackSpeed { get { return totalStat.attackSpeed; } }

    public float AttackPower { get { return totalStat.attackPower; } }
    public float Defense { get { return totalStat.defense; } }

    public float DamageIncrease { get { return totalStat.damageIncrease; } }
    public float DamageReduction { get { return totalStat.damageReduction; } }
    public float JumpPower { get { return totalStat.jumpPower; } }
    public float RunStamina { get { return totalStat.runStamina; } }

    public ConditionType curCondition { get; private set; }    // 현재 상태(기본, 침묵, 기절)

    private void Awake()
    {
        totalStat = baseStat.status;
    }

    // 아이템도 초기화를 해야할까? 영구적인 아이템은?
    public void InitStatus()
    {
        if (baseStat) totalStat = baseStat.status;

        SetHealth(totalStat.healthGauge.maxValue);
        SetMana(totalStat.manaGauge.maxValue);
        SetStamina(totalStat.staminaGauge.maxValue);

        curCondition = ConditionType.Default;
    }
    
    // sum of itemStats & unitStat
    public void SetTotalStat(StatusData StatusObject)
    {
        if (totalStat != null)
        {
            if (StatusObject != null) totalStat.AddStat(StatusObject);
            if (itemList != null) totalStat.AddStat(ItemMgr.Instance.GetTotalItemStats(itemList));
        }
    }
    
    private void InitItemList()
    {
        itemList.Clear();
    }
    
    public void AddItemCodeToList(int itemCode)
    {
        itemList.Add(itemCode);
    }
    public void RemoveItemCodeToList(int itemCode)
    {
        itemList.Remove(itemCode);
    }
    
    public float GetHpRatio()
    {
        return curHealth / totalStat.healthGauge.maxValue;
    }
    public float GetMpRatio()
    {
        return curMana / totalStat.manaGauge.maxValue;
    }
    public float GetStaminaRatio()
    {
        return curStamina / totalStat.staminaGauge.maxValue;
    }
    
    public virtual void SetHealth(float value)
    {
        float re;
        SetValue(value, totalStat.healthGauge.maxValue, out re);
        curHealth = re;
    }
    public virtual void SetMana(float value)
    {
        float re;
        SetValue(value, totalStat.manaGauge.maxValue, out re);
        curMana = re;
    }
    public virtual void SetStamina(float value)
    {
        float re;
        SetValue(value, totalStat.staminaGauge.maxValue, out re);
        curStamina = re;
    }

    private void SetValue(float value, float max, out float target)
    {
        target = Mathf.Clamp(value, 0, max);
    }

    private void Clear()
    {
        if (baseStat != null) totalStat = baseStat.status;
        else Debug.LogWarning("Status : Can't found baseStat");
        InitItemList();
        curCondition = ConditionType.Default;
    }
}
