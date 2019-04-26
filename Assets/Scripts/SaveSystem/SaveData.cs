using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string[] saveNames;

    public SaveData(WorldManager manager)
    {
        saveNames = new string[manager.worlds.Length];
        for (int i = 0; i < saveNames.Length; i++)
        {
            saveNames[i] = manager.worlds[i];
        }
    }
}
