using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
/*    public float speed { get; private set; } // ���� �޸��� �ӵ�
    private int skillPoint = 0; // ��ų����Ʈ
    [SerializeField] private float health; // ���� ü��.
    private Animator anim;
    [SerializeField] private AudioClip deathSound;  // ��� ȿ����.
    [SerializeField] private AudioClip hitSound; // �ǰ� ȿ����.
    [SerializeField] private ParticleSystem hitEffect;  // �ǰ� ����Ʈ.
    private AudioSource audioSource;    // ȿ������ ����ϴµ� ���.
    private void Awake()
    {
        InitStatus();
        speed = moveSpeed;
        health = curHealth;
        OnDeath += () =>
        {
            // �� �̻� �ǰ� ������ ���� �ʰ� collider�� ����.
            //if (collider) collider.enabled = false;
            if (anim) anim.SetBool("isDead", isDead);   // Zombie Death �ִϸ��̼� ����.
            if (audioSource && deathSound) audioSource.PlayOneShot(deathSound);     // ��� ȿ���� 1ȸ ���.
            //GameMgr.instance.AddScore(100); // enemy óġ ��, 100 score ���.
            //EnemyMgr.Instance.DecreaseSpawnCount(); // enemy óġ ��, Spawn Count ����.
            UIMgr.Instance.GameOver();
        };
    }*/

/*    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        base.OnDamage(damage, hitPoint, hitNormal);
        health = curHealth;
        UIMgr.Instance.SetHealthBar(GetHpRatio());
        if (anim && !isDead)
        {
            if (hitEffect)
            {
                var hitEffectTR = hitEffect.transform;
                hitEffectTR.position = hitPoint;    // ����Ʈ�� �ǰ� �������� �̵�.
                // �ǰ� ���� �������� ȸ��.
                hitEffectTR.rotation = Quaternion.LookRotation(hitNormal);
                hitEffect.Play();   // ����Ʈ ���.
            }

            // �ǰ� ȿ���� 1ȸ ���.
            if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
            anim.SetTrigger("Damaged"); // �������� �԰� ���� �ʾҴٸ�, �ǰ� �ִϸ��̼� ����.
        }
    }*/

}
