// Copyright (c) 2019 JensenJ
// NAME: MapData
// PURPOSE: Holds data for map for saving and loading

using UnityEngine;

[System.Serializable]
public class MapData
{
    //Map data
    public int seed;
    public float[] frequency;
    public float[] amplitude;

    //Map data
    public MapData(MapGenerator mapgen)
    {
        frequency = new float[mapgen.terrainOctaves.Length];
        amplitude = new float[mapgen.terrainOctaves.Length];

        for (int i = 0; i < mapgen.terrainOctaves.Length; i++)
        {
            frequency[i] = mapgen.terrainOctaves[i].frequency;
            amplitude[i] = mapgen.terrainOctaves[i].amplitude;
        }
        seed = mapgen.seed;
    }

}