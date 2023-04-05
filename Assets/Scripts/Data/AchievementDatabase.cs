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
}
public class PlayData
{
    public int code;
    public string name;
    public int amount;
    public bool completed;
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

    public List<AchievementData> achievementDataList { get; private set; }

/*    private void Start()
    {
*//*        List<AchievementData> emptyAchievementDataList = new List<AchievementData>();
        AchievementData achievementData = new AchievementData();
        achievementData.code = 1;
        achievementData.name = "Survivor";
        achievementData.description = "Clear Stage 1";
        achievementData.isCompleted = false;
        emptyAchievementDataList.Add(achievementData);
        XML<AchievementData>.Write(saveFileName, emptyAchievementDataList);*//*
    }*/

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
    public AchievementData GetAchievementData(int index)
    {
        if(index < achievementDataList.Count)
            return achievementDataList[index];
        return null;
    }
    public AchievementData GetAchievementDataByCode(int code)
    {
        // Find and return the achievement data with the specified code
        foreach (AchievementData achievementData in achievementDataList)
        {
            if (achievementData.code == code)
            {
                return achievementData;
            }
        }
        return null;
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

    public void SetAchievementData(AchievementData achievementData)
    {
        // Find and update the achievement data with the specified code
        for (int i = 0; i < achievementDataList.Count; i++)
        {
            if (achievementDataList[i].code == achievementData.code)
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
