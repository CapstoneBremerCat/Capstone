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
    [SerializeField] private TextMeshProUGUI waveText; // �� ���̺� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private TextMeshProUGUI ammoText; // ź�� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private GameObject gameoverUI; // ���� ������ Ȱ��ȭ�� UI.
    [SerializeField] private GameObject sleepUI; // ���� UI.
    [SerializeField] private Animator waveStartUI; // ���̺� ���� UI.
    [SerializeField] private Animator waveClearUI; // ���̺� Ŭ���� UI.
    [SerializeField] private Animator stageClearUI; // �������� Ŭ���� UI.
    [SerializeField] private Animator nextDayUI; // ���� �� ���� UI.
    [SerializeField] private TextMeshProUGUI beforeDayText; // ���� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI afterDayText;  // ���� ���� �ؽ�Ʈ

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
        if (gameoverUI) gameoverUI.SetActive(true);
    }

    public void WaveStart()
    {
        if (waveStartUI) StartCoroutine(PlayAnim(waveStartUI));
    }

    public void WaveClear()
    {
        if (waveClearUI) StartCoroutine(PlayAnim(waveClearUI));
    }

    public void StageClear()
    {
        if (stageClearUI) StartCoroutine(PlayAnim(stageClearUI));
    }

    public void DisplayNextDay(int nextDay)
    {
        beforeDayText.text = (nextDay - 1).ToString();
        afterDayText.text = (nextDay).ToString();
        if (nextDayUI) StartCoroutine(PlayAnim(nextDayUI));
    }

    private IEnumerator PlayAnim(Animator anim)
    {
        anim.gameObject.SetActive(true);

        // �ش� �ִϸ��̼� ���� �������� ���
        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        };
        anim.gameObject.SetActive(false);
    }
}
