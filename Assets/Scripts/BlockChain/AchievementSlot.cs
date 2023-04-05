using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using BlockChain;

public class AchievementSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Button completeButton;
    public bool IsCompleted { get; private set; }
    public bool IsEarned { get; private set; }
    public Button CompleteButton { get { return completeButton; } }
    public AchievementData achievementData { get; private set; }

    public void SetAchievementSlot(AchievementData achievementData, bool isEarned)
    {
        this.achievementData = achievementData;
        // Update the UI elements with the given information
        title.text = achievementData.name;
        description.text = achievementData.description;
        IsCompleted = achievementData.isCompleted;
        IsEarned = isEarned; // earned는 블록체인 서버에서 관리

        RefreshUI();
    }

    public void RefreshUI()
    {
        // If the achievement has already been earned, disable the complete button
        completeButton.interactable = !IsEarned;

        // If the achievement has not yet been completed, hide the complete button
        completeButton.gameObject.SetActive(IsCompleted);
    }
    public void Complete()
    {
        achievementData.isCompleted = IsCompleted = true;
    }

    public void OnComplete()
    {
        IsEarned = true;
        CompleteButton.interactable = false;
    }
}
