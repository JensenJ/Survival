// Copyright (c) 2019 JensenJ
// NAME: MeshGenerator
// PURPOSE: Generates a mesh for the map

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    public int xSize = 250;
    public int zSize = 250;
    public float frequency = 0.5f;
    public float amplitude = 10.0f;
    public float heightDetail = 2.0f;
    public float layerHeight = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (xSize * zSize > 62500)
        {
            Debug.LogError("Map size is too large for a single mesh. Cancelling map generation.");
        }
        else
        {
            CreateShape();
        }
    }

    private void Update()
    {                    
        UpdateMesh();    
    }                    
                         
    void CreateShape()
    {
        //Generating vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        frequency /= 10;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.Round(Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude * heightDetail) * layerHeight;
                vertices[i] = new Vector3(x, y, z);

                i++;
            }
        }


        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
            
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
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}