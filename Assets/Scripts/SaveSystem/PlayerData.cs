// Copyright (c) 2019 JensenJ
// NAME: PlayerData
// PURPOSE: Holds data for player for saving and loading

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Current player data
    public float[] position;
    public float playerRot;
    public float speed;
    public float sprintSpeed;
    public float jumpForce;
    public float swimSpeed;

    public PlayerData(PlayerController player)
    {
        //Position
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        //Rotation
        playerRot = player.transform.rotation.eulerAngles.y;

        //Other attributes
        speed = player.speed;
        swimSpeed = player.swimmingSpeed;
        sprintSpeed = player.sprintSpeed;
        jumpForce = player.jumpForce;
    }
}
