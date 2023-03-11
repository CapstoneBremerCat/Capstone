using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Light directionalLight;
    public Transform directionalLightTransform;
    public Transform shadowCenter;
    public float maxIntensity = 1f;
    public float minIntensity = 0.1f;
    public ShadowQuality shadowQuality = ShadowQuality.All;
    public float shadowStrength = 0.8f;
    private float angle;
    private float timeOfDay;
    private float shadowLength = 10f;

    void Start()
    {
        directionalLightTransform = directionalLight.transform;

        directionalLight.shadows = LightShadows.Soft;
        directionalLight.shadowStrength = shadowStrength;
        directionalLight.shadowNearPlane = 0.1f;
        directionalLight.shadowNormalBias = 0.05f;
        directionalLight.shadowBias = 0.05f;
        QualitySettings.shadows = ShadowQuality.All;

        UpdateDirectionalLight();
    }

    void Update()
    {
        timeOfDay += Time.deltaTime / 60f; // 하루가 1분인 경우
        if (timeOfDay > 1f)
        {
            timeOfDay -= 1f;
        }
        angle = CalculateAngleForTimeOfDay(timeOfDay);
        UpdateDirectionalLight();
        UpdateLightIntensity();
    }

    void UpdateDirectionalLight()
    {
        Vector3 shadowDirection = shadowCenter.position - directionalLightTransform.position;
        shadowDirection.y = 0f;
        shadowDirection = shadowDirection.normalized;
        Quaternion shadowRotation = Quaternion.LookRotation(shadowDirection);

        Vector3 lightDirection = Quaternion.Euler(angle, 0f, 0f) * Vector3.right;
        Quaternion lightRotation = Quaternion.LookRotation(lightDirection);

        directionalLightTransform.rotation = lightRotation * Quaternion.Inverse(shadowRotation);
    }

    void UpdateLightIntensity()
    {
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.Abs(Mathf.Sin(angle * Mathf.Deg2Rad)));
        directionalLight.intensity = intensity;
    }

    public float CalculateAngleForTimeOfDay(float timeOfDay)
    {
        float angle = 0f;
        float hour = timeOfDay * 24f;

        if (hour < 6f)
        {
            angle = 90f; // 새벽
        }
        else if (hour < 12f)
        {
            angle = Mathf.Lerp(90f, 0f, (hour - 6f) / 6f); // 오전
        }
        else if (hour < 18f)
        {
            angle = 0f; // 오후
        }
        else
        {
            angle = Mathf.Lerp(0f, 90f, (hour - 18f) / 6f); // 저녁
        }

        return angle;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        //Gizmos.DrawWireDisc(shadowCenter.position, Vector3.right, 1f);

        Vector3 shadowDirection = shadowCenter.position - directionalLightTransform.position;
        shadowDirection.y = 0f;
        shadowDirection = shadowDirection.normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(shadowCenter.position, shadowCenter.position + shadowDirection * shadowLength);

        Gizmos.color = Color.blue;
        //Handles.DrawWireArc(shadowCenter.position, Vector3.right, shadowDirection, 360f, 1f);
    }
}