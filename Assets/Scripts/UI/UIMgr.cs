using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIMgr : MonoBehaviour
{
    #region instance
    private static UIMgr instance = null;
    public static UIMgr Instance { get { return instance; } }

    private void Awake()
    {
        // Scene에 이미 인스턴스가 존재 하는지 확인 후 처리
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        // instance를 유일 오브젝트로 만든다
        instance = this;

        // Scene 이동 시 삭제 되지 않도록 처리
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] private RectTransform healthBar;
    [SerializeField] private float defaultLength = 400f;

    [SerializeField] private Slider staminaSlider;
    [SerializeField] private TextMeshProUGUI waveText; // 적 웨이브 표시용 텍스트.
    [SerializeField] private TextMeshProUGUI ammoText; // 탄약 표시용 텍스트.
    [SerializeField] private GameObject gameoverUI; // 게임 오버시 활성화할 UI.

    private float healthLength;

    public void Init()
    {
        gameoverUI.SetActive(false);
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

    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        //var strBuilder = new System.Text.StringBuilder();
        //strBuilder.Append(magAmmo);
        //strBuilder.Append(" / ");
        //strBuilder.Append(remainAmmo);
        ammoText.text = string.Format("{0} / {1}", magAmmo, remainAmmo);
    }

    public void GameOver()
    {
        if(gameoverUI) gameoverUI.SetActive(true);
    }
}
