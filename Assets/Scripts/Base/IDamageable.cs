using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public interface IDamageable
    {
        // ������ ũ��(damage), ���� ����(hitPoint), ���� ǥ���� ����(hitNormal)
        void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
    }

}