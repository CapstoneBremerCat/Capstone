using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour, IDamageable
{
    public string unitName { get; private set; } // name
    public int level { get; private set; }   // level
    public List<int> itemList { get; private set; } // 적용중인 아이템코드 리스트

    [SerializeField] private StatusObject baseStat; // default Stat
    private StatusData totalStat; // sum of all stats
    public float curHealth { get; private set; }    // current Hp
    public float curMana { get; private set; }      // current Mp
    public float curStamina { get; private set; }   // current Sp

    public float maxHealth { get { return totalStat.healthGauge.maxValue; } }
    public float maxMana { get { return totalStat.manaGauge.maxValue; } }
    public float maxStamina { get { return totalStat.staminaGauge.maxValue; } }

    public float regenHealth { get { return totalStat.healthGauge.regenValue; } }
    public float regenMana { get { return totalStat.manaGauge.regenValue; } }
    public float regenStamina { get { return totalStat.staminaGauge.regenValue; } }

    public bool isHpFull { get { return totalStat.healthGauge.maxValue <= curHealth; } }
    public bool isHpZero { get { return 0 >= curHealth; } }    // 죽음 상태 확인.
    public bool isMpFull { get { return totalStat.manaGauge.maxValue <= curMana; } }
    public bool isMpZero { get { return 0 >= curMana; } }
    public bool isSpFull { get { return totalStat.staminaGauge.maxValue <= curStamina; } }
    public bool isSpZero { get { return 0 >= curStamina; } }

    public float moveSpeed { get { return totalStat.moveSpeed; } }
    public float attackSpeed { get { return totalStat.attackSpeed; } }

    public float attackPower { get { return totalStat.attackPower; } }
    public float defense { get { return totalStat.defense; } }

    public float damageIncrease { get { return totalStat.damageIncrease; } }
    public float damageReduction { get { return totalStat.damageReduction; } }
    public float jumpPower { get { return totalStat.jumpPower; } }
    public float runStamina { get { return totalStat.runStamina; } }

    public ConditionType curCondition { get; private set; }    // 현재 상태(기본, 침묵, 기절)
    public bool isDead { get { return (0 >= curHealth); } }    // 죽음 상태 확인.
    public event Action OnDeath; // 사망 시 발동할 이벤트

    protected virtual void OnEnable()
    {
        totalStat = baseStat.status;
        curHealth = maxHealth;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (isDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.

        curHealth -= damage;   // 데미지 만큼 체력 감소.
        if (isDead) Die();  // 데미지를 입어 체력이 0이하(사망 상태) 라면 사망 이벤트 실행.
    }
    private void Die()
    {
        if (null != OnDeath) OnDeath(); // 등록된 사망 이벤트 실행.
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
    public virtual void RestoreHealth(float value)
    {
        if (isDead) return; // 이미 죽은 상태에서는 체력회복 불가능.
        SetHealth(curHealth + value);
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
