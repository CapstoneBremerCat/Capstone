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
        public StatusData totalStat { get; private set; } // sum of all stats
        public float curHealth { get; private set; }    // current Hp
        public float curMana { get; private set; }      // current Mp
        public float curStamina { get; private set; }   // current Sp

        public float maxHealth { get { return totalStat.healthGauge.maxValue; } }
        public float maxMana { get { return totalStat.manaGauge.maxValue; } }
        public float maxStamina { get { return totalStat.staminaGauge.maxValue; } }

        // 초당 회복량
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
        public float totalDamage { get { return totalStat.damage + weaponDamage; } }
        public float damageReduction { get { return CalculateDamageReduction(); } }
        public float coolTimeReduce { get { return CalculateCoolTimeReduction(); } }
        public float jumpPower { get { return totalStat.jumpPower; } }
        public float runStamina { get { return totalStat.runStamina; } }

        private float weaponDamage;
        public ConditionType curCondition { get; private set; }    // ���� ����(�⺻, ħ��, ����)
        public bool isDead { get { return (0 >= curHealth); } }    // ���� ���� Ȯ��.
        public event Action OnDeath; // ��� �� �ߵ��� �̺�Ʈ

        protected virtual void OnEnable()
        {
            InitStatus();
        }

        // 캐릭터 상태 초기화
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

        public void SetWeaponDamage(float damage)
        {
            weaponDamage = damage;
        }
        private void Update()
        {
            // 매 프레임마다 회복시킨다면, 초당 회복량과 동일한 수치로 회복이 된다.
            RestoreHealth(totalStat.healthGauge.regenValue * Time.deltaTime);
            RestoreStamina(totalStat.staminaGauge.regenValue * Time.deltaTime);
        }
        public virtual void RestoreHealth(float value)
        {
            if (isDead) return;
            SetHealth(curHealth + value);
        }
        public virtual void RestoreStamina(float value)
        {
            if (isDead) return;
            SetStamina(curStamina + value);
        }
        public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (isDead) return; // If already dead, do not process any further.

            // Calculate actual damage by applying damage reduction.
            float actualDamage = damage * (100 - damageReduction) / 100;
            Debug.Log("ActualDamage: " + actualDamage + "reduced: -" + damageReduction);
            curHealth -= actualDamage;   // Reduce health by received damage amount.
            if (isDead) Die();  // If health becomes 0 or below, execute death handling.
        }

        // Function to calculate damage reduction based on defense stat using logarithmic function
        private float CalculateDamageReduction()
        {
            // Damage reduction calculated by function
            return totalStat.armor / (totalStat.armor + 100) * 100;
        }
        private float CalculateCoolTimeReduction()
        {
            // Damage reduction calculated by function
            return Mathf.Clamp(totalStat.coolTimeReduce, 0, 60);
        }

        private void Die()
        {
            if (null != OnDeath) OnDeath(); // 관련된 이벤트 처리를 실행한다.
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