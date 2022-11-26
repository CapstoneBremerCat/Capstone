using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour //, IDamageable
{
    [SerializeField] private Status status;

    private bool IsDead { get { return status.isHpZero; } }

    public event Action OnDeath; // 사망 시 발동할 이벤트

    private void Die()
    {
        if (null != OnDeath) OnDeath(); // 등록된 사망 이벤트 실행.
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.

        status.SetHealth(status.curHealth - damage);
        if (IsDead) Die();  // 데미지를 입어 체력이 0이하(사망 상태) 라면 사망 이벤트 실행.
    }

    public virtual bool UseMana(float value)
    {
        var mana = status.curMana;
        if (mana < value) return false; // 마나가부족할 경우 false 반환 후 리턴.(마나 부족 메시지 호출)
        status.SetMana(mana - value);
        return true;
    }

    public virtual bool UseStamina(float value)
    {
        var stamina = status.curStamina;
        if (stamina < value) return false; // 마나가 부족할 경우 false 반환 후 리턴.(스태미나 부족 메시지 호출)
        status.SetStamina(stamina - value);
        return true;
    }

    public virtual void RestoreHealth(float value)
    {
        if (IsDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.
        status.SetHealth(status.curHealth + value);
    }
    
    public virtual void RestoreMana(float value)
    {
        if (IsDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.
        status.SetMana(status.curMana + value);
    }
    
    public virtual void RestoreStamina(float value)
    {
        if (IsDead) return; // 이미 죽은 상태라면 더 이상 처리하지 않는다.
        status.SetStamina(status.curStamina + value);
    }

}