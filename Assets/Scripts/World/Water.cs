// Copyright (c) 2019 JensenJ
// NAME: Water
// PURPOSE: Generates Water and creates wave effects

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Water : MonoBehaviour
{
    //Variables
    Mesh mesh;
    MeshRenderer meshRenderer;
    Vector3[] vertices;
    int chunkSize = 16;
    int[] triangles;
    float waterHeight;
    public WaveOctave[] octaves;
    [SerializeField] Vector2 offset;

    //Coroutines
    IEnumerator currentWaveCoroutine;

    //Function to generate water mesh.
    public void AddWater(int m_chunkSize, float m_waterHeight, WaveOctave[] m_waveOctaves, Vector2 m_offset)
    {
        //Variable assigning
        chunkSize = m_chunkSize;
        waterHeight = m_waterHeight;
        octaves = m_waveOctaves;
        offset = m_offset;
        //Create new mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Water";

        CreateMesh();
        UpdateMesh();
    }

    void Update()
    {
        //Run wave coroutine
        //if (currentWaveCoroutine != null)
        //{
        //    StopCoroutine(currentWaveCoroutine);
        //}
        //Waves();
        UpdateMesh();
    }

    // TODO: Optimise to run much smoother
    IEnumerator Waves()
    {
        while (true)
        {
            //For each vertex on z axis
            for (int i = 0, z = 0; z <= chunkSize; z++)
            {
                //For each vertex on x axis
                for (int x = 0; x <= chunkSize; x++)
                {
                    float y = waterHeight;
                    //Calculate next position in wave, for each octave
                    for (int j = 0; j < octaves.Length; j++)
                    {
                        //If alternate
                        if (octaves[j].alternate)
                        {
                            //Calculate position of vertex using perlin noise, using method 1
                            float xSample = ((x + offset.x) * octaves[j].scale.x) / chunkSize;
                            float zSample = ((z + offset.y) * octaves[j].scale.y) / chunkSize;

                            float perl = Mathf.PerlinNoise(xSample, zSample) * Mathf.PI * 2f;
                            y += Mathf.Cos(perl + octaves[j].speed.magnitude * Time.time) * octaves[j].height;
                        }
                        else
                        {
                            //Calculate position of vertex using perlin noise, using method 2
                            float xSample = ((x + offset.x) * octaves[j].scale.x + Time.time * octaves[j].speed.x) / chunkSize;
                            float zSample = ((z + offset.y) * octaves[j].scale.y + Time.time * octaves[j].speed.y) / chunkSize; 

                            float perl = Mathf.PerlinNoise(xSample, zSample) - 0.5f;
                            y += perl * octaves[j].height;
                        }
                    }
                    //Set vertex position
                    vertices[i].y = y;
                    i++;
                }
            }
            yield return null;
        }
    }

    void CreateMesh()
    {
        //Functions for generating parts of a mesh
        Vertices();
        Triangles();
        //Run wave coroutine
        currentWaveCoroutine = Waves();
        StartCoroutine(currentWaveCoroutine);
    }

    //Generates vertices for a mesh
    void Vertices()
    {
        //Create vertices array
        vertices = new Vector3[(chunkSize + 1) * (chunkSize + 1)];

        //For each vertex on z axis
        for (int i = 0, z = 0; z <= chunkSize; z++)
        {
            //For each vertex on x axis
            for (int x = 0; x <= chunkSize; x++)
            {
                //Set vertex position
                vertices[i] = new Vector3(x, waterHeight, z);
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

    //Updates mesh data
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}

[System.Serializable]
public struct WaveOctave
{
    public Vector2 speed;
    public Vector2 scale;
    public float height;
    public bool alternate;
}
