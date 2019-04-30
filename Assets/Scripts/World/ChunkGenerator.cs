// Copyright (c) 2019 JensenJ
// NAME: ChunkGenerator
// PURPOSE: Generates a chunk of the map

using UnityEngine;
using System;
using System.Collections;

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
    bool[] edgeData;

    //Settings for chunk generation
    int chunkSize = 16;
    float amplitude = 10.0f;
    float frequency = 1.0f;

    //Variables only relevant to this chunk
    [SerializeField] Vector2 offset;
    [SerializeField] public int LoadedBy = 0;

    HeightData[] heightData;
    System.Random mapgen;

    //Draw new chunk with info from map generator.
    public void DrawChunk(int m_chunkSize, float m_amplitude, float m_frequency, int m_seed, 
        Vector2 m_offset, Material m_mat, int m_loaderID, HeightData[] m_heights)
    {
        //Variable assigning
        mapgen = new System.Random(m_seed);
        chunkSize = m_chunkSize;
        amplitude = m_amplitude;
        frequency = m_frequency;
        offset = m_offset;
        LoadedBy = m_loaderID;
        heightData = m_heights;
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(m_mat);

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
        //Functions for generating parts of a mesh
        Vertices();
        Triangles();
        Colours();
        StartCoroutine(Resources());
    }

    //Generates vertices for a mesh
    void Vertices()
    {
        //Seed generation from new seed generator
        int xSeed = mapgen.Next(-100000, 100000);
        int zSeed = mapgen.Next(-100000, 100000);

        frequency /= 1000.0f;

        //Create vertices array
        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        //For each vertex on z axis
        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x <= chunkSize; x++)
            {
                //Generation of noise 
                float xSample = ((x + offset.x) * frequency) + xSeed;
                float zSample = ((z + offset.y) * frequency) + zSeed;
                float y = Mathf.PerlinNoise(xSample, zSample) * amplitude;

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
            //For each height
            for (int j = 0; j < heightData.Length; j++)
            {
                //If the normalised height is less than the height data region's height
                if (vertices[i].y >= heightData[j].height)
                {
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

    IEnumerator Resources()
    {
        //For each layer
        for (int i = 0; i < heightData.Length; i++)
        {
            //For each resource in layer
            for (int j = 0; j < heightData[i].resourceData.Length; j++)
            {
                //Check if there is resource data in the layer
                if (heightData[i].resourceData.Length != 0) {
                    //Get the resource density
                    float density = heightData[i].resourceData[j].density;

                    //Get the resource to spawn
                    GameObject resourceToSpawn = heightData[i].resourceData[j].resourceModel;

                    //For each vertex
                    for (int k = 0; k < vertices.Length; k++)
                    {
                        //Check whether height for resource is correct
                        if (vertices[k].y >= heightData[i].height && vertices[k].y < heightData[i + 1].height) // TODO: Check for i + 1 error in height checking
                        {

                            //Generate random value
                            float randomValue = UnityEngine.Random.Range(0.0001f, 1); // TODO: Make use of Random.Next() for random gen and consistent seed
                            //Compare random value to density
                            if (randomValue < density)
                            {
                                //Spawn new resource object
                                Vector3 position = new Vector3(vertices[k].x + transform.position.x, vertices[k].y, vertices[k].z + transform.position.z);
                                Instantiate(resourceToSpawn, position, Quaternion.identity, transform);
                            }
                        }

                        //Performance for coroutine, every 100 vertices in loop, return to main method
                        if(k % 100 == 0)
                        {
                            yield return null;
                        }
                    }
                }
            }
        }
    }

    //Updates mesh data
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}