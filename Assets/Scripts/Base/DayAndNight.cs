using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game
{
    public class DayAndNight : MonoBehaviour
    {
        [SerializeField] private float secondPerRealTimeSecond; // ���� ���迡���� 100�� = ���� ������ 1��

        private bool isNight = false;

        [SerializeField] private float nightFogDensity; // �� ������ Fog �е�
        private float dayFogDensity; // �� ������ Fog �е�
        [SerializeField] private float fogDensityCalc; // ������ ����
        private float currentFogDensity;

        private void OnEnable()
        {
            dayFogDensity = RenderSettings.fogDensity;
        }

        public void InitSun()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            isNight = false;
        }

        public void UpdateSun()
        {
            // ���ǽð� 1�ð� ���� 1�ð� �� 15�� ȸ��. 
            // ��� �¾��� X �� �߽����� ȸ��. ���ǽð� 1�ʿ�  0.1f * secondPerRealTimeSecond ������ŭ ȸ��
            transform.Rotate(Vector3.right, 6 * Time.deltaTime);

            if (transform.eulerAngles.x >= 170) // x �� ȸ���� 170 �̻��̸� ���̶�� �ϰ���
                isNight = true;
            else if (transform.eulerAngles.x <= 10)  // x �� ȸ���� 10 ���ϸ� ���̶�� �ϰ���
                isNight = false;

            if (isNight)
            {
                if (currentFogDensity <= nightFogDensity)
                {
                    currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                    RenderSettings.fogDensity = currentFogDensity;
                }
            }
            else
            {
                if (currentFogDensity >= dayFogDensity)
                {
                    currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                    RenderSettings.fogDensity = currentFogDensity;
                }
            }
        }
    }
}