// Copyright (c) 2019 JensenJ
// NAME: ChunkLoader
// PURPOSE: Placed on an object that loads chunks

using UnityEngine;
using System.Collections;
using System;

public class ChunkLoader : MonoBehaviour
{
    //Variables
    [SerializeField] [Range(5, 32)] private int chunkRenderDistance = 8;
    [SerializeField] private int chunkLoaderID = 0;
    [SerializeField] GameObject hitObject;
    bool bIsGenerating = true;
    string[] chunknames;
    GameObject previousObject = null;
    MapGenerator mg;
    ChunkManager cm;

    //Coroutines
    IEnumerator currentLoadCoroutine;
    IEnumerator currentUnloadCoroutine;

    void Start()
    {
        //Setup
        cm = transform.root.GetChild(5).GetComponent<ChunkManager>();
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();
        chunkLoaderID = cm.RegisterChunkLoader();
    }

    // Update is called once per frame
    void Update()
    {
        //Get chunk below object
        GameObject chunk = GetChunkBelowObject(transform.gameObject, chunkRenderDistance);

        if (chunk != null)
        {
            //Get chunknames from the surrounding chunks
            chunknames = GetSurroundingChunks(new Vector2(chunk.transform.position.x, chunk.transform.position.z));


            //Run loading coroutine
            if (currentLoadCoroutine != null)
            {
                StopCoroutine(currentLoadCoroutine);
            }
            currentLoadCoroutine = LoadChunks();
            StartCoroutine(currentLoadCoroutine);

        }

        //Runs the unloading coroutine
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            if (currentUnloadCoroutine != null)
            {
                StopCoroutine(currentUnloadCoroutine);
            }
            currentUnloadCoroutine = UnloadChunks(i);
            StartCoroutine(currentUnloadCoroutine);
        }
    }

    GameObject GetChunkBelowObject(GameObject m_ObjectToRaycast, int m_RenderDistance)
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

    //Returns the names of the surrounding chunks
    string[] GetSurroundingChunks(Vector2 chunkPos)
    {
        //Gets current chunk coordinate
        int xPos = (int)chunkPos.x / mg.chunkSize;
        int zPos = (int)chunkPos.y / mg.chunkSize;

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

    //Coroutine for loading chunks
    IEnumerator LoadChunks()
    {
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
            //Coroutine pause point
            yield return null;
        }
        bIsGenerating = false;
    }

    //Removes chunks that are not loaded
    IEnumerator UnloadChunks(int i)
    {
        //Get position of current transform in array
        string value = mg.transform.GetChild(i).name;
        int pos = Array.IndexOf(chunknames, value);

        //If transform is not in chunknames for loaded chunks
        if (pos == -1)
        {
            //Destroy the current object only if it is loaded by this chunk
            if (mg.transform.GetChild(i).GetComponent<ChunkGenerator>().LoadedBy == chunkLoaderID)
            {
                Destroy(mg.transform.GetChild(i).gameObject);
            }
        }
        yield return null;
        
    }

    //Unload all chunks loaded by this object when the object is destroyed
    void OnDestroy()
    {
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            if (mg.transform.GetChild(i).GetComponent<ChunkGenerator>().LoadedBy == chunkLoaderID)
            {
                Destroy(mg.transform.GetChild(i).gameObject);
            }
        }
        //Unregister this chunk loader
        cm.UnregisterChunkLoader();
    }
}
