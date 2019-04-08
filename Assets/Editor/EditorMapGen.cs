// Copyright (c) 2019 JensenJ
// NAME: EditorMapGen
// PURPOSE: Live editing for the mesh generator, allows generation while in edit mode of editor.

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGenerator meshGen = (MeshGenerator)target;

        if (DrawDefaultInspector())
        {
            if (meshGen.liveUpdate)
            {
                if (meshGen.changeSeedOnGen)
                {
                    meshGen.DrawMap(Random.Range(0, 10000));
                }
                else
                {
                    meshGen.DrawMap(meshGen.seed);
                }
            }
        }

        if (GUILayout.Button("Generate"))
        {
            if (meshGen.changeSeedOnGen)
            {
                meshGen.DrawMap(Random.Range(0, 10000));
            }
            else
            {
                meshGen.DrawMap(meshGen.seed);
            }
        }
    }
}
