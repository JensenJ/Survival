// Copyright (c) 2019 JensenJ
// NAME: ChunkLoader
// PURPOSE: Placed on an object that loads chunks

using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] [Range(4, 16)] private int chunkRenderDistance = 8;
    [SerializeField] private int chunkLoaderID = 0;
    ChunkManager cm;

    void Start()
    {
        cm = transform.root.GetChild(5).GetComponent<ChunkManager>();
        chunkLoaderID = cm.RegisterChunkLoader();
    }

    // Update is called once per frame
    void Update()
    {
        //Chunk Loading
        cm.GetChunkBelowObject(transform.gameObject, chunkRenderDistance, chunkLoaderID);
    }
}
