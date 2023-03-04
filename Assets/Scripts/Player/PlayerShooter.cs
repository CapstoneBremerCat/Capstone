using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Gun gun;   // �� ������Ʈ
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Vector3 ScreenCenter;    // ���� ��ġ(���߾�)
    private Animator anim;

    // Start is called before the first frame update

    private void Start()
    {
        ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void SetGun(Gun gun)
    {
        this.gun = gun;
    }

    // Update is called once per frame
    public void ShootUpdate()
    {
        if (!gun) return;
        Look();
        if(!playerInput) playerInput = GetComponent<PlayerInput>();
        if (playerInput.fire) gun.Fire();   // �Ѿ� �߻�.  
                                            // �Ѿ��� ������� ������ �õ�.
        if ((gun.GetState.Equals(State.Empty) || playerInput.reload) && gun.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.
        StageUIController.Instance.UpdateAmmoText(gun.MagAmmo, gun.AmmoRemain);  // ? ��ȣ ����
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
        if(gun) gun.transform.LookAt(gun.FirePos.position + cameraRay.direction * gun.HitRange);
    }
}
