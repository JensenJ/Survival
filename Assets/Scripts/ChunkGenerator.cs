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
    int xSize = 250;
    int zSize = 250;
    float amplitude = 10.0f;
    float frequency = 1.0f;
    float layerHeight = 1.0f;
    float redistribution = 1.0f;
    public int seed = 0;
    bool isTerrainSmooth = false;

    [SerializeField] int xOffset = 0;
    [SerializeField] int yOffset = 0;
    [SerializeField] float maxHeight = float.MinValue;
    [SerializeField] float minHeight = float.MaxValue;
    [SerializeField] public int LoadedBy = 0;

    HeightData[] heightData;

    // TODO: [R-2] Make use of coroutines for better performance on chunk load

    private void Update()
    {
        //Update mesh every frame for changes, e.g. changes in landscape
        UpdateMesh();
    }

    //Draw new map with seed
    public void DrawChunk(int m_xSize, int m_zSize, float m_amplitude, float m_frequency, float m_layerHeight, float m_redistribution, 
        int m_seed, int m_xOffset, int m_yOffset, bool m_bIsTerrainSmooth, Material m_mat, int m_loaderID, HeightData[] m_heights)
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

    void Colours()
    {
        //New colours array with same length as vertices
        colours = new Color[vertices.Length];
        //For each vertex
        for (int i = 0; i < colours.Length; i++)
        {
            //Get the local normalised height
            float currentHeight = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
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

        //Prints the colour of every vertex
        //for (int i = 0; i < colours.Length; i++)
        //{
        //    print("R: " + colours[i].r + ", G: " + colours[i].g + ", B: " + colours[i].b);
        //}

        //Create texture from colour data and apply to the mesh
        Texture2D texture = new Texture2D(xSize, zSize);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colours);
        texture.Apply();

        meshRenderer.material.mainTexture = texture;
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