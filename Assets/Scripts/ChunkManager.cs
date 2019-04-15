// Copyright (c) 2019 JensenJ
// NAME: ChunkManager
// PURPOSE: Manages chunk data, such as which chunks are currently loaded

using UnityEngine;
using System.Collections;
using System;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] GameObject hitObject;
    GameObject previousObject = null;
    MapGenerator mg;
    int xChunkSize;
    int zChunkSize;

    string[] chunknames;
    int renderDistance = 4;

    public int numberOfLoaders = 0;
    int currentLoaderID = 0;

    //Coroutine
    IEnumerator currentLoadCoroutine;
    bool bIsGenerating = true;

    void Start()
    {
        //Get variables
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();

        xChunkSize = mg.xSize;
        zChunkSize = mg.zSize;
    }

    //Returns a chunk loader ID.
    public int RegisterChunkLoader()
    {
        numberOfLoaders++;
        return numberOfLoaders;
    }

    public void GetChunkBelowObject(GameObject m_ObjectToRaycast, int m_RenderDistance, int m_LoaderID)
    {
        //Set render distance
        renderDistance = m_RenderDistance;
        currentLoaderID = m_LoaderID;

        //Check what is below the object calling the function
        RaycastHit hit;
        if (Physics.Raycast(m_ObjectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            hitObject = hit.transform.gameObject;
        }

        //Optimisation
        if(hitObject == previousObject)
        {
            return;
        }

        //Set previous object to hitobject
        previousObject = hitObject;

        GetSurroundingChunks();

        //Run loading coroutine
        if(currentLoadCoroutine != null)
        {
            StopCoroutine(currentLoadCoroutine);
        }
        currentLoadCoroutine = LoadChunks();
        StartCoroutine(currentLoadCoroutine);
    }

    void GetSurroundingChunks()
    {
        //Gets current chunk coordinate
        int xPos = (int) previousObject.transform.position.x / xChunkSize;
        int zPos = (int) previousObject.transform.position.z / zChunkSize;

        //Names of chunks to be loaded
        chunknames = new string[renderDistance * renderDistance * numberOfLoaders];

        //Loop for filling chunknames array based on surrounding chunks
        int k = 0;
        for (int z = 0; z < renderDistance; z++)
        {
            for (int x = 0; x < renderDistance; x++)
            {
                chunknames[k] = "Chunk (" + (xPos + x - ((renderDistance + 1) / 2)) + ", " + (zPos + z - ((renderDistance + 1) / 2)) + ")";
                k++;
            }
        }
    }
    
    IEnumerator LoadChunks()
    {
        //For each chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            //For each element in the chunk array
            for (int j = 0; j < chunknames.Length; j++)
            {
                //If chunk does not exist, create a new one at this position. This allows creation of endless terrain.
                GameObject chunk = GameObject.Find(chunknames[j]);
                if (chunk == null)
                {
                    string chunkname = chunknames[j];

                    //Get coordinates of chunk from name of chunk
                    int a = chunkname.IndexOf("(");
                    int b = chunkname.IndexOf(",");
                    int x = int.Parse(chunkname.Substring(a + 1, (b - a) - 1));

                    int c = chunkname.IndexOf(")");
                    int z = int.Parse(chunkname.Substring(b + 2, (c - (b + 1)) - 1));

                    //Create new chunk at these coordinates
                    mg.CreateNewChunk(x, z, currentLoaderID);

                    //Coroutine pause point
                    if (!bIsGenerating)
                    {
                        yield return null;
                    }
                }
            }
            //Removes chunks that are not loaded
            RemoveUnloadedChunks(i);
        }
        bIsGenerating = false;
    }

    //Function to remove unloaded chunks
    void RemoveUnloadedChunks(int index)
    {
        //Get position of current transform in array
        string value = mg.transform.GetChild(index).name;
        int pos = Array.IndexOf(chunknames, value);

        //If transform is not in chunknames for loaded chunks
        if (pos == -1)
        {
            //Destroy the current object
            Destroy(mg.transform.GetChild(index).gameObject);

        }
    }
}