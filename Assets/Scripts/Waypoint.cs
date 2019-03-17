// Copyright (c) 2019 JensenJ
// NAME: FacePlayer
// PURPOSE: Make game objects face the camera/player

using UnityEngine;
public class Waypoint : MonoBehaviour
{

    Transform objectToFace = null;
    Transform playerPos;
    PlayerController pc = null;
    [SerializeField] float distanceFromPlayer;

    //Setup
    void Start()
    {
        objectToFace = transform.root.GetChild(3).transform;
        playerPos = objectToFace;
        pc = objectToFace.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.bHasDeployedDrone)
        {
            objectToFace = playerPos.GetChild(3);
        }
        else
        {
            objectToFace = playerPos;
        }
        
        if(objectToFace != null)
        {
            distanceFromPlayer = Vector3.Distance(objectToFace.position, transform.position);
            distanceFromPlayer = Mathf.Round(distanceFromPlayer);
            transform.LookAt(objectToFace);
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        else
        {
            Debug.LogError("Object to face is null");
        }
    }
}
