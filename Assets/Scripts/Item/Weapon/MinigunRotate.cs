using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunRotate : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObject barrel; //회전시킬 총열
    [SerializeField] private float rotateSpeed; //회전속도
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
