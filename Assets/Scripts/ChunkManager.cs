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
    void Update()
    {
        //Sort chunks into alphabetical order in the hierarchy
        SortChunks();
    }

    public void GetChunkBelowObject(GameObject m_objectToRaycast, int m_renderDistance)
    {
        //Set render distance
        renderDistance = m_renderDistance;

        //Check what is below the object calling the function
        RaycastHit hit;
        if (Physics.Raycast(m_objectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
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
        chunknames = new string[renderDistance * renderDistance];

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

    // TODO: [R-2] Delete chunks that are inactive, to make looping through children more efficient and remove duplicate chunks upon re-generation
    
    IEnumerator LoadChunks()
    {
        //For each chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            //Deactivate all chunks
            mg.transform.GetChild(i).gameObject.SetActive(false);

            //For each element in the chunk array
            for (int j = 0; j < chunknames.Length; j++)
            {

                //If chunk array element is equal to the current child iteration then set active
                //If the chunk is loaded
                if (chunknames[j] == mg.transform.GetChild(i).gameObject.name)
                {
                    mg.transform.GetChild(mg.transform.GetChild(i).GetSiblingIndex()).gameObject.SetActive(true);
                    //Coroutine pause point
                    if (!bIsGenerating)
                    {
                        yield return null;
                    }
                }

                if (mg.isTerrainEndless == true)
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
                        mg.CreateNewChunk(x, z);
                    }
                }
            }
            RemoveDuplicateChunks();
        }
        bIsGenerating = false;
    }

    void SortChunks()
    {
        //Create new array of chunks
        GameObject[] chunks = new GameObject[mg.transform.childCount];
        //Assign each element in array a chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            chunks[i] = mg.transform.GetChild(i).gameObject;
        }
        //Sort the array
        IComparer comparer = new ChunkSorter();
        Array.Sort(chunks, comparer);

        //Reorder objects in hierarchy based off sorted chunks
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            mg.transform.GetChild(i).SetSiblingIndex(chunks[i].transform.GetSiblingIndex());
        }
    }

    // TODO: [R-2] Remove function as it is temporary to remove duplicate chunks, duplicate chunks should not generate at all.
    void RemoveDuplicateChunks()
    {
        //For each chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            int index = -1;

            //If index isnt 0 (this stops an error)
            if (i != 0)
            {
                //If chunks names are equal between this and the last one in the hierarchy
                if (mg.transform.GetChild(i).name == mg.transform.GetChild(i + index).name)
                {
                    //Delete the duplicate chunk
                    print("called");
                    Destroy(mg.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}

//Chunk sorting
public class ChunkSorter : IComparer
{
    int IComparer.Compare(System.Object x, System.Object y)
    {
        //Sort alphabetically
        return new CaseInsensitiveComparer().Compare(((GameObject)x).name, ((GameObject)y).name);
    }
}