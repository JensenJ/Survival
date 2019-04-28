// Copyright (c) 2019 JensenJ
// NAME: ChunkData
// PURPOSE: Holds chunk data, such as which are currently loaded

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
