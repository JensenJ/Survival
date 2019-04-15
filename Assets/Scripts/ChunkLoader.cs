// Copyright (c) 2019 JensenJ
// NAME: ChunkLoader
// PURPOSE: Placed on an object that loads chunks

using UnityEngine;
using System.Collections;
using System;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField] [Range(4, 16)] private int chunkRenderDistance = 8;
    [SerializeField] private int chunkLoaderID = 0;
    ChunkManager cm;
    MapGenerator mg;
    bool bIsGenerating = true;
    string[] chunknames;

    //Coroutine
    IEnumerator currentLoadCoroutine;

    void Start()
    {
        cm = transform.root.GetChild(5).GetComponent<ChunkManager>();
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();
        chunkLoaderID = cm.RegisterChunkLoader();
    }

    // Update is called once per frame
    void Update()
    {
        //Get chunk below object
        GameObject chunk = cm.GetChunkBelowObject(transform.gameObject, chunkRenderDistance, chunkLoaderID);

        if(chunk != null)
        {
            chunknames = cm.GetSurroundingChunks(new Vector2(chunk.transform.position.x, chunk.transform.position.z));

            //Run loading coroutine
            if (currentLoadCoroutine != null)
            {
                StopCoroutine(currentLoadCoroutine);
            }
            currentLoadCoroutine = LoadChunks();
            StartCoroutine(currentLoadCoroutine);
        }
    }

    int GetChunkLoaderID()
    {
        return chunkLoaderID;
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
                    mg.CreateNewChunk(x, z, chunkLoaderID);

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
