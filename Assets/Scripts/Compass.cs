// Copyright (c) 2019 JensenJ
// NAME: Compass
// PURPOSE: Give direction of object based on angle
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Compass : MonoBehaviour
{

    //Variables
    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest};

    [SerializeField] private Direction currentDirection = Direction.North;

    [SerializeField] int MaxMarkerAmount = 16;
    [SerializeField] Marker[] markers;
    [SerializeField] GameObject markerPrefab = null;
    [SerializeField] GameObject markerManagerPanel = null;

    [SerializeField] PlayerController pc = null;

    [SerializeField] Vector3 targetTransform = Vector3.zero;
    [SerializeField] private float targetRotation = 0.0f;

    private GameObject spawnedDrone = null;

    void Start()
    {
        //Sets array max length for markers
        markers = new Marker[MaxMarkerAmount];
        pc = GetComponent<PlayerController>();
        markerManagerPanel = transform.root.GetChild(1).GetChild(1).gameObject;
    }

    // Update every frame
    void Update()
    {
        if (pc.bHasDeployedDrone)
        {
            spawnedDrone = pc.spawnedDrone;
            if(!(spawnedDrone == null))
            {
                targetTransform = spawnedDrone.transform.position;
                targetRotation = spawnedDrone.transform.rotation.eulerAngles.y;
            }
        }
        else
        {
            spawnedDrone = null;
            targetTransform = transform.position;
            targetRotation = transform.rotation.eulerAngles.y;
        }
        //logs current direction
        print(currentDirection.ToString());

        //Test code for marker system
        if (Input.GetKeyDown(KeyCode.B))
        {
            markerManagerPanel.SetActive(!markerManagerPanel.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RemoveMarker(1);
        }

        //Cycle through if statements until angle is within a bracket then set direction based on angle.
        if(targetRotation >= 360 - 22)
        {
            currentDirection = Direction.North;
        }else if (targetRotation >= 270 + 22)
        {
            currentDirection = Direction.NorthWest;
        }else if(targetRotation >= 270 - 22)
        {
            currentDirection = Direction.West;
        }else if(targetRotation >= 180 + 22)
        {
            currentDirection = Direction.SouthWest;
        }else if(targetRotation >= 180 - 22)
        {
            currentDirection = Direction.South;
        }else if(targetRotation >= 90 + 22)
        {
            currentDirection = Direction.SouthEast;
        }else if(targetRotation >= 90 - 22)
        {
            currentDirection = Direction.East;
        }else if(targetRotation >= 0 + 22)
        {
            currentDirection = Direction.NorthEast;
        }
        else
        {
            currentDirection = Direction.North;
        }
    }

    //Adds marker with specified parameters
    void AddMarker(string m_name, Vector3 m_location, Color m_color)
    {
        //Keeps track of current iteration in for each loop
        int iteration = 0;
        bool bHasFilled = false;
        //Loops through each marker space to check for next available one.
        foreach (Marker i in markers)
        {   
            //Checks whether the space is free for a marker
            if(i.bIsEnabled == false)
            {
                bHasFilled = true;
                //Sets variables for marker
                markers[iteration].index = iteration;
                markers[iteration].bIsEnabled = true;
                markers[iteration].color = m_color;
                markers[iteration].location = m_location;
                markers[iteration].name = m_name;
                GameObject marker = Instantiate(markerPrefab, transform.position, transform.rotation, transform.root.GetChild(2));
                Waypoint waypoint = marker.GetComponent<Waypoint>();
                waypoint.SetWaypointSettings(iteration, true, m_name, m_location, m_color);
                
                //Breaks out of loop
                break;
            }
            //Increases which element we are on.
            iteration++;
        }

        //Checks whether a space was actually able to be filled and prints out if not.
        if(bHasFilled == false)
        {
            print("All marker slots taken.");
        }
    }

    //Clears data of marker at specified index.
    public void RemoveMarker(int index)
    {
        markers[index].index = 0;
        markers[index].bIsEnabled = false;
        markers[index].name = "";
        markers[index].location = Vector3.zero;
        markers[index].color = Color.black;

        Transform markerList = transform.root.GetChild(2);
        for (int i = 0; i < markerList.childCount; i++)
        {
            Waypoint marker = markerList.GetChild(i).GetComponent<Waypoint>();
            if (marker.MarkerID == index)
            {
                marker.RemoveWaypoint();
                break;
            }
        }
    }

    public void NewMarker()
    {
        AddMarker("test", targetTransform, Color.blue);
    }

    public void CloseMarkerManager()
    {
        markerManagerPanel.SetActive(false);
    }
}

//Struct for markers and what they should contain
[System.Serializable]
public struct Marker {
    public int index;
    public bool bIsEnabled;
    public string name;
    public Vector3 location;
    public Color color;
}
