using System.Collections.Generic;
using UnityEngine;
using Game;
[System.Serializable]
public class AchievementData
{
    public int code;
    public string name;
    public string description;
    public bool isCompleted;
    public bool isEarned;
}

public class AchievementDatabase : MonoBehaviour
{
    #region instance
    private static AchievementDatabase instance = null;
    public static AchievementDatabase Instance { get { return instance; } }

    private void Awake()
    {
        if (instance)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);

        // Load the achievement data from the save file
        LoadAchievementData();
    }
    #endregion

    [SerializeField] private string saveFileName = "achievementData";

    private List<AchievementData> achievementDataList;

    private void LoadAchievementData()
    {
        // Read the achievement data from the XML file
        try
        {
            achievementDataList = XML<AchievementData>.Read(saveFileName);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to load achievement data: " + e.Message);
            achievementDataList = new List<AchievementData>();
        }
    }

    private void SaveAchievementData()
    {
        // Save the achievement data to the XML file
        try
        {
            XML<AchievementData>.Write(saveFileName, achievementDataList);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to save achievement data: " + e.Message);
        }
    }

    public AchievementData GetAchievementData(int code)
    {
        // Find and return the achievement data with the specified code
        foreach (AchievementData achievementData in achievementDataList)
        {
            if (achievementData.code == code)
            {
                return achievementData;
            }
        }

        // If the achievement data with the specified code does not exist, create it and add it to the list
        AchievementData newAchievementData = new AchievementData
        {
            code = code,
            name = "Achievement Title",
            description = "Achievement Description",
            isCompleted = false,
            isEarned = false
        };
        achievementDataList.Add(newAchievementData);
        SaveAchievementData();

        return newAchievementData;
    }

    public void SetAchievementData(int code, AchievementData achievementData)
    {
        // Find and update the achievement data with the specified code
        for (int i = 0; i < achievementDataList.Count; i++)
        {
            if (achievementDataList[i].code == code)
            {
                achievementDataList[i] = achievementData;
                SaveAchievementData();
                return;
            }
        }

        // If the achievement data with the specified code does not exist, add it to the list
        achievementDataList.Add(achievementData);
        SaveAchievementData();
    }
}
