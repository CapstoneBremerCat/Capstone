using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;   // ���� ������Ʈ
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

        public void SetWeapon(Weapon weapon)
        {
            this.weapon = weapon;
        }

        // Update is called once per frame
        public void ShootUpdate()
        {
            if (!weapon) return;
            Look();
            if (!playerInput) playerInput = GetComponent<PlayerInput>();
            if (playerInput.fire) weapon.Fire();   // �Ѿ� �߻�.  
                                                   // �Ѿ��� ������� ������ �õ�.
            if ((weapon.GetState.Equals(State.Empty) || playerInput.reload) && weapon.Reload() && anim) anim.SetTrigger("Reload");  //������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.
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
    }
}