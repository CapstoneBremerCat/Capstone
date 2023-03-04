using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Gun gun;   // 총 오브젝트
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Vector3 ScreenCenter;    // 에임 위치(정중앙)
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
        if (playerInput.fire) gun.Fire();   // 총알 발사.  
                                            // 총알이 비었으면 재장전 시도.
        if ((gun.GetState.Equals(State.Empty) || playerInput.reload) && gun.Reload() && anim) anim.SetTrigger("Reload");  //재장전 상태 확인 후, 재장전 애니메이션 재생.
        StageUIController.Instance.UpdateAmmoText(gun.MagAmmo, gun.AmmoRemain);  // ? 보호 수준
    }

    public void AddAmmo(int value)
    {
        if (gun) gun.AddAmmo(value);
    }

    // 총 회전
    private void Look()
    {
        // 스크린 상의 에임 위치를 참조하여 총을 해당 위치로 회전시킨다.
        Ray cameraRay = Camera.main.ScreenPointToRay(ScreenCenter);
        if(gun) gun.transform.LookAt(gun.FirePos.position + cameraRay.direction * gun.HitRange);
    }
}
