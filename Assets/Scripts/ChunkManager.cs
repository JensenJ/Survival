// Copyright (c) 2019 JensenJ
// NAME: ChunkManager
// PURPOSE: Manages chunk data, such as which chunks are currently loaded

using UnityEngine;
using System.Collections;

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
            }
        }
        bIsGenerating = false;
    }
}