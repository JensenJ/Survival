using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public string[] loadedChunks;

    public ChunkData(ChunkLoader loader)
    {
        //Currently loaded chunks
        loadedChunks = loader.chunknames;
    }

}
