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

    [Header("Movement Settings:")]
    public bool bCanMove = true;
    [SerializeField] [Range(0, 10)] private float speed = 5.0f;
    [SerializeField] [Range(0, 15)] private float sprintSpeed = 7.0f;
    [SerializeField] [Range(0, 5)] private float sensitivity = 3.0f;
    [SerializeField] [Range(0, 4)] private float jumpForce = 2.0f;

    //References
    private Attributes attributes = null;
    private WaypointManager waypointManager = null;
    private bool bCanUseWaypointManager = true;
    private PlayerMotor motor;

    [Header("Debug:")]
    public Image Crosshair = null;
    bool bHasSpawned = false;

    // Setup
    void Start()
    {
        //Movement setup
        motor = GetComponent<PlayerMotor>();
        attributes = GetComponent<Attributes>();
        waypointManager = transform.root.GetChild(2).GetComponent<WaypointManager>();
        Crosshair = transform.root.GetChild(1).GetChild(3).GetComponent<Image>();
        Cursor.lockState = CursorLockMode.Locked;
        Crosshair.gameObject.SetActive(true);
    }

    // Update every frame
    void Update()
    {

        //Spawn player down on the ground
        if (bHasSpawned == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y + 2, transform.position.z);
                bHasSpawned = true;
            }
        }

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
            }
            else
            {
                waypointManager.waypointManagerPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Crosshair.gameObject.SetActive(false);
                bCanMove = false;
            }
        }
        //Movement
        Move();
        Rotate();
        Jump();
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
            if (Input.GetButton("Run") && (xMove != 0.0f || zMove != 0.0f))
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