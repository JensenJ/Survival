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
    bool bIsGenerating = true;
    string[] chunknames;
    string[] lastchunknames;

    [SerializeField] GameObject hitObject;
    GameObject previousObject = null;
    MapGenerator mg;
    int xChunkSize;
    int zChunkSize;

    //Coroutine
    IEnumerator currentLoadCoroutine;

    void Start()
    {
        cm = transform.root.GetChild(5).GetComponent<ChunkManager>();
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();
        chunkLoaderID = cm.RegisterChunkLoader();

        xChunkSize = mg.xSize;
        zChunkSize = mg.zSize;
    }

    // Update is called once per frame
    void Update()
    {
        //Get chunk below object
        GameObject chunk = GetChunkBelowObject(transform.gameObject, chunkRenderDistance, chunkLoaderID);

        if (chunk != null)
        {
            chunknames = GetSurroundingChunks(new Vector2(chunk.transform.position.x, chunk.transform.position.z));

            //Run loading coroutine
            if (currentLoadCoroutine != null)
            {
                StopCoroutine(currentLoadCoroutine);
                lastchunknames = chunknames;
            }
            currentLoadCoroutine = LoadChunks();
            StartCoroutine(currentLoadCoroutine);
        }

        if (lastchunknames != chunknames)
        {
            print("different");
        }
    }

    int GetChunkLoaderID()
    {
        return chunkLoaderID;
    }

    GameObject GetChunkBelowObject(GameObject m_ObjectToRaycast, int m_RenderDistance, int m_LoaderID)
    {
        //Set render distance
        chunkRenderDistance = m_RenderDistance;

        //Check what is below the object calling the function
        RaycastHit hit;
        if (Physics.Raycast(m_ObjectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            hitObject = hit.transform.gameObject;
        }

        //Optimisation
        if (hitObject == previousObject)
        {
            return null;
        }

        //Set previous object to hitobject
        previousObject = hitObject;
        return previousObject;
    }

    string[] GetSurroundingChunks(Vector2 chunkPos)
    {
        //Gets current chunk coordinate
        int xPos = (int)chunkPos.x / xChunkSize;
        int zPos = (int)chunkPos.y / zChunkSize;

        //Names of chunks to be loaded
        string[] chunknames = new string[chunkRenderDistance * chunkRenderDistance];

        //Loop for filling chunknames array based on surrounding chunks
        for (int z = 0, k = 0; z < chunkRenderDistance; z++)
        {
            for (int x = 0; x < chunkRenderDistance; x++)
            {
                chunknames[k] = "Chunk (" + (xPos + x - ((chunkRenderDistance + 1) / 2)) + ", " + (zPos + z - ((chunkRenderDistance + 1) / 2)) + ")";
                k++;
            }
        }

        return chunknames;
    }

    IEnumerator LoadChunks()
    {
        lastchunknames = chunknames;
        //For each chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            //For each element in the chunk array
            for (int j = 0; j < chunknames.Length; j++)
            {
                string chunkname = chunknames[j];

                //If chunk does not exist, create a new one at this position. This allows creation of endless terrain.
                GameObject chunk = GameObject.Find(chunkname);
                if (chunk == null)
                {

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
            //Destroy the current object only if it is loaded by this chunk
            UnloadChunks(index);
        }
    }

    //Unloads chunks at the specified index only for chunks loaded by this object
    void UnloadChunks(int index)
    {
        if (mg.transform.GetChild(index).GetComponent<ChunkGenerator>().LoadedBy == chunkLoaderID)
        {
            Destroy(mg.transform.GetChild(index).gameObject);
        }
    }

    //Unload all chunks loaded by this object when the object is destroyed
    void OnDestroy()
    {
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            UnloadChunks(i);
        }
        //Unregister this chunk loader
        cm.UnregisterChunkLoader();
    }

}
