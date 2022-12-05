using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigunRotate : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject barrel; //회전시킬 총열
    [SerializeField] private float rotateSpeed; //회전속도
    // Update is called once per frame
    void Update()
    {
        if (playerInput.fire)
        {
            barrel.transform.Rotate(0f, 0f, rotateSpeed);
        }
    }
}
