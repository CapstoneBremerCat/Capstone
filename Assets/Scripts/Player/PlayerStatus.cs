using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    public float speed { get; private set; } // ���� �޸��� �ӵ�
    private int skillPoint = 0; // ��ų����Ʈ
    private void Awake()
    {
        InitStatus();
        speed = MoveSpeed;
    }

    //private bool isRun = false;
    //
    //[SerializeField] private bool isHeal;    // ü��ȸ�� ���� Ȯ��

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
    //        // ü���� �ִ�ġ�� ��� ȸ�� �ߴ�
    //        if (isHpFull)
    //        {
    //            isHeal = false;
    //        }
    //        else
    //        {
    //            // ��� �ڿ�ȸ��.
    //            RestoreHealth(recoverHp);
    //        }
    //    }
    //
    //    if (playerInput.run && curStamina > 0.0f)
    //    {
    //        // ���̻� �޸� �� ���� ���
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
    //        // ��� ���� �ƴҰ�� ������ �ڵ� ȸ��
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
