using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class AchievementDataManager : BooleanDictionaryManager
{
    public bool GetAchievement(string achievementID)
    {
        if (AchievementExists(achievementID)) return Serializer.GetDataAsBool(achievementID);
        return false;
    }

    public void SetAchievement(string achievementID, bool value)
    {
        Serializer.SetData(achievementID, value ? "true" : "false");
        SaveData();
    }

    public bool AchievementExists(string achievementID)
    {
        return Serializer.DataExists(achievementID);
    }
}