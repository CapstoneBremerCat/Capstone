using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using BlockChain;

public enum Achievement
{
    Survivor = 1 << 0,
    Savior = 1 << 1,
    RingOwner = 1 << 2,
    NFTPioneer = 1 << 3,
    ZombieSlayer = 1 << 4,
    ZombieExterminator = 1 << 5,
    ZombieAnnihilator = 1 << 6,
    GiantSlayer = 1 << 7
}

public class AchievementController : MonoBehaviour
{
    [SerializeField] private AchievementSlot[] achievementSlots;
    [SerializeField] private TextMeshProUGUI achievedText;
    [SerializeField] private TextMeshProUGUI tokensText;
    [SerializeField] private Button getNFTButton;
    private int tokens;

    private void OnEnable()
    {
        // Refresh each achievement slot
        foreach (AchievementSlot slot in achievementSlots)
        {
            // Refresh the slot's achievement state
            slot.RefreshUI();
        }
        // Update the tokens and achieved text elements with the initial values
        UpdateTokensText();
        UpdateAchievedText();
    }

    void Start()
    {
        // Set the initial value of tokens
        tokens = 0;

        // Initialize each achievement slot
        foreach (AchievementSlot slot in achievementSlots)
        {
            // Get the achievement data from the database
            AchievementData achievementData = AchievementDatabase.Instance.GetAchievementData(1/*���ü�� ������*/);

            // Set the slot's achievement information
            slot.SetAchievementSlot(achievementData);

            // Set the slot's complete button click listener
            slot.CompleteButton.onClick.AddListener(() => {
                // If the achievement is not yet completed, mark it as completed and add a token
                if (slot.IsCompleted)
                {
                    slot.OnComplete();
                    tokens++;
                    UpdateAchievedText();
                    UpdateTokensText();
                }
            });
        }

        // Set the getNFTButton click listener
        getNFTButton.onClick.AddListener(() => {
            // If the player has at least one token, spend one token and give the player an NFT
            if (tokens > 0)
            {
                tokens--;
                UpdateTokensText();
                GivePlayerNFT();
            }
        });

        // Update the tokens and achieved text elements with the initial values
        UpdateTokensText();
        UpdateAchievedText();
    }
    private void UpdateAchievedText()
    {
        int achievedCount = 0;
        foreach (AchievementSlot slot in achievementSlots)
        {
            if (slot.IsEarned)
            {
                achievedCount++;
            }
        }

        achievedText.text = string.Format("{0} / {1}", achievedCount, achievementSlots.Length);
    }

    private void UpdateTokensText()
    {
        // Update the tokens text element with the current value of tokens
        tokensText.text = tokens.ToString();
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
