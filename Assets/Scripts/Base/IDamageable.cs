using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // 데미지 크기(damage), 맞은 지점(hitPoint), 맞은 표면의 방향(hitNormal)
    void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}
