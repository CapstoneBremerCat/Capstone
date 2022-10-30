using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    public float speed { get; private set; } // 실제 달리기 속도
    private int skillPoint = 0; // 스킬포인트
    private void Awake()
    {
        InitStatus();
        speed = MoveSpeed;
    }

    //private bool isRun = false;
    //
    //[SerializeField] private bool isHeal;    // 체력회복 조건 확인

    //IEnumerator Simulate()
    //{
    //    OnDamage(20, Vector3.zero, Vector3.zero);
    //    yield return new WaitForSeconds(1);
    //    OnDamage(30, Vector3.zero, Vector3.zero);
    //    yield return new WaitForSeconds(2);
    //    OnDamage(40, Vector3.zero, Vector3.zero);
    //    yield return new WaitForSeconds(2);
    //}

    //public void Updates()
    //{
    //    if (isHeal == true)
    //    {
    //        // 체력이 최대치일 경우 회복 중단
    //        if (isHpFull)
    //        {
    //            isHeal = false;
    //        }
    //        else
    //        {
    //            // 통상 자연회복.
    //            RestoreHealth(recoverHp);
    //        }
    //    }
    //
    //    if (playerInput.run && curStamina > 0.0f)
    //    {
    //        // 더이상 달릴 수 없는 경우
    //        if (curStamina < runStamina)
    //        {
    //            speed = normalSpeed;
    //        }
    //        else
    //        {
    //            UseStamina(runStamina);  // update
    //            speed = runSpeed;
    //        }
    //    }
    //    else
    //    {
    //        // 대시 중이 아닐경우 지구력 자동 회복
    //        RestoreStamina(recoverStamina); // update
    //        speed = playerStat.moveSpeed;;
    //    }
    //    // use item
    //    if(playerInput.itemSlot)
    //    {
    //        Debug.Log("Use Item");
    //        RestoreHealth(20.0f);
    //    }
    //}
}
