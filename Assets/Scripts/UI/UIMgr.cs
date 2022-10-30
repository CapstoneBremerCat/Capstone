using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    #region instance
    private static UIMgr instance = null;
    public static UIMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance�� ���� ������Ʈ�� �����
        instance = this;

        // Scene �̵� �� ���� ���� �ʵ��� ó��
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float defaultLength = 400f;

    [SerializeField] private Slider staminaSlider;




    private float healthLength;

    private void Start()
    {

    }

    public void SetHealthBar(float ratio)
    {
        healthLength = GetHealthLength(ratio);
        //if (playerStatus.isHpFull) healthLength = defaultLength;

        healthBar.sizeDelta = new Vector2(healthLength, 30);
    }
    private float GetHealthLength(float ratio)
    {
        return defaultLength * ratio;
    }

    public void SetManaBar(float ratio)
    {
        staminaSlider.value = ratio;
    }

    public void SetStaminaBar(float ratio)
    {
        staminaSlider.value = ratio;
    }
}
