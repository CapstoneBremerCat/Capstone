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
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Look();
        if (playerInput && gun)
        {
            if (playerInput.fire) gun.Fire();   // �Ѿ� �߻�.  
            // �Ѿ��� ������� ������ �õ�.
            if ((gun.GetState.Equals(State.Empty) || playerInput.reload) && gun.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.

            if (gun && UIMgr.Instance) UIMgr.Instance.UpdateAmmoText(gun.MagAmmo, gun.AmmoRemain);  // ? ��ȣ ����
        }
    }

    public void AddAmmo(int value)
    {
        if (gun) gun.AddAmmo(value);
    }


    // ��ũ�� ���� ���콺 ��ġ�� �����Ͽ� �÷��̾ �ش� �������� ȸ����Ų��.
    private void Look()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane GroupPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (GroupPlane.Raycast(cameraRay, out rayLength))

        {
            Vector3 pointTolook = cameraRay.GetPoint(rayLength);
            gun.transform.LookAt(new Vector3(pointTolook.x, gun.transform.position.y, pointTolook.z));
        }
    }
}
