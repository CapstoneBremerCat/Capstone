using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    public float speed { get; private set; } // 실제 달리기 속도
    private int skillPoint = 0; // 스킬포인트
    [SerializeField] private float health; // 현재 체력.
    private Animator anim;
    [SerializeField] private AudioClip deathSound;  // 사망 효과음.
    [SerializeField] private AudioClip hitSound; // 피격 효과음.
    [SerializeField] private ParticleSystem hitEffect;  // 피격 이펙트.
    private AudioSource audioSource;    // 효과음을 출력하는데 사용.
    private void Awake()
    {
        InitStatus();
        speed = moveSpeed;
        health = curHealth;
        OnDeath += () =>
        {
            // 더 이상 피격 판정이 되지 않게 collider를 끈다.
            //if (collider) collider.enabled = false;
            if (anim) anim.SetBool("isDead", isDead);   // Zombie Death 애니메이션 실행.
            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);     // 사망 효과음 1회 재생.
            //GameMgr.instance.AddScore(100); // enemy 처치 시, 100 score 상승.
            //EnemyMgr.Instance.DecreaseSpawnCount(); // enemy 처치 시, Spawn Count 감소.
            UIMgr.Instance.GameOver();
        };
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        health = curHealth;
        UIMgr.Instance.SetHealthBar(GetHpRatio());
        if (anim && !isDead)
        {
            if (hitEffect)
            {
                var hitEffectTR = hitEffect.transform;
                hitEffectTR.position = hitPoint;    // 이펙트를 피격 지점으로 이동.
                // 피격 당한 방향으로 회전.
                hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play();   // 이펙트 재생.
            }

            // 피격 효과음 1회 재생.
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
            anim.SetTrigger("Damaged"); // 데미지를 입고 죽지 않았다면, 피격 애니메이션 실행.
        }
    }

}
