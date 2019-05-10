// Copyright (c) 2019 JensenJ
// NAME: SaveManager
// PURPOSE: Manages the save system

using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //Classes of things to save
    MenuManager mm;
    MapGenerator mg;
    PlayerController pc;
    ChunkLoader pcl;
    Attributes pa;
    WaypointManager wm;
    EnvironmentController env;

    //Get references
    public void Start()
    {
        mm = transform.root.GetChild(5).GetComponent<MenuManager>();
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();
        pc = transform.root.GetChild(3).GetComponent<PlayerController>();
        pa = transform.root.GetChild(3).GetComponent<Attributes>();
        wm = transform.root.GetChild(2).GetComponent<WaypointManager>();
        pcl = transform.root.GetChild(3).GetComponent<ChunkLoader>();
        env = transform.root.GetChild(5).GetComponent<EnvironmentController>();
    }

    //Function for saving all data in game
    public void SaveGame()
    {
        if (WorldData.currentlyLoadedName != null)
        {
            SaveSystem.SaveMap(mg, WorldData.currentlyLoadedName);
            SaveSystem.SavePlayer(pc, WorldData.currentlyLoadedName);
            SaveSystem.SaveAttributes(pa, WorldData.currentlyLoadedName);
            SaveSystem.SaveLoadedChunks(pcl, WorldData.currentlyLoadedName);
            SaveSystem.SaveWaypoints(wm, WorldData.currentlyLoadedName);
            SaveSystem.SaveEnvironment(env, WorldData.currentlyLoadedName);
        }
    }

    //Exits to main menu with saving
    public void Exit()
    {
        SaveGame();
        mm.MainMenu();
    }

    //loads game data
    public void LoadGame()
    {
        //Map Data
        MapData mapData = SaveSystem.LoadMap(WorldData.currentlyLoadedName);
        for (int i = 0; i < mapData.amplitude.Length; i++)
        {
            mg.terrainOctaves[i].amplitude = mapData.amplitude[i];
            mg.terrainOctaves[i].frequency = mapData.frequency[i];
        }
        mg.GenerateMap(mapData.seed);

        //Player Data
        PlayerData playerData = SaveSystem.LoadPlayer(WorldData.currentlyLoadedName);
        pc.speed = playerData.speed;
        pc.sprintSpeed = playerData.sprintSpeed;
        pc.jumpForce = playerData.jumpForce;
        pc.transform.position = new Vector3(playerData.position[0], playerData.position[1], playerData.position[2]);
        pc.transform.rotation = Quaternion.Euler(new Vector3(0, playerData.playerRot, 0));

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

        //Waypoint data
        WaypointData waypointData = SaveSystem.LoadWaypoints(WorldData.currentlyLoadedName);
        wm.waypoints = new Waypoint[waypointData.waypointIDs.Length];
        for (int i = 0; i < wm.waypoints.Length; i++)
        {
            if (waypointData.waypointInUse[i] == true) // if waypoint is in use
            {
                string name = waypointData.waypointNames[i];
                bool enabled = waypointData.waypointEnabled[i];

                float r = waypointData.waypointColors[i, 0];
                float g = waypointData.waypointColors[i, 1];
                float b = waypointData.waypointColors[i, 2];

                float x = waypointData.waypointLocations[i, 0];
                float y = waypointData.waypointLocations[i, 1];
                float z = waypointData.waypointLocations[i, 2];

                wm.AddWaypoint(name, new Vector3(x, y, z), new Color(r, g, b), enabled);
            }
        }

        //Load environment data
        EnvironmentData envData = SaveSystem.LoadEnvironment(WorldData.currentlyLoadedName);
        env.timeMultiplier = envData.timeMultiplier;
        env.daysInMonth = envData.daysInMonth;
        env.monthsInYear = envData.MonthsInYear;
        env.Clockwork = envData.clockWork;

        env.tempMultiplier = envData.tempMultiplier;
        env.bIsTempFahrenheit = envData.tempFahrenheit;
        env.temperature = envData.temperature;
        env.tempPrecision = envData.tempPrecision;

        env.windStrengthMultiplier = envData.windStrengthMultiplier;
        env.windStrength = envData.windStrength;
        env.windStrengthPrecision = envData.windStrengthPrecision;

        env.windAnglePrecision = envData.windAnglePrecision;
        env.windAngle = envData.windAngle;

        env.seasonEnum = (EnvironmentController.ESeasonEnum)System.Enum.Parse(typeof(EnvironmentController.ESeasonEnum), envData.season);
        env.weatherEnum = (EnvironmentController.EWeatherEnum)System.Enum.Parse(typeof(EnvironmentController.EWeatherEnum), envData.weather);
    }
}
