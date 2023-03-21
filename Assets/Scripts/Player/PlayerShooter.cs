using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;   // ���� ������Ʈ
        [SerializeField] private Vector3 ScreenCenter;    // ���� ��ġ(���߾�)
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
            Item weaponItem = weaponItemObject as Item;
            // Retrieves the Weapon instance corresponding to the weaponItem's itemCode using the ItemMgr.
            Weapon weapon = ItemMgr.Instance.GetWeaponById(weaponItem.itemCode);
            if(EquipManager.Instance.EquippedWeapon) this.weapon = weapon;
            // If there is no currently equipped weapon, sets weapon to null.
            else this.weapon = null;
        }

        // Update is called once per frame
        public void ShootUpdate(bool fire, bool reload)
        {
            if (!weapon) return;
            Look();
            if (fire) weapon.Fire();   // �Ѿ� �߻�.  
                                                   // �Ѿ��� ������� ������ �õ�.
            if ((weapon.GetState.Equals(State.Empty) || reload) && weapon.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.
            StageUIController.Instance.UpdateAmmoText(weapon.MagAmmo, weapon.AmmoRemain);  // ? ��ȣ ����
        }

        public void AddAmmo(int value)
        {
            if (weapon) weapon.AddAmmo(value);
        }

        // �� ȸ��
        private void Look()
        {
            // ��ũ�� ���� ���� ��ġ�� �����Ͽ� ���� �ش� ��ġ�� ȸ����Ų��.
            Ray cameraRay = Camera.main.ScreenPointToRay(ScreenCenter);
            if (weapon) weapon.transform.LookAt(weapon.FirePos.position + cameraRay.direction * weapon.HitRange);
        }
        private void OnDestroy()
        {
            Mediator.Instance.UnregisterEventHandler(GameEvent.EQUIPPED_WEAPON, RefreshWeapon);
        }
    }
}