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
        private void Awake()
        {
            Mediator.Instance.RegisterEventHandler(GameEvent.EQUIPPED_WEAPON, RefreshWeapon);
        }
        private void Start()
        {
            ScreenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2 + 32);
            anim = GetComponent<Animator>();
        }

        // Refreshes the currently equipped weapon based on the weaponItemObject passed in.
        private void RefreshWeapon(object weaponItemObject)
        {
            // If there is no currently equipped weapon, sets weapon to null.
            weapon = EquipManager.Instance.EquippedWeapon;
        }

        // Update is called once per frame
        public void ShootUpdate(bool fire)
        {
            if (!weapon) return;
            Look();
            if (fire) weapon.Fire();   // �Ѿ� �߻�.  
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