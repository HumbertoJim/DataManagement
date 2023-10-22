using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement.Managers;

public class CharacterDataManager : BooleanDictionaryManager
{
    public List<string> GetUnlockedCharacters()
    {
        List<string> characters = Serializer.GetKeys();
        List<string> unlockedCharacters = new List<string>();
        foreach(string character in characters)
        {
            if (IsCharacterUnlocked(character)) unlockedCharacters.Add(character);
        }
        return unlockedCharacters;
    }

    public bool IsCharacterUnlocked(string characterID)
    {
        if (CharacterExists(characterID)) return Serializer.GetDataAsBool(characterID);
        return false;
    }

    public void SetCharacter(string characterID, bool value)
    {
        Serializer.SetData(characterID, value ? "true" : "false");
        SaveData();
    }

    public bool CharacterExists(string characterID)
    {
        return Serializer.DataExists(characterID);
    }
}