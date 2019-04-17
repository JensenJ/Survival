// Copyright (c) 2019 JensenJ
// NAME: MapGenerator
// PURPOSE: Generates a map

using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Settings for map generation
    [SerializeField] [Range(16, 250)] public int xSize = 16;
    [SerializeField] [Range(16, 250)] public int zSize = 16;
    [SerializeField] [Range(1, 20)] float amplitude = 10.0f;
    [SerializeField] [Range(0.1f, 20)] float frequency = 1.0f;
    [SerializeField] [Range(0.1f, 3)] float layerHeight = 0.5f;
    [SerializeField] [Range(0.5f, 1.5f)] float redistribution = 1.0f;
    [SerializeField] [Range(-200000, 200000)] public int seed = 0;
    [SerializeField] bool isTerrainSmooth = false;
    [SerializeField] bool isRandomGeneration = false;
    [SerializeField] Material material = null;

    [SerializeField] public HeightData[] heights;
    
    //Function for creating the chunk objects and drawing the map
    public void GenerateMap(int m_seed)
    {
        //Sets seed
        seed = m_seed;

        if(isRandomGeneration == true)
        {
            amplitude = Random.Range(10.0f, 20.0f);
            frequency = Random.Range(5.0f, 13.0f);
            seed = Random.Range(-200000, 200000);
            //redistribution = Random.Range(0.5f, 1.0f);
        }

        //Checks for too many vertices in single mesh.
        if (xSize * zSize > 62500)
        {
            Debug.LogError("Map size is too large for a single mesh. Cancelling map generation.");
        }
        else
        {
            //loader ID of 0 means this will never be unloaded
            CreateNewChunk(0, 0, 0);
        }
    }

    public GameObject CreateNewChunk(int x, int z, int loaderID)
    {
        //Create new terrain object, set name and parent to this. 
        //Set position and then call function within Chunk Generator
        GameObject terrain = new GameObject();
        terrain.name = "Chunk (" + x + ", " + z + ")";
        terrain.transform.SetParent(transform);
        terrain.transform.position = new Vector3(x * xSize, 0, z * zSize);
        ChunkGenerator cg = terrain.AddComponent<ChunkGenerator>();
        cg.DrawChunk(xSize, zSize, amplitude, frequency, layerHeight, redistribution, seed, xSize * x, zSize * z, isTerrainSmooth, material, loaderID, heights);
        return terrain;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(seed);
    }
}