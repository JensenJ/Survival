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
    public float octaves = 1.0f;
    //public float persistence = 1.0f;
    public float amplitude = 10.0f;
    public float frequency = 1.0f;
    public float layerHeight = 0.5f;

    public float maxHeight;
    public float minHeight;

    public bool isTerrainSmooth = false;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Terrain";
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

    void GetBiomeFromHeight(float elevation)
    {
        float e = elevation / maxHeight;
        if(e < 0.1f)
        {
            //print("Water");
        }else if( e < 0.2f)
        {
            //print("Beach");
        }else if(e < 0.3f)
        {
            //print("Plains");
        }else if(e < 0.5f)
        {
            //print("Forest");
        }else if(e < 0.8f)
        {
            //print("Hills");
        }
        else
        {
            //print("Mountains");
        }
    }
    void CreateShape()
    {
        float lfrequency = frequency / 10.0f;
        //Generating vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0;

                for (int j = 1; j < octaves + 1; j++)
                {
                    float freq = (j * octaves) / 100;
                    //float amp = j * octaves;
                    float amp = 1 / j;

                    y += Mathf.PerlinNoise(x * freq * lfrequency, z * freq * lfrequency) * amp * amplitude;
                }
                if (isTerrainSmooth == false)
                {
                    y = Mathf.Round(y);
                }
                vertices[i] = new Vector3(x, y, z);
                
                if (y > maxHeight)
                {
                    maxHeight = y;
                }
                if(y < minHeight)
                {
                    minHeight = y;
                }
                GetBiomeFromHeight(vertices[i].y);
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