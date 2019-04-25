// Copyright (c) 2019 JensenJ
// NAME: MapData
// PURPOSE: Holds data for map for saving and loading

using UnityEngine;

[System.Serializable]
public class MapData
{
    public int seed;
    public float frequency;
    public float amplitude;

    public MapData(MapGenerator mapgen)
    {
        amplitude = mapgen.amplitude;
        seed = mapgen.seed;
        frequency = mapgen.frequency;
    }

}