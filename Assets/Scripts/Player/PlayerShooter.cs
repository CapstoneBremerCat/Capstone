using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;   // 무기 오브젝트
        [SerializeField] private Vector3 ScreenCenter;    // 에임 위치(정중앙)
        private Animator anim;

        // Start is called before the first frame update

        private void Start()
        {
            ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
            anim = GetComponent<Animator>();
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_WEAPON, RefreshWeapon);
        }

        // Refreshes the currently equipped weapon based on the weaponItemObject passed in.
        private void RefreshWeapon(object weaponItemObject)
        {
            // If there is no currently equipped weapon, sets weapon to null.
            weapon = EquipManager.Instance.EquippedWeapon;
            if(weapon) weapon.Init();
        }

        // Update is called once per frame
        public void ShootUpdate(bool fire, bool reload)
        {
            if (!weapon) return;
            Look();
            if (fire) weapon.Fire();   // 총알 발사.  
                                                   // 총알이 비었으면 재장전 시도.
            if ((weapon.GetState.Equals(State.Empty) || reload) && weapon.Reload() && anim) anim.SetTrigger("Reload");  //재장전 상태 확인 후, 재장전 애니메이션 재생.
            StageUIController.Instance.UpdateAmmoText(weapon.MagAmmo, weapon.AmmoRemain);  // ? 보호 수준
        }

        public void AddAmmo(int value)
        {
            if (weapon) weapon.AddAmmo(value);
        }

        // 총 회전
        private void Look()
        {
            // 스크린 상의 에임 위치를 참조하여 총을 해당 위치로 회전시킨다.
            Ray cameraRay = Camera.main.ScreenPointToRay(ScreenCenter);
            if (weapon) weapon.transform.LookAt(weapon.FirePos.position + cameraRay.direction * weapon.HitRange);
        }
        private void OnDestroy()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_WEAPON, RefreshWeapon);
        }
    }
}