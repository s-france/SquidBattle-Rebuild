using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem
{
    static SaveData saveData = new SaveData();


    [System.Serializable]
    public struct SaveData
    {
        //add actual data to be saved/loaded here
        ///currency
        int currency;

        ///colors
        int[] colors;

        ///hats
        int[] hats;

        ///modes
        int[] modes;

        ///arenas
        int[] arenas;

        ///stagebuilder parts
        int[] stageParts;

        ///Stats
        
        
    }


    //returns filepath to game save data
    public static string SaveFilePath()
    {
        return Application.persistentDataPath + "/savedata" + ".json";
    }


    public static void Save()
    {
        //write savedata to file
        File.WriteAllText(SaveFilePath(), JsonUtility.ToJson(saveData, true));
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFilePath());
        saveData = JsonUtility.FromJson<SaveData>(saveContent);
    }
    
}
