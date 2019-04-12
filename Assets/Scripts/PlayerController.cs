// Copyright (c) 2019 JensenJ
// NAME: PlayerController
// PURPOSE: Controls for player character (getting input)

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Compass))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Attributes))]
public class PlayerController : MonoBehaviour
{
    //Variables
    [Header("Drone Settings:")]
    [SerializeField] private GameObject droneToSpawn = null;
    [SerializeField] private Vector3 droneSpawnPos = Vector3.zero;
    [SerializeField] [Range(0, 20)] private float droneRetrievalDistance = 5.0f;
    private bool bCanSpawnDrone = true;

    [Header("Movement Settings:")]
    public bool bCanMove = true;
    [SerializeField] [Range(0, 10)] private float speed = 5.0f;
    [SerializeField] [Range(0, 15)] private float sprintSpeed = 7.0f;
    [SerializeField] [Range(0, 5)] private float sensitivity = 3.0f;
    [SerializeField] [Range(0, 4)] private float jumpForce = 2.0f;

    [Header("Other:")]
    [SerializeField] [Range(4, 16)] private int chunkRenderDistance = 8;
    //References
    private Attributes attributes = null;
    private WaypointManager waypointManager = null;
    private bool bCanUseWaypointManager = true;
    private PlayerMotor motor;
    private Transform droneSpawnLocation;
    private DroneController dc = null;

    [Header("Debug:")]
    public bool bHasDeployedDrone = false;
    public GameObject spawnedDrone = null;
    public Image Crosshair = null;

    // Setup
    void Start()
    {
        //Drone and movement setup
        motor = GetComponent<PlayerMotor>();
        attributes = GetComponent<Attributes>();
        droneSpawnLocation = transform.GetChild(2);
        droneSpawnLocation.position = droneSpawnPos + transform.position;
        waypointManager = transform.root.GetChild(2).GetComponent<WaypointManager>();
        Crosshair = transform.root.GetChild(1).GetChild(3).GetComponent<Image>();
        Cursor.lockState = CursorLockMode.Locked;
        Crosshair.gameObject.SetActive(true);
    }

    // Update every frame
    void Update()
    {

        //Waypoint Manager
        if (Input.GetKeyDown(KeyCode.B) && bCanUseWaypointManager)
        {
            if(waypointManager.waypointManagerPanel.activeSelf == true)
            {
                waypointManager.waypointManagerPanel.SetActive(false);
                waypointManager.waypointEditorPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Crosshair.gameObject.SetActive(true);
                bCanMove = true;
                if (bHasDeployedDrone)
                {
                    dc.bCanMove = true;
                }
            }
            else
            {
                waypointManager.waypointManagerPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Crosshair.gameObject.SetActive(false);
                bCanMove = false;
                if (bHasDeployedDrone)
                {
                    dc.bCanMove = false;
                }
            }
        }
        //Movement
        Move();
        Rotate();
        Jump();
        
        Drone();

        //Chunk Loading
        transform.root.GetChild(5).GetComponent<ChunkManager>().LoadChunks(transform.gameObject, chunkRenderDistance);
    }

    void Drone()
    {
        if (!attributes.bIsDead)
        {
            //Check for drone key
            if (motor.IsGrounded() && bCanSpawnDrone)
            {
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
                        dc = spawnedDrone.GetComponent<DroneController>();
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
        else
        {
            if (bHasDeployedDrone)
            {
                dc.bCanMove = false;
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

            //Sprinting
            if (Input.GetButton("Run") && (xMove != 0.0f || zMove != 0.0f) && !bHasDeployedDrone)
            {
                //Disables stamina regen while running
                attributes.bCanRegenStamina = false;
                //Check for exhausted
                if (!attributes.bIsExhausted)
                {   
                    //drain stamina when player attempts to run
                    attributes.ChangeStaminaLevel(-Time.deltaTime, true);
                    //Stop speed boost when stamina meter is below 10
                    if(!(attributes.GetStaminaLevel() <= 10.0f))
                    {
                        //Apply speed boost
                        velocity = (moveHorizontal + moveVertical).normalized * sprintSpeed;
                    }
                    else
                    {
                        //Apply normal speed
                        velocity = (moveHorizontal + moveVertical).normalized * speed;
                    }
                }
                else
                {
                    //Apply normal speed
                    velocity = (moveHorizontal + moveVertical).normalized * speed;
                }
            }
            else
            {
                //Otherwise allow regen of stamina
                attributes.bCanRegenStamina = true;
                //Apply normal speed
                velocity = (moveHorizontal + moveVertical).normalized * speed;
            }

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