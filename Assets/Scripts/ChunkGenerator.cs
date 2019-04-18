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
    MeshRenderer meshRenderer;
    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    MapGenerator mg;

    //Settings for chunk generation
    int chunkSize = 16;
    float amplitude = 10.0f;
    float frequency = 1.0f;
    float layerHeight = 1.0f;
    bool isTerrainSmooth = false;

    [SerializeField] Vector2 offset;
    [SerializeField] float maxHeight = float.MinValue;
    [SerializeField] float minHeight = float.MaxValue;
    [SerializeField] public int LoadedBy = 0;

    HeightData[] heightData;
    System.Random mapgen;

    // TODO: [R-2] Make use of coroutines for better performance on chunk load

    //Draw new map with seed
    public void DrawChunk(int m_chunkSize, float m_amplitude, float m_frequency, float m_layerHeight, int m_seed, 
        Vector2 m_offset, bool m_bIsTerrainSmooth, Material m_mat, int m_loaderID, HeightData[] m_heights)
    {
        mapgen = new System.Random(m_seed);
        //Set variables
        chunkSize = m_chunkSize;
        amplitude = m_amplitude;
        frequency = m_frequency;
        layerHeight = m_layerHeight;
        offset = m_offset;
        isTerrainSmooth = m_bIsTerrainSmooth;
        LoadedBy = m_loaderID;
        heightData = m_heights;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(m_mat);
        mg = transform.GetComponentInParent<MapGenerator>();
        
        maxHeight = float.MinValue;
        minHeight = float.MaxValue;

        //Create new mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Terrain";

        //Create mesh and update it.
        CreateMesh();
        UpdateMesh();

        //Collisions
        meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.enabled = true;
        meshCollider.sharedMesh = mesh;
    }


    void CreateMesh()
    {
        Vertices();
        Triangles();
        Colours();
    }

    //Generates vertices for a mesh
    void Vertices()
    {
        int xSeed = mapgen.Next(-100000, 100000);
        int zSeed = mapgen.Next(-100000, 100000);

        float lamplitude = amplitude;

        if(isTerrainSmooth == false)
        {
            lamplitude = amplitude / layerHeight;
        }
        float lfrequency = frequency / 1000.0f;
        //Generating vertices
        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        //For each vertex on z axis
        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x <= chunkSize; x++)
            {

                //Generation of noise 
                float xSample = ((x + offset.x) * lfrequency) + xSeed;
                float zSample = ((z + offset.y) * lfrequency) + zSeed;
                float y = Mathf.PerlinNoise(xSample, zSample) * lamplitude;

                //Round vertices to nearest integer if terrain is not smooth.
                if (isTerrainSmooth == false)
                {
                    y = Mathf.Round(y) * layerHeight;
                }

                //Set new max and min height.
                if (y > maxHeight)
                {
                    maxHeight = y;
                }
                if (y < minHeight)
                {
                    minHeight = y;
                }

                //Set vertex position
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
    }

    //Generates triangles for a mesh
    void Triangles()
    {
        //Create new triangle array
        triangles = new int[chunkSize * chunkSize * 6];
        int vert = 0;
        int tris = 0;
        //For each vertex on z axis
        for (int z = 0; z < chunkSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x < chunkSize; x++)
            {
                //Creates 2 triangles to form a square
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + chunkSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + chunkSize + 1;
                triangles[tris + 5] = vert + chunkSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void Colours()
    {
        //New colours array with same length as vertices
        colours = new Color[vertices.Length];
        //For each vertex
        for (int i = 0; i < colours.Length; i++)
        {
            //Use global height values
            float currentHeight = vertices[i].y;

            //Better method, but needs to be global normalisation instead of local, 
            //global could be retrieved by generating max possible values at current gen settings and storing it in manager
            //to be retrieved here and used in this normalisation

            // TODO: Make global normalisation instead of local
            //currentHeight = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
            
            //For each height
            for (int j = 0; j < heightData.Length; j++)
            {
                //If the normalised height is less than the height data region's height
                if (currentHeight >= heightData[j].height)
                {
                    //print("I: " + i + ", " + heightData[j].layerName);
                    //Set the colour to the height data region's colour
                    colours[i] = heightData[j].colour;
                }
                //Otherwise break out the loop to prevent all being the same colour
                else
                {
                    break;
                }
            }
        }
    }

    void UpdateMesh()
    {
        //Refreshes mesh data
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}