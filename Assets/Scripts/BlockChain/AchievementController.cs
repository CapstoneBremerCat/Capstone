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
    SteelDestroyer = 1 << 8
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
                    // complete 연출
                    achievementUnlockedText.text = slot.achievementData.name;
                    StartCoroutine(ShowAchievementPopup());
                    NFTManager.Instance.MintRewardNFT(slot.achievementData.code);
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


    /*    //좀비 1000마리 죽이기
    if (GameManager.Instance.zombieKillCount >= 1000) 
    {
        name = "Zombie Apocalypse Survivor"; //좀비 아포칼립스 생존자
        description = "Kill 1000 zombies";
    }

    //좀비 5000마리 죽이기
    if (GameManager.Instance.zombieKillCount >= 5000) 
    {
        name = "Zombie Slayer"; //좀비 학살자
        description = "Kill 5000 zombies";
    }

//좀비 10000마리 죽이기
if (GameManager.Instance.zombieKillCount >= 10000)
{
    name = "Undead Exterminator"; //좀비 근절자
    description = "Kill 10000 zombies";
}

//좀비 50000마리 죽이기
if (GameManager.Instance.zombieKillCount >= 50000)
{
    name = "Zombie Annihilator"; //좀비 소멸자
    description = "Kill 50000 zombies";
}

//보스 3분 만에 죽이기
if (GameManager.Instance.bossKilledTime <= 180)
{
    name = "Boss slayer"; //보스 슬레이어
    description = " Clear the boss in 3 minutes";
}

//브레멘 동료들 다 모으기
if (GameManager.Instance.allMusiciansRecruited)
{
    name = "Squad Leader"; //분대장
    description = "Recruiting Bremen Musicians";
}

//절대반지 소유한 경험이 생기기
if (GameManager.Instance.ringOwner)
{
    name = "Ring Bearer"; //반지의 제왕에서 절대반지 소유자를 지칭한다함.
    description = "Being the absolute ring owner";
}

//NFT 아이템을 첫 구매하기
if (GameManager.Instance.firstNFTPurchase)
{
    name = "NFT Pioneer"; //NFT 개척자
    description = "Buying NFT items for the first time";
}

//모든 스테이지를 클러어하기
if (GameManager.Instance.allStagesCleared)
{
    name = "Ultimate Survivor"; //궁극의 생존자
    description = "Clear All Stages";
}

NFTManager.Instance.mintRewardNFT(name, description);
}
*/
