// Copyright (c) 2019 JensenJ
// NAME: Water
// PURPOSE: Generates Water and creates wave effects

using UnityEngine;

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

    //Function to generate water mesh.
    public void AddWater(int m_chunkSize, float m_waterHeight)
    {
        //Variable assigning
        chunkSize = m_chunkSize;
        waterHeight = m_waterHeight;
        //Create new mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Water";

        CreateMesh();
        UpdateMesh();
    }

    void CreateMesh()
    {
        //Functions for generating parts of a mesh
        Vertices();
        Triangles();
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
