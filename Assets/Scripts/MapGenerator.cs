// Copyright (c) 2019 JensenJ
// NAME: MapGenerator
// PURPOSE: Generates a map

using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Settings for map generation
    [SerializeField] int xChunks = 2;
    [SerializeField] int zChunks = 2;

    [SerializeField] [Range(16, 250)] int xSize = 250;
    [SerializeField] [Range(16, 250)] int zSize = 250;
    [SerializeField] [Range(1, 20)] float amplitude = 10.0f;
    [SerializeField] [Range(0.1f, 20)] float frequency = 1.0f;
    [SerializeField] [Range(0.1f, 3)] float layerHeight = 1.0f;
    [SerializeField] [Range(0.5f, 1.5f)] float redistribution = 1.0f;
    [SerializeField] [Range(-200000, 200000)] public int seed = 0;
    [SerializeField] bool isTerrainSmooth = false;
    [SerializeField] Material material = null;

    //Function for creating the chunk objects and drawing the map
    public void GenerateMap(int m_seed)
    {
        //Sets seed
        seed = m_seed;

        //Checks for too many vertices in single mesh.
        if (xSize * zSize > 62500)
        {
            Debug.LogError("Map size is too large for a single mesh. Cancelling map generation.");
        }
        else
        {
            //For each chunk on z axis
            for (int z = 0; z < zChunks; z++)
            {
                //For each chunk on x axis
                for (int x = 0; x < xChunks; x++)
                {
                    //Create new terrain object, set name and parent to this. 
                    //Set position and then call function within Chunk Generator
                    GameObject terrain = new GameObject();
                    terrain.name = "Chunk (" + x + ", " + z + ")";
                    terrain.transform.SetParent(transform);
                    terrain.transform.position = new Vector3(x * xSize, 0, z * zSize);
                    ChunkGenerator cg = terrain.AddComponent<ChunkGenerator>();
                    cg.DrawChunk(xSize, zSize, amplitude, frequency, layerHeight, redistribution, seed, xSize * x, zSize * z, isTerrainSmooth, material);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap(seed);
    }
}
