using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour //, IDamageable
{
    [SerializeField] private Status status;

    private bool IsDead { get { return status.IsHpZero; } }

    public event Action OnDeath; // ��� �� �ߵ��� �̺�Ʈ

    private void Die()
    {
        if (null != OnDeath) OnDeath(); // ��ϵ� ��� �̺�Ʈ ����.
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.

        status.SetHealth(status.curHealth - damage);
        if (IsDead) Die();  // �������� �Ծ� ü���� 0����(��� ����) ��� ��� �̺�Ʈ ����.
    }

    public virtual bool UseMana(float value)
    {
        var mana = status.curMana;
        if (mana < value) return false; // ������������ ��� false ��ȯ �� ����.(���� ���� �޽��� ȣ��)
        status.SetMana(mana - value);
        return true;
    }

    public virtual bool UseStamina(float value)
    {
        var stamina = status.curStamina;
        if (stamina < value) return false; // ������ ������ ��� false ��ȯ �� ����.(���¹̳� ���� �޽��� ȣ��)
        status.SetStamina(stamina - value);
        return true;
    }

    public virtual void RestoreHealth(float value)
    {
        if (IsDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.
        status.SetHealth(status.curHealth + value);
    }
    
    public virtual void RestoreMana(float value)
    {
        if (IsDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.
        status.SetMana(status.curMana + value);
    }
    
    public virtual void RestoreStamina(float value)
    {
        if (IsDead) return; // �̹� ���� ���¶�� �� �̻� ó������ �ʴ´�.
        status.SetStamina(status.curStamina + value);
    }

}