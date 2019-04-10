// Copyright (c) 2019 JensenJ
// NAME: EditorMapGen
// PURPOSE: Live editing for the mesh generator, allows generation while in edit mode of editor.

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.liveUpdate)
            {
                if (mapGen.changeSeedOnGen)
                {
                    mapGen.DrawMap(Random.Range(-200000, 200000));
                }
                else
                {
                    mapGen.DrawMap(mapGen.seed);
                }
            }
        }

        if (GUILayout.Button("Generate"))
        {
            bool livegen = mapGen.liveUpdate;
            mapGen.liveUpdate = false;
            if (mapGen.changeSeedOnGen)
            {
                mapGen.DrawMap(Random.Range(-200000, 200000));
            }
            else
            {
                mapGen.DrawMap(mapGen.seed);
            }
            mapGen.liveUpdate = livegen;
        }
    }
}
