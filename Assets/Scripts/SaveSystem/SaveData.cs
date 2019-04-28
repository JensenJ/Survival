// Copyright (c) 2019 JensenJ
// NAME: SaveData
// PURPOSE: Manages the save data

[System.Serializable]
public class SaveData
{
    public string[] saveNames;

    //World list
    public SaveData(WorldManager manager)
    {
        saveNames = new string[manager.worlds.Length];
        for (int i = 0; i < saveNames.Length; i++)
        {
            saveNames[i] = manager.worlds[i];
        }
    }
}
