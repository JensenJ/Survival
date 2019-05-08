// Copyright (c) 2019 JensenJ
// NAME: MapGenerator
// PURPOSE: Generates a map

using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Settings for map generation
    [SerializeField] [Range(16, 128)] public int chunkSize = 16;
    [SerializeField] [Range(1.0f, 20.0f)] public float amplitude = 10.0f;
    [SerializeField] [Range(0.1f, 20.0f)] public float frequency = 1.0f;
    [SerializeField] [Range(2.0f, 20.0f)] public float waterHeight = 5.0f;
    [SerializeField] [Range(-100000, 100000)] public int seed = 0;
    [SerializeField] public Vector2 offset;
    [SerializeField] private Material material = null;
    [SerializeField] private Material waterMaterial = null;
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] public WaveOctave[] waveOctaves;
    [SerializeField] public HeightData[] heights;

    //Function for creating the chunk objects and drawing the map
    public void GenerateMap(int m_seed)
    {
        //Sets seed
        seed = m_seed;

        //If map is new, randomly generate values
        if (WorldData.isNewMap == true)
        {
            amplitude = 11;
            frequency = 8.75f;
            seed = WorldData.currentSeed;
        }
        CreateNewChunk(0, 0, 0);
    }

    void Start()
    {
        if(generateOnStart == true)
        {
            GenerateMap(seed);
        }
    }

    public GameObject CreateNewChunk(int x, int z, int loaderID)
    {
        //Create new terrain object, set name and parent to this. 
        //Set position and then call function within Chunk Generator
        GameObject terrain = new GameObject();
        terrain.name = "Chunk (" + x + ", " + z + ")";
        terrain.transform.SetParent(transform);
        terrain.transform.position = new Vector3(x * chunkSize, 0, z * chunkSize);
        ChunkGenerator cg = terrain.AddComponent<ChunkGenerator>();
        cg.DrawChunk(chunkSize, amplitude, frequency, seed, new Vector2((chunkSize * x) + offset.x, (chunkSize * z) + offset.y), material, loaderID, heights, waterHeight, waterMaterial, waveOctaves);
        return terrain;
    }
}