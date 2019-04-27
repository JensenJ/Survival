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
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Path does not exist: " + path);
            return null;
        }
    }

    public static void SaveAttributes(Attributes attributes, string worldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + worldName);
        string path = Application.dataPath + "/Saves/" + worldName + "/PlayerAttributes.SGSAVE";
        FileStream stream = new FileStream(path, FileMode.Create);

        AttributeData data = new AttributeData(attributes);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static AttributeData LoadAttributes(string worldName)
    {
        string path = Application.dataPath + "/Saves/" + worldName + "/PlayerAttributes.SGSAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            AttributeData data = formatter.Deserialize(stream) as AttributeData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Path does not exist: " + path);
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
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Path does not exist: " + path);
            return null;
        }
    }

    public static void SaveLoadedChunks(ChunkLoader loader, string worldName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + worldName);
        string path = Application.dataPath + "/Saves/" + worldName + "/Chunks.SGSAVE";
        FileStream stream = new FileStream(path, FileMode.Create);

        ChunkData data = new ChunkData(loader);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ChunkData LoadLoadedChunks(string worldName)
    {
        string path = Application.dataPath + "/Saves/" + worldName + "/Chunks.SGSAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ChunkData data = formatter.Deserialize(stream) as ChunkData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Path does not exist: " + path);
            return null;
        }
    }

    public static void SaveSaves(WorldManager worldman)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Directory.CreateDirectory(Application.dataPath + "/Saves/");
        string path = Application.dataPath + "/Saves/SaveList.SGSAVE";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(worldman);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadSaves()
    {
        string path = Application.dataPath + "/Saves/SaveList.SGSAVE";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Path does not exist: " + path);
            return null;
        }
    }
}
