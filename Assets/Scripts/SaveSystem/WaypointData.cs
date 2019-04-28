// Copyright (c) 2019 JensenJ
// NAME: WaypointData
// PURPOSE: Holds data for waypoints

using UnityEngine;

[System.Serializable]
public class WaypointData
{
    //Waypoint data
    public int[] waypointIDs;
    public string[] waypointNames;
    public float[,] waypointColors;
    public float[,] waypointLocations;
    public bool[] waypointEnabled;
    public bool[] waypointInUse;

    public WaypointData(WaypointManager manager)
    {
        //Waypoint array for struct
        waypointIDs = new int[manager.waypoints.Length];
        waypointNames = new string[waypointIDs.Length];
        waypointColors = new float[waypointIDs.Length, 3];
        waypointEnabled = new bool[waypointIDs.Length];
        waypointInUse = new bool[waypointIDs.Length];
        waypointLocations = new float[waypointIDs.Length, 3];

        //Set all values
        for (int i = 0; i < waypointIDs.Length; i++)
        {
            waypointIDs[i] = manager.waypoints[i].index;
            waypointNames[i] = manager.waypoints[i].name;
            waypointEnabled[i] = manager.waypoints[i].bIsEnabled;
            waypointInUse[i] = manager.waypoints[i].bIsInUse;

            waypointColors[i, 0] = manager.waypoints[i].color.r;
            waypointColors[i, 1] = manager.waypoints[i].color.g;
            waypointColors[i, 2] = manager.waypoints[i].color.b;

            waypointLocations[i, 0] = manager.waypoints[i].location.x;
            waypointLocations[i, 1] = manager.waypoints[i].location.y;
            waypointLocations[i, 2] = manager.waypoints[i].location.z;

        }
    }
}
