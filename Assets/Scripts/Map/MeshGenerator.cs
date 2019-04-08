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

    [SerializeField] [Range(16, 250)] int xSize = 250;
    [SerializeField] [Range(16, 250)] int zSize = 250;
    [SerializeField] [Range(1, 20)] float amplitude = 10.0f;
    [SerializeField] [Range(0.1f, 20)] float frequency = 1.0f;
    [SerializeField] [Range(0.1f, 3)] float layerHeight = 0.5f;
    [SerializeField] [Range(-200000, 200000)] public int seed = 0;
    [SerializeField] int xOffset = 0;
    [SerializeField] int yOffset = 0;

    [SerializeField] bool isTerrainSmooth = false;
    [SerializeField] public bool liveUpdate = true;
    [SerializeField] public bool changeSeedOnGen = false;
    float maxHeight;
    float minHeight;

    public void DrawMap(int m_seed)
    {
        seed = m_seed;
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
            UpdateMesh();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawMap(seed);
    }

    private void Update()
    {
        UpdateMesh();
    }

    void CreateShape()
    {
        float lfrequency = frequency / 1000.0f;
        //Generating vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                
                float y = Mathf.PerlinNoise(((x + xOffset) * lfrequency) + seed, ((z + yOffset) * lfrequency) + seed) * amplitude;

                if (isTerrainSmooth == false)
                {
                    y = Mathf.Round(y) * layerHeight;
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