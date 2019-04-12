// Copyright (c) 2019 JensenJ
// NAME: ChunkManager
// PURPOSE: Manages chunk data, such as which chunks are currently loaded

using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] GameObject hitObject;
    GameObject previousObject = null;
    MapGenerator mg;
    int xChunkSize;
    int zChunkSize;

    void Start()
    {
        //Get variables
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();

        xChunkSize = mg.xSize;
        zChunkSize = mg.zSize;
    }

    public void LoadChunks(GameObject objectToRaycast, int renderDistance)
    {
        //Check what is below the object calling the function
        RaycastHit hit;
        if (Physics.Raycast(objectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
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

        //Gets current chunk coordinate
        int xPos = (int) hitObject.transform.position.x / xChunkSize;
        int zPos = (int) hitObject.transform.position.z / zChunkSize;

        //Names of chunks to be loaded
        string[] chunknames = new string[renderDistance * renderDistance];

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

        //For each chunk
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            //If the chunk is active, skip it, otherwise deactivate it
            if(mg.transform.GetChild(i).gameObject.activeSelf == true)
            {
                mg.transform.GetChild(i).gameObject.SetActive(false);
            }
            //For each element in the chunk array
            for (int j = 0; j < chunknames.Length; j++)
            {
                //If chunk array element is equal to the current child iteration then set active
                //If the chunk is loaded
                if (chunknames[j] == mg.transform.GetChild(i).gameObject.name)
                {
                    mg.transform.GetChild(mg.transform.GetChild(i).GetSiblingIndex()).gameObject.SetActive(true);
                }
            }
        }
    }
}