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

    string[] chunknames;
    int renderDistance = 4;

    public int numberOfLoaders = 0;

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

    public GameObject GetChunkBelowObject(GameObject m_ObjectToRaycast, int m_RenderDistance, int m_LoaderID)
    {
        //Set render distance
        renderDistance = m_RenderDistance;

        //Check what is below the object calling the function
        RaycastHit hit;
        if (Physics.Raycast(m_ObjectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            hitObject = hit.transform.gameObject;
        }

        //Optimisation
        if(hitObject == previousObject)
        {
            return null;
        }

        //Set previous object to hitobject
        previousObject = hitObject;
        return previousObject;
        //chunknames = GetSurroundingChunks(new Vector2(previousObject.transform.position.x, previousObject.transform.position.z));
    }

    public string[] GetSurroundingChunks(Vector2 chunkPos)
    {
        //Gets current chunk coordinate
        int xPos = (int) chunkPos.x / xChunkSize;
        int zPos = (int) chunkPos.y / zChunkSize;

        //Names of chunks to be loaded
        string[] chunknames = new string[renderDistance * renderDistance * numberOfLoaders];

        //Loop for filling chunknames array based on surrounding chunks
        for (int z = 0, k = 0; z < renderDistance; z++)
        {
            for (int x = 0; x < renderDistance; x++)
            {
                chunknames[k] = "Chunk (" + (xPos + x - ((renderDistance + 1) / 2)) + ", " + (zPos + z - ((renderDistance + 1) / 2)) + ")";
                k++;
            }
        }

        return chunknames;
    }
}