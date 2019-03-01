// Copyright (c) 2019 JensenJ
// NAME: PlayerController
// PURPOSE: Controls for player character (getting input)

using UnityEngine;

[RequireComponent(typeof(Compass))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    //Variables
    [Header("Drone Settings:")]
    [SerializeField] private GameObject droneToSpawn = null;
    [SerializeField] private Vector3 droneSpawnPos = Vector3.zero;
    [SerializeField] private float droneRetrievalDistance = 5.0f;

    [Header("Movement Settings:")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float jumpForce = 2.0f;

    [Header("Debug:")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Transform droneSpawnLocation;
    [SerializeField] private bool bHasDeployedDrone = false;
    [SerializeField] private GameObject spawnedDrone = null;

    [Header("Attributes:")]
    [SerializeField] private bool bIsDead = false;
    [SerializeField] private float maxHealthMeter = 100.0f;
    [SerializeField] private float healthMeter;
    [SerializeField] private float healthMeterDrainSpeed = 20.0f;
    [SerializeField] private float healthPercentage = 100.0f;

    [Space(15)]
    [SerializeField] private bool bIsExhausted = false;
    [SerializeField] private float maxStaminaMeter = 100.0f;
    [SerializeField] private float staminaMeter;
    [SerializeField] private float staminaMeterDrainSpeed = 20.0f;
    [SerializeField] private float staminaPercentage = 100.0f;
    // Setup
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        droneSpawnLocation = transform.GetChild(2);
        droneSpawnLocation.position = droneSpawnPos + transform.position;
        healthMeter = maxHealthMeter;
    }

    // Update every frame
    void Update()
    {
        //Movement
        Move();
        Rotate();
        Jump();
        
        SpawnDrone();

        HealthMeter();
        healthPercentage = (healthMeter / maxHealthMeter) * 100;

        StaminaMeter();
        staminaPercentage = (staminaMeter / maxStaminaMeter) * 100;
    }

    //Function for changing health level, negatives allowed for deducting health
    public void ChangeStaminaLevel(float amount)
    {
        staminaMeter += amount;
    }

    void StaminaMeter()
    {
        //Makes sure health does not go negative.
        if (staminaMeter <= 0.0f)
        {
            bIsExhausted = true;
            staminaMeter = 0.0f;

        }
        //Otherwise, keep normal
        else
        {
            bIsExhausted = false;
        }

        //Makes sure health does not regenerate over the max limit
        if (staminaMeter >= maxStaminaMeter)
        {
            staminaMeter = maxStaminaMeter;
        }
    }

    //Function for changing health level, negatives allowed for deducting health
    public void ChangeHealthLevel(float amount)
    {
        healthMeter += amount;
    }

    void HealthMeter()
    {
        //Makes sure health does not go negative.
        if (healthMeter <= 0.0f)
        {
            bIsDead = true;
            healthMeter = 0.0f;

        }
        //Otherwise, keep normal
        else
        {
            bIsDead = false;
        }

        //Makes sure health does not regenerate over the max limit
        if (healthMeter >= maxHealthMeter)
        {
            healthMeter = maxHealthMeter;
        }
    }

    void SpawnDrone()
    {
        //Check for drone key
        if (motor.IsGrounded()) {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                //Check whether a drone has already been deployed.
                if (!bHasDeployedDrone)
                {
                    //Spawns new drone and disable player controller movement
                    spawnedDrone = Instantiate(droneToSpawn, droneSpawnLocation.position, transform.rotation, transform);
                    bCanMove = false;
                    bHasDeployedDrone = true;
                    motor.Freeze(RigidbodyConstraints.FreezeAll);
                }
                else
                {
                    //Checks whether drone actually exists
                    if (spawnedDrone != null)
                    {
                        //Makes sure drone is within distance to be able to be picked up by player
                        if (spawnedDrone.transform.position.x - transform.position.x <= droneRetrievalDistance &&
                            spawnedDrone.transform.position.y - transform.position.y <= droneRetrievalDistance &&
                            spawnedDrone.transform.position.z - transform.position.z <= droneRetrievalDistance)
                        {
                            //Destroys currently spawned drone and enables movement
                            Destroy(spawnedDrone);
                            bCanMove = true;
                            bHasDeployedDrone = false;
                            motor.Freeze(RigidbodyConstraints.FreezeRotation);
                        }
                        else
                        {
                            Debug.Log("Drone needs to be closer to player in order to retrieve.");
                            bHasDeployedDrone = true;
                            motor.Freeze(RigidbodyConstraints.FreezeAll);
                        }
                    }
                }
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