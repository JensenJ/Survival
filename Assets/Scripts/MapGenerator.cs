// Copyright (c) 2019 JensenJ
// NAME: MapGenerator
// PURPOSE: Generates a map

using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //Settings for map generation
    [SerializeField] [Range(1, 5)] int xChunks = 2;
    [SerializeField] [Range(1, 5)] int zChunks = 2;

    [SerializeField] [Range(16, 250)] int xSize = 250;
    [SerializeField] [Range(16, 250)] int zSize = 250;
    [SerializeField] [Range(1, 20)] float amplitude = 10.0f;
    [SerializeField] [Range(0.1f, 20)] float frequency = 1.0f;
    [SerializeField] [Range(0.1f, 3)] float layerHeight = 1.0f;
    [SerializeField] [Range(0.5f, 1.5f)] float redistribution = 1.0f;
    [SerializeField] [Range(-200000, 200000)] public int seed = 0;

    //Settings for map generation in editor.
    [SerializeField] bool isTerrainSmooth = false;
    [SerializeField] public bool liveUpdate = true;
    [SerializeField] public bool changeSeedOnGen = false;

    [SerializeField] Material material;
    public void GenerateMap(int m_seed)
    {
        seed = m_seed;

        for (int z = 0; z < zChunks; z++)
        {
            for (int x = 0; x < xChunks; x++)
            {

                GameObject terrain = new GameObject();
                terrain.name = "Chunk (" + x + ", " + z + ")";
                terrain.transform.SetParent(transform);
                terrain.transform.position = new Vector3(x * xSize, 0, z * zSize);
                ChunkGenerator cg = terrain.AddComponent<ChunkGenerator>();
                cg.DrawChunk(xSize, zSize, amplitude, frequency, layerHeight, redistribution, seed, xSize * x, zSize * z, isTerrainSmooth, liveUpdate, material);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        liveUpdate = false;
        GenerateMap(seed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
