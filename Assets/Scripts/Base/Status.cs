using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class Status : MonoBehaviour, IDamageable
    {
        public string unitName { get; private set; } // name
        public int level { get; private set; }   // level

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
        public bool isHpZero { get { return 0 >= curHealth; } }    // ���� ���� Ȯ��.
        public bool isMpFull { get { return totalStat.manaGauge.maxValue <= curMana; } }
        public bool isMpZero { get { return 0 >= curMana; } }
        public bool isSpFull { get { return totalStat.staminaGauge.maxValue <= curStamina; } }
        public bool isSpZero { get { return 0 >= curStamina; } }
        public bool isGodMode { get; private set; }
        public float moveSpeed { get { return totalStat.moveSpeed; } }
        public float attackSpeed { get { return totalStat.attackSpeed; } }

        public float attackPower { get { return totalStat.attackPower; } }
        public float defense { get { return totalStat.defense; } }

        public float damageIncrease { get { return totalStat.damageIncrease; } }
        public float damageReduction { get { return totalStat.damageReduction; } }
        public float jumpPower { get { return totalStat.jumpPower; } }
        public float runStamina { get { return totalStat.runStamina; } }

        public ConditionType curCondition { get; private set; }    // ���� ����(�⺻, ħ��, ����)
        public bool isDead { get { return (0 >= curHealth); } }    // ���� ���� Ȯ��.
        public event Action OnDeath; // ��� �� �ߵ��� �̺�Ʈ

        protected virtual void OnEnable()
        {
            InitStatus();
        }

        public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (isDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.

            curHealth -= damage;   // ������ ��ŭ ü�� ����.
            if (isDead) Die();  // �������� �Ծ� ü���� 0����(��� ����) ��� ��� �̺�Ʈ ����.
        }
        private void Die()
        {
            if (null != OnDeath) OnDeath(); // ��ϵ� ��� �̺�Ʈ ����.
        }

        // �����۵� �ʱ�ȭ�� �ؾ��ұ�? �������� ��������?
        public void InitStatus()
        {
            totalStat = new StatusData();
            if (baseStat) totalStat.AddStat(baseStat.status);

            SetHealth(totalStat.healthGauge.maxValue);
            SetMana(totalStat.manaGauge.maxValue);
            SetStamina(totalStat.staminaGauge.maxValue);

            curCondition = ConditionType.Default;
            OffGodMode();
        }
        public virtual void RestoreHealth(float value)
        {
            if (isDead) return; // �̹� ���� ���¿����� ü��ȸ�� �Ұ���.
            SetHealth(curHealth + value);
        }
        public virtual void RestoreStamina(float value)
        {
            if (isDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.
            SetStamina(curStamina + value);
        }
        
        public void ApplyStatus(StatusData status)
        {
            if (totalStat != null && status != null)
            {
                totalStat.AddStat(status);
                curHealth += status.healthGauge.maxValue;
                curStamina += status.staminaGauge.maxValue;
                curMana += status.manaGauge.maxValue;
            }
        }

        public void RemoveStatus(StatusData status)
        {
            if (totalStat != null && status != null)
            {
                totalStat.SubStat(status);
                curHealth = Mathf.Min(curHealth, totalStat.healthGauge.maxValue);
                curStamina = Mathf.Min(curHealth, totalStat.staminaGauge.maxValue);
                curMana = Mathf.Min(curHealth, totalStat.manaGauge.maxValue);
            }
        }

        public void OnGodMode()
        {
            isGodMode = true;
        }
        public void OffGodMode()
        {
            isGodMode = false;
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
        public virtual bool UseStamina(float value)
        {
            var stamina = curStamina;
            if (stamina < value) return false; // ������ ������ ��� false ��ȯ �� ����.(���¹̳� ���� �޽��� ȣ��)
            SetStamina(stamina - value);
            return true;
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
    }
}