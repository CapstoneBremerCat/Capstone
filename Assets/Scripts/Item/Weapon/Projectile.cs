using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigid;   // 투사체의 rigidbody
        [SerializeField] private float force = 100.0f; // 가해지는 힘


        public void Start()
        {
            rigid.velocity = transform.forward * force;
            StartCoroutine(ExistTime(10.0f));
        }

        private void OnCollisionEnter(Collision collision)
        {
            gameObject.SetActive(false);
        }

        // 총알이 일정 시간동안 존재 후 비활성화되도록 처리
        private IEnumerator ExistTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }
}