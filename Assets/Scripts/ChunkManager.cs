// Copyright (c) 2019 JensenJ
// NAME: ChunkManager
// PURPOSE: Manages chunk data, such as how many chunk loaders are in the scene

using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] public int numberOfLoaders = 0;

    //Adds a chunk loader to the the register and returns a chunk loader ID.
    public int RegisterChunkLoader()
    {
        numberOfLoaders++;
        return numberOfLoaders;
    }

    //Removes a chunk loader from the register
    public void UnregisterChunkLoader()
    {
        numberOfLoaders--;
    }
}