using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class MinigunRotate : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private GameObject barrel; //ȸ����ų �ѿ�
        [SerializeField] private float rotateSpeed; //ȸ���ӵ�
                                                    // Update is called once per frame
        void Update()
        {
            //barrel.transform.Rotate(0f, 0f, rotateSpeed);
            if (weapon.IsFire)
            {
                barrel.transform.Rotate(0f, 0f, rotateSpeed);
            }
        }
    }
}