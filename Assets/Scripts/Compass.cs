// Copyright (c) 2019 JensenJ
// NAME: Compass
// PURPOSE: Give direction of object based on angle
using UnityEngine;

public class Compass : MonoBehaviour
{

    //Variables

    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest};

    [SerializeField] private float rotation = 0.0f;
    [SerializeField] private Direction currentDirection = Direction.North;

    [SerializeField] int MaxMarkerAmount = 16;
    [SerializeField] Marker[] markers;

    void Start()
    {
        //Sets array max length for markers
        markers = new Marker[MaxMarkerAmount];
    }

    // Update every frame
    void Update()
    {
        
        //Test code for marker system
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddMarker("test", transform.position, Color.blue);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RemoveMarker(1);
        }
        //Update rotation
        rotation = transform.rotation.eulerAngles.y;

        //Cycle through if statements until angle is within a bracket then set direction based on angle.
        if(rotation >= 360 - 22)
        {
            currentDirection = Direction.North;
        }else if (rotation >= 270 + 22)
        {
            currentDirection = Direction.NorthWest;
        }else if(rotation >= 270 - 22)
        {
            currentDirection = Direction.West;
        }else if(rotation >= 180 + 22)
        {
            currentDirection = Direction.SouthWest;
        }else if(rotation >= 180 - 22)
        {
            currentDirection = Direction.South;
        }else if(rotation >= 90 + 22)
        {
            currentDirection = Direction.SouthEast;
        }else if(rotation >= 90 - 22)
        {
            currentDirection = Direction.East;
        }else if(rotation >= 0 + 22)
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
                markers[iteration].bIsEnabled = true;
                markers[iteration].color = m_color;
                markers[iteration].location = m_location;
                markers[iteration].name = m_name;
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
    void RemoveMarker(int index)
    {
        markers[index].bIsEnabled = false;
        markers[index].name = "";
        markers[index].location = Vector3.zero;
        markers[index].color = Color.black;
    }
}

//Struct for markers and what they shouuld contain
[System.Serializable]
public struct Marker {
    public bool bIsEnabled;
    public string name;
    public Vector3 location;
    public Color color;
}
