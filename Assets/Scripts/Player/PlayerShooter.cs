using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Gun gun;
    private Animator anim;
    private PlayerInput playerInput;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (gun && anim)
        {
            // 해당 오브젝트(Gun)의 pivot을 해당 애니메이션(upper body)의 오른쪽 팔꿈치 위치로 이동.
            gun.Pivot = anim.GetIKHintPosition(AvatarIKHint.RightElbow);
            // 왼손의 position, rotation을 해당 오브젝트(Gun)의 왼쪽 손잡이 위치에 맞춘다.
            anim.SetIKPosition(AvatarIKGoal.LeftHand, gun.LeftHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, gun.LeftHandMountRo);
            // 가중치(weight)를 추가하여 위치, 회전을 미세조정 한다.
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, gun.LeftHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, gun.LeftHandRoWeight);
            // 오른손의 position, rotation을 해당 오브젝트(Gun)의 오른쪽 손잡이 위치에 맞춘다.
            anim.SetIKPosition(AvatarIKGoal.RightHand, gun.RightHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.RightHand, gun.RightHandMountRo);
            // 가중치(weight)를 추가하여 위치, 회전을 미세조정 한다.
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, gun.RightHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, gun.RightHandRoWeight);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerInput && gun)
        {
            if (playerInput.fire) gun.Fire();   // 총알 발사.  
            // 총알이 비었으면 재장전 시도.
            if ((gun.GetState.Equals(State.Empty) || playerInput.reload) && gun.Reload() && anim) anim.SetTrigger("Reload");  //재장전 상태 확인 후, 재장전 애니메이션 재생.

            //if (gun && UIMgr.Instance) UIMgr.Instance.UpdateAmmoText(gun.MagAmmo, gun.AmmoRemain);  // ? 보호 수준
        }
    }

    public void AddAmmo(int value)
    {
        if (gun) gun.AddAmmo(value);
    }
}
