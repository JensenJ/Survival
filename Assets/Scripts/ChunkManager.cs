// Copyright (c) 2019 JensenJ
// NAME: ChunkManager
// PURPOSE: Manages chunk data, such as which chunks are currently loaded

using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] GameObject objectToRaycast;
    [SerializeField] GameObject hitObject;

    MapGenerator mg;
    int xChunkSize;
    int zChunkSize;

    int xChunks;
    int zChunks;

    void Start()
    {
        mg = transform.root.GetChild(4).GetComponent<MapGenerator>();

        xChunkSize = mg.xSize;
        zChunkSize = mg.zSize;

        xChunks = mg.xChunks;
        zChunks = mg.zChunks;
    }

    public void LoadChunks(GameObject objectToRaycast, int renderDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(objectToRaycast.transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            hitObject = hit.transform.gameObject;
        }

        int xPos = (int) hitObject.transform.position.x / xChunkSize;
        int zPos = (int) hitObject.transform.position.z / zChunkSize;

        string[] chunknames = new string[9];
        int[] loadedchunks = new int[9];

        chunknames[0] = "Chunk (" + xPos + ", " + zPos + ")";
        chunknames[1] = "Chunk (" + (xPos + 1) + ", " + zPos + ")";
        chunknames[2] = "Chunk (" + (xPos - 1) + ", " + zPos + ")";
        chunknames[3] = "Chunk (" + xPos + ", " + (zPos + 1) + ")";
        chunknames[4] = "Chunk (" + xPos + ", " + (zPos - 1) + ")";
        chunknames[5] = "Chunk (" + (xPos + 1) + ", " + (zPos - 1) + ")";
        chunknames[6] = "Chunk (" + (xPos - 1) + ", " + (zPos + 1) + ")";
        chunknames[7] = "Chunk (" + (xPos + 1) + ", " + (zPos + 1) + ")";
        chunknames[8] = "Chunk (" + (xPos - 1) + ", " + (zPos - 1) + ")";


        //Add chunks to array
        for (int i = 0; i < mg.transform.childCount; i++)
        {
            mg.transform.GetChild(i).gameObject.SetActive(false);
            for (int j = 0; j < chunknames.Length; j++)
            {
                if (chunknames[j] == mg.transform.GetChild(i).gameObject.name)
                {
                    loadedchunks[j] = mg.transform.GetChild(i).GetSiblingIndex();
                    mg.transform.GetChild(loadedchunks[j]).gameObject.SetActive(true);
                }
            }
        }
    }
}