using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //Current player data
    public float[] position;
    public float[] rotation;
    public float speed;
    public float sprintSpeed;
    public float jumpForce;

    public PlayerData(PlayerController player)
    {
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        rotation = new float[3];
        rotation[0] = player.transform.GetChild(1).rotation.eulerAngles.x;
        rotation[1] = player.transform.rotation.eulerAngles.y;
        rotation[2] = player.transform.rotation.eulerAngles.z;

        speed = player.speed;
        sprintSpeed = player.sprintSpeed;
        
    }
}
