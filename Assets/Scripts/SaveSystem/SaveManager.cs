using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    MenuManager mm;
    MapGenerator mg;
    PlayerController pc;
    ChunkLoader pcl;
    Attributes pa;

    public void Start()
    {
        mm = transform.root.GetChild(5).GetComponent<MenuManager>();
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
        pa = transform.root.GetChild(3).GetComponent<Attributes>();
        pcl = transform.root.GetChild(3).GetComponent<ChunkLoader>();
    }

    public void SaveGame()
    {
        SaveSystem.SaveMap(mg, WorldData.currentlyLoadedName);
        SaveSystem.SavePlayer(pc, WorldData.currentlyLoadedName);
        SaveSystem.SaveAttributes(pa, WorldData.currentlyLoadedName);
        SaveSystem.SaveLoadedChunks(pcl, WorldData.currentlyLoadedName);
    }

    public void Exit()
    {
        SaveGame();
        mm.MainMenu();
    }

    public void LoadGame()
    {
        //Map Data
        MapData mapData = SaveSystem.LoadMap(WorldData.currentlyLoadedName);
        mg.amplitude = mapData.amplitude;
        mg.frequency = mapData.frequency;
        mg.GenerateMap(mapData.seed);

        //Player Data
        PlayerData playerData = SaveSystem.LoadPlayer(WorldData.currentlyLoadedName);
        pc.speed = playerData.speed;
        pc.sprintSpeed = playerData.sprintSpeed;
        pc.jumpForce = playerData.jumpForce;
        pc.transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
        pc.transform.rotation = Quaternion.Euler(0, playerData.rotation[1], playerData.rotation[2]);
        pc.transform.GetChild(1).rotation = Quaternion.Euler(playerData.rotation[0], 0, 0);

        //Player attribute data
        AttributeData attributeData = SaveSystem.LoadAttributes(WorldData.currentlyLoadedName);
        pa.bCanRegenHealth = attributeData.canRegen[0];
        pa.bCanRegenStamina = attributeData.canRegen[1];
        pa.bCanRegenThirst = attributeData.canRegen[2];
        pa.bCanRegenHunger = attributeData.canRegen[3];

        pa.healthMeterDrainSpeed = attributeData.drainSpeeds[0];
        pa.staminaMeterDrainSpeed = attributeData.drainSpeeds[1];
        pa.thirstMeterDrainSpeed = attributeData.drainSpeeds[2];
        pa.hungerMeterDrainSpeed = attributeData.drainSpeeds[3];

        pa.healthMeterRegenSpeed = attributeData.regenSpeeds[0];
        pa.staminaMeterRegenSpeed = attributeData.regenSpeeds[1];
        pa.thirstMeterRegenSpeed = attributeData.regenSpeeds[2];
        pa.hungerMeterRegenSpeed = attributeData.regenSpeeds[3];

        pa.maxHealthMeter = attributeData.maxValues[0];
        pa.maxStaminaMeter = attributeData.maxValues[1];
        pa.maxThirstMeter = attributeData.maxValues[2];
        pa.maxHungerMeter = attributeData.maxValues[3];

        pa.ChangeHealthLevel(attributeData.currentValues[0], false);
        pa.ChangeStaminaLevel(attributeData.currentValues[1], false);
        pa.ChangeThirstLevel(attributeData.currentValues[2], false);
        pa.ChangeHungerLevel(attributeData.currentValues[3], false);

        //Currently loaded chunks
        ChunkData chunkData = SaveSystem.LoadLoadedChunks(WorldData.currentlyLoadedName);
        pcl.chunknames = chunkData.loadedChunks;
        pcl.LoadMap();
    }
}
