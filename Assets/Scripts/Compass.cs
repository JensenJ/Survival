// Copyright (c) 2019 JensenJ
// NAME: Compass
// PURPOSE: Keep track of waypoints and direction
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Compass : MonoBehaviour
{

    //Variables
    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

    [SerializeField] private Direction currentDirection = Direction.North;

    [SerializeField] PlayerController pc = null;

    [SerializeField] private float targetRotation = 0.0f;

    private GameObject spawnedDrone = null;

    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update every frame
    void Update()
    {
        if (pc.bHasDeployedDrone)
        {
            spawnedDrone = pc.spawnedDrone;
            if (!(spawnedDrone == null))
            {
                targetRotation = spawnedDrone.transform.rotation.eulerAngles.y;
            }
        }
        else
        {
            spawnedDrone = null;
            targetRotation = transform.rotation.eulerAngles.y;
        }


        //Cycle through if statements until angle is within a bracket then set direction based on angle.
        if (targetRotation >= 360 - 22)
        {
            currentDirection = Direction.North;
        }
        else if (targetRotation >= 270 + 22)
        {
            currentDirection = Direction.NorthWest;
        }
        else if (targetRotation >= 270 - 22)
        {
            currentDirection = Direction.West;
        }
        else if (targetRotation >= 180 + 22)
        {
            currentDirection = Direction.SouthWest;
        }
        else if (targetRotation >= 180 - 22)
        {
            currentDirection = Direction.South;
        }
        else if (targetRotation >= 90 + 22)
        {
            currentDirection = Direction.SouthEast;
        }
        else if (targetRotation >= 90 - 22)
        {
            currentDirection = Direction.East;
        }
        else if (targetRotation >= 0 + 22)
        {
            currentDirection = Direction.NorthEast;
        }
        else
        {
            currentDirection = Direction.North;
        }
    }
}