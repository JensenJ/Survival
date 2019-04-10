// Copyright (c) 2019 JensenJ
// NAME: ChunkGenerator
// PURPOSE: Generates a chunk of the map

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkGenerator : MonoBehaviour
{
    //Data related to mesh
    Mesh mesh;
    MeshCollider meshCollider;
    Vector3[] vertices;
    int[] triangles;

    //Settings for chunk generation
    [SerializeField] int xSize = 250;
    [SerializeField] int zSize = 250;
    [SerializeField] float amplitude = 10.0f;
    [SerializeField] float frequency = 1.0f;
    [SerializeField] float layerHeight = 1.0f;
    [SerializeField] float redistribution = 1.0f;
    [SerializeField] public int seed = 0;
    [SerializeField] int xOffset = 0;
    [SerializeField] int yOffset = 0;

    //Settings for map generation in editor.
    [SerializeField] bool isTerrainSmooth = false;
    [SerializeField] public bool liveUpdate = true;
    [SerializeField] public bool changeSeedOnGen = false;

    [SerializeField] float maxHeight = float.MinValue;
    [SerializeField] float minHeight = float.MaxValue;

    //Draw new map with seed
    public void DrawChunk(int m_xSize, int m_zSize, float m_amplitude, float m_frequency, float m_layerHeight, float m_redistribution, int m_seed, int m_xOffset, int m_yOffset, bool m_bIsTerrainSmooth, bool m_bIsLiveUpdate, Material m_mat)
    {
        //Set variables
        xSize = m_xSize;
        zSize = m_zSize;
        amplitude = m_amplitude;
        frequency = m_frequency;
        layerHeight = m_layerHeight;
        redistribution = m_redistribution;
        seed = m_seed;
        xOffset = m_xOffset;
        yOffset = m_yOffset;
        isTerrainSmooth = m_bIsTerrainSmooth;
        liveUpdate = m_bIsLiveUpdate;
        GetComponent<MeshRenderer>().sharedMaterial = m_mat;

        maxHeight = float.MinValue;
        minHeight = float.MaxValue;

        //Create new mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Terrain";

        //Check for too many vertices in single mesh.
        if (xSize * zSize > 62500)
        {
            Debug.LogError("Map size is too large for a single mesh. Cancelling map generation.");
        }
        else
        {
            //Create mesh and update it.
            CreateMesh();
            UpdateMesh();
        }

        meshCollider = gameObject.GetComponent<MeshCollider>();
        //Collisions, do not generate if live update as performance takes a hit, only generates on actual playing and when generate button is pressed
        if(liveUpdate == false)
        {
            meshCollider.enabled = true;
            meshCollider.sharedMesh = mesh;
        }
        else
        {
            meshCollider.enabled = false;
        }
    }

    //void Start()
    //{
    //    liveUpdate = false;
    //    DrawChunk(seed);
    //}

    private void Update()
    {
        //Update mesh every frame for changes, e.g. changes in landscape
        UpdateMesh();
    }

    void CreateMesh()
    {
        Vertices();
        Triangles();
    }

    //Generates vertices for a mesh
    void Vertices()
    {
        float lamplitude = amplitude;
        if(isTerrainSmooth == false)
        {
            lamplitude = amplitude / layerHeight;
        }
        float lfrequency = frequency / 1000.0f;
        //Generating vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        //For each vertex on z axis
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x <= xSize; x++)
            {

                //Generation of noise 
                float xSample = ((x + xOffset) * lfrequency) + seed;
                float zSample = ((z + yOffset) * lfrequency) + seed;
                float y = Mathf.PerlinNoise(xSample, zSample) * lamplitude;

                //Redistribution, adding 1 and then removing it prevents bug with mesh in low height areas
                y++;
                y = Mathf.Pow(y, redistribution);
                y--;
                //Round vertices to nearsst integer if terrain is unsmooth.
                if (isTerrainSmooth == false)
                {
                    y = Mathf.Round(y) * layerHeight;
                }
                //Set vertex position
                vertices[i] = new Vector3(x, y, z);

                //Set new max and min height.
                if (y > maxHeight)
                {
                    maxHeight = y;
                }
                if (y < minHeight)
                {
                    minHeight = y;
                }
                i++;
            }
        }
    }

    //Generates triangles for a mesh
    void Triangles()
    {
        //Create new triangle array
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        //For each vertex on z axis
        for (int z = 0; z < zSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x < xSize; x++)
            {
                //Creates 2 triangles to form a square
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        //Refreshes mesh data
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}