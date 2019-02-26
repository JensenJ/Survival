// Copyright (c) 2019 JensenJ
// NAME: PlayerController
// PURPOSE: Controls for player character (getting input)

using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    [Header("Drone Settings")]
    [SerializeField] private GameObject droneToSpawn;
    [SerializeField] private bool bHasDeployedDrone = false;
    [SerializeField] private float droneHeightSpawnDistance = 0.5f;

    //Variables
    [Header("Movement Settings")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float jumpForce = 2.0f;

    private PlayerMotor motor;
    private GameObject spawnedDrone = null;
    // Setup
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // Update movement
    void Update()
    {
        Move();
        Rotate();
        Jump();

        SpawnDrone();
    }

    void SpawnDrone()
    {
        //Check for drone key
        if (Input.GetKeyDown(KeyCode.Z))
        {   
            //Check whether a drone has already been deployed.
            if (!bHasDeployedDrone)
            {
                //Spawns new drone and disable player controller movement
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + droneHeightSpawnDistance, transform.position.z);
                spawnedDrone = Instantiate(droneToSpawn, spawnPos, transform.rotation, transform);
                bCanMove = false;
                bHasDeployedDrone = true;
            }
            else
            {
                //Checks whether drone actually exists
                if (spawnedDrone != null)
                {
                    //Destroys currently spawned drone and enables movement
                    Destroy(spawnedDrone);
                    bCanMove = true;
                }
                bHasDeployedDrone = false;
            }
            
        }
    }

    //Movement
    void Move()
    {
        Vector3 velocity = Vector3.zero;
        //Checks whether player can move
        if (bCanMove)
        {
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;

            velocity = (moveHorizontal + moveVertical).normalized * speed;

        }
        motor.Move(velocity);
    }

    //Rotation
    void Rotate()
    {
        Vector3 rotation = Vector3.zero;
        float CameraRotationX = 0.0f;
        //Checks whether player can move
        if (bCanMove)
        {
            float yRot = Input.GetAxisRaw("Mouse X");
            float xRot = Input.GetAxisRaw("Mouse Y");

            rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
            CameraRotationX = xRot * sensitivity;
        }
        motor.Rotate(rotation);
        motor.RotateCamera(CameraRotationX);
    }

    //Jumping
    void Jump()
    {
        float power = 0.0f;
        //Checks whether player can move
        if (bCanMove)
        {
            if (Input.GetButtonDown("Jump"))
            {
                power = jumpForce;
            }
            else
            {
                power = 0.0f;
            }
        }
        motor.Jump(power);
    }
}