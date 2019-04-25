// Copyright (c) 2019 JensenJ
// NAME: SaveSystem
// PURPOSE: Manages the save system

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SavePlayer(PlayerController player, string worldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + worldName);
        string path = Application.dataPath + "/Saves/" + worldName + "/Player.SGSAVE";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer(string worldName)
    {
        string path = Application.dataPath + "/Saves/" + worldName + "/Player.SGSAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            return data;
        }
        else
        {
            Debug.LogError("Path does not exist" + path);
            return null;
        }
    }

    public static void SaveMap(MapGenerator mapgen, string worldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + worldName);
        string path = Application.dataPath + "/Saves/" + worldName + "/Map.SGSAVE";
        FileStream stream = new FileStream(path, FileMode.Create);

        MapData data = new MapData(mapgen);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static MapData LoadMap(string worldName)
    {
        string path = Application.dataPath + "/Saves/" + worldName + "/Map.SGSAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MapData data = formatter.Deserialize(stream) as MapData;

            return data;
        }
        else
        {
            Debug.LogError("Path does not exist" + path);
            return null;
        }
    }
}
