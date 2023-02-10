using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Gun gun;   // �� ������Ʈ
    [SerializeField] private Vector3 ScreenCenter;    // ���� ��ġ(���߾�)
    private Animator anim;
    private PlayerInput playerInput;
    // Start is called before the first frame update

    private void Start()
    {
        ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
/*        if (gun && anim)
        {
            // �ش� ������Ʈ(Gun)�� pivot�� �ش� �ִϸ��̼�(upper body)�� ������ �Ȳ�ġ ��ġ�� �̵�.
            gun.Pivot = anim.GetIKHintPosition(AvatarIKHint.RightElbow);
            // �޼��� position, rotation�� �ش� ������Ʈ(Gun)�� ���� ������ ��ġ�� �����.
            anim.SetIKPosition(AvatarIKGoal.LeftHand, gun.LeftHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, gun.LeftHandMountRo);
            // ����ġ(weight)�� �߰��Ͽ� ��ġ, ȸ���� �̼����� �Ѵ�.
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, gun.LeftHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, gun.LeftHandRoWeight);
            // �������� position, rotation�� �ش� ������Ʈ(Gun)�� ������ ������ ��ġ�� �����.
            anim.SetIKPosition(AvatarIKGoal.RightHand, gun.RightHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.RightHand, gun.RightHandMountRo);
            // ����ġ(weight)�� �߰��Ͽ� ��ġ, ȸ���� �̼����� �Ѵ�.
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, gun.RightHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, gun.RightHandRoWeight);
        }*/
    }

    // Update is called once per frame
    public void ShootUpdate()
    {
        Look();
        if (playerInput && gun)
        {
            if (playerInput.fire) gun.Fire();   // �Ѿ� �߻�.  
            // �Ѿ��� ������� ������ �õ�.
            if ((gun.GetState.Equals(State.Empty) || playerInput.reload) && gun.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.

            if (StageUIController.Instance) StageUIController.Instance.UpdateAmmoText(gun.MagAmmo, gun.AmmoRemain);  // ? ��ȣ ����
        }
    }

    public void AddAmmo(int value)
    {
        if (gun) gun.AddAmmo(value);
    }

    // �� ȸ��
    private void Look()
    {
        // ��ũ�� ���� ���� ��ġ�� �����Ͽ� ���� �ش� ��ġ�� ȸ����Ų��.
        Ray cameraRay = Camera.main.ScreenPointToRay(ScreenCenter);
        gun.transform.LookAt(gun.FirePos.position + cameraRay.direction * gun.HitRange);
    }
}
