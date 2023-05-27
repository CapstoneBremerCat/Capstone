using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using BlockChain;

public enum Achievement
{
    Default = 0,
    Survivor = 1 << 0,
    Savior = 1 << 1,
    RingOwner = 1 << 2,
    NFTPioneer = 1 << 3,
    ZombieSlayer = 1 << 4,
    ZombieExterminator = 1 << 5,
    ZombieAnnihilator = 1 << 6,
    GiantSlayer = 1 << 7,
    SteelDestroyer = 1 << 8,
    FirstPartner = 1 << 9,
    TheAvengers = 1 << 10,
}

public class AchievementController : MonoBehaviour
{
    [SerializeField] private GameObject achievementUnlockedWindow;
    [SerializeField] private GameObject achievementUnlockedPopup;
    [SerializeField] private TextMeshProUGUI achievementUnlockedText;

    [SerializeField] private GameObject achievementsPanel;
    [SerializeField] private GameObject achievementSlotPrefab;
    [SerializeField] private List<AchievementSlot> achievementSlotList;
    [SerializeField] private TextMeshProUGUI achievedText;
    [SerializeField] private TextMeshProUGUI ticketsText;
    [SerializeField] private Button getNFTButton;

    public void OpenAchievementWindow()
    {
        // Refresh each achievement slot
        foreach (AchievementSlot slot in achievementSlotList)
        {
            // Refresh the slot's achievement state
            slot.RefreshUI();
        }
        // Update the tickets and achieved text elements with the initial values
        UpdateTicketsText();
        UpdateAchievedText();
    }

    public void initAchievement()
    {
        var index = 1;
        achievementSlotList.Clear();
        // Initialize each achievement slot
        for (int i = 0; i < AchievementDatabase.Instance.achievementDataList.Count; i++)
        {
            // Get the achievement data from the database
            AchievementData achievementData = AchievementDatabase.Instance.GetAchievementData(i);

            AchievementSlot slot = Instantiate(achievementSlotPrefab, achievementsPanel.transform).GetComponent<AchievementSlot>();
            // Set the slot's achievement information
            bool isEarned = (achievementData.code & NFTManager.Instance.achievementsCode) != 0;
            slot.SetAchievementSlot(achievementData, isEarned);

            // Set the slot's complete button click listener
            slot.CompleteButton.onClick.AddListener(() => {
                // If the achievement is not yet completed, mark it as completed and add a token
                if (slot.IsCompleted)
                {
                    slot.OnComplete();
                    NFTManager.Instance.AddRewardTickets();
                    UpdateAchievedText();
                    UpdateTicketsText();

                    // Save achievement completion to database
                    AchievementDatabase.Instance.SetAchievementData(slot.achievementData);
                    // save

                }
            });
            achievementSlotList.Add(slot);
            index *= 2;
        }

        // Expand the content rect to fit all the achievement slots
        achievementsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(
            achievementsPanel.GetComponent<RectTransform>().sizeDelta.x,
            achievementSlotList.Count * achievementSlotPrefab.GetComponent<RectTransform>().rect.height
        );

        // Set the getNFTButton click listener
        getNFTButton.onClick.AddListener(() => {
            // If the player has at least one token, spend one token and give the player an NFT
            if (OwnedRewardTickets.GetOwnedRewardTickets() > 0)
            {
                NFTManager.Instance.UseRewardTicket();
                UpdateTicketsText();
                GivePlayerNFT();

            }
        });

        // Update the tickets and achieved text elements with the initial values
        UpdateTicketsText();
        UpdateAchievedText();
    }

    void Start()
    {
        Mediator.Instance.RegisterEventHandler(GameEvent.ACHIEVEMENT_UNLOCKED, UnlockAchievement);
        Mediator.Instance.RegisterEventHandler(GameEvent.NFTTICKET_EARNED, RefreshTicketsText);
    }

    private void OnDestroy()
    {
        Mediator.Instance.UnregisterEventHandler(GameEvent.ACHIEVEMENT_UNLOCKED, UnlockAchievement);
        Mediator.Instance.UnregisterEventHandler(GameEvent.NFTTICKET_EARNED, RefreshTicketsText);
    }

    public void UnlockAchievement(object achievement)
    {
        foreach (AchievementSlot slot in achievementSlotList)
        {
            if(slot.achievementData.code == (int)achievement)
            {
                if (!slot.IsCompleted && !slot.IsEarned)
                {
                    slot.Complete();
                    AchievementDatabase.Instance.SetAchievementData(slot.achievementData);
                    // complete ����
                    achievementUnlockedText.text = slot.achievementData.name;
                    StartCoroutine(ShowAchievementPopup());
                    NFTManager.Instance.MintRewardNFT(slot.achievementData.code);
                    SoundManager.Instance.OnPlaySFX("Achieve");
                }
                break;
            }
        }
    }

    private IEnumerator ShowAchievementPopup()
    {
        Animation animation = achievementUnlockedPopup.GetComponent<Animation>();
        achievementUnlockedWindow.gameObject.SetActive(true);
        achievementUnlockedPopup.gameObject.SetActive(true);
        do
        {
            yield return null;
        } while (animation.isPlaying);
        achievementUnlockedWindow.gameObject.SetActive(false);
        achievementUnlockedPopup.gameObject.SetActive(false);
    }

    private void UpdateAchievedText()
    {
        int achievedCount = 0;
        foreach (AchievementSlot slot in achievementSlotList)
        {
            if (slot.IsEarned)
            {
                achievedCount++;
            }
        }

        achievedText.text = string.Format("{0} / {1}", achievedCount, achievementSlotList.Count);
    }

    private void UpdateTicketsText()
    {
        // Update the tickets text element with the current value of tickets
        ticketsText.text = OwnedRewardTickets.GetOwnedRewardTickets().ToString();
    }
    public void RefreshTicketsText(object tickets)
    {
        // Update the tickets text element with the current value of tickets
        ticketsText.text = ((int)tickets).ToString();
    }
    private void GivePlayerNFT()
    {
        // Give the player an NFT
        Debug.Log("Player has received an NFT!");
    }
}


    /*    //���� 1000���� ���̱�
    if (GameManager.Instance.zombieKillCount >= 1000) 
    {
        name = "Zombie Apocalypse Survivor"; //���� ����Į���� ������
        description = "Kill 1000 zombies";
    }

    //���� 5000���� ���̱�
    if (GameManager.Instance.zombieKillCount >= 5000) 
    {
        name = "Zombie Slayer"; //���� �л���
        description = "Kill 5000 zombies";
    }

//���� 10000���� ���̱�
if (GameManager.Instance.zombieKillCount >= 10000)
{
    name = "Undead Exterminator"; //���� ������
    description = "Kill 10000 zombies";
}

//���� 50000���� ���̱�
if (GameManager.Instance.zombieKillCount >= 50000)
{
    name = "Zombie Annihilator"; //���� �Ҹ���
    description = "Kill 50000 zombies";
}

//���� 3�� ���� ���̱�
if (GameManager.Instance.bossKilledTime <= 180)
{
    name = "Boss slayer"; //���� �����̾�
    description = " Clear the boss in 3 minutes";
}

//�극�� ����� �� ������
if (GameManager.Instance.allMusiciansRecruited)
{
    name = "Squad Leader"; //�д���
    description = "Recruiting Bremen Musicians";
}

//������� ������ ������ �����
if (GameManager.Instance.ringOwner)
{
    name = "Ring Bearer"; //������ ���տ��� ������� �����ڸ� ��Ī�Ѵ���.
    description = "Being the absolute ring owner";
}

//NFT �������� ù �����ϱ�
if (GameManager.Instance.firstNFTPurchase)
{
    name = "NFT Pioneer"; //NFT ��ô��
    description = "Buying NFT items for the first time";
}

//��� ���������� Ŭ�����ϱ�
if (GameManager.Instance.allStagesCleared)
{
    name = "Ultimate Survivor"; //�ñ��� ������
    description = "Clear All Stages";
}

NFTManager.Instance.mintRewardNFT(name, description);
}
*/
