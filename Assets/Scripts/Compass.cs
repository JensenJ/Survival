// Copyright (c) 2019 JensenJ
// NAME: Compass
// PURPOSE: Give Direction of object based on angle
using UnityEngine;

public class Compass : MonoBehaviour
{

    //Variables
    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest};

    [SerializeField] private float rotation = 0.0f;
    [SerializeField] private Direction currentDirection = Direction.North;

    // Update every frame
    void Update()
    {
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
}
