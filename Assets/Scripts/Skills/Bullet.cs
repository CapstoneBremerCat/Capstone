using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigid;   // ����ü�� rigidbody
    [SerializeField] private float force = 100.0f; // �������� ��

    public void Start()
    {
        rigid.velocity = transform.forward * force;
        StartCoroutine(BulletExistTime(10.0f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    // �Ѿ��� ���� �ð����� ���� �� ��Ȱ��ȭ�ǵ��� ó��
    private IEnumerator BulletExistTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
