// Copyright (c) 2019 JensenJ
// NAME: PlayerController
// PURPOSE: Controls for player character (getting input)

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Compass))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    //Variables
    [Header("Drone Settings:")]
    [SerializeField] private GameObject droneToSpawn = null;
    [SerializeField] private Vector3 droneSpawnPos = Vector3.zero;
    [SerializeField] private float droneRetrievalDistance = 5.0f;
    [SerializeField] private bool bCanSpawnDrone = true;

    [Header("Movement Settings:")]
    [SerializeField] public bool bCanMove = true;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float jumpForce = 2.0f;

    [Header("Attributes:")]
    //Health
    [SerializeField] private bool bIsDead = false;
    [SerializeField] private bool bCanRegenHealth = true;
    [SerializeField] private float maxHealthMeter = 100.0f;
    [SerializeField] private float healthMeter;
    [SerializeField] private float healthMeterDrainSpeed = 1.0f;
    [SerializeField] private float healthMeterRegenSpeed = 0.5f;
    [SerializeField] private float healthPercentage = 1.0f;
    [SerializeField] private Image healthBar = null;

    [Space(15)]
    //Stamina
    [SerializeField] private bool bIsExhausted = false;
    [SerializeField] private bool bCanRegenStamina = true;
    [SerializeField] private float maxStaminaMeter = 100.0f;
    [SerializeField] private float staminaMeter;
    [SerializeField] private float staminaMeterDrainSpeed = 10.0f;
    [SerializeField] private float staminaMeterRegenSpeed = 3.0f;
    [SerializeField] private float staminaPercentage = 1.0f;
    [SerializeField] private Image staminaBar = null;

    [Space(15)]
    //Hunger
    [SerializeField] private bool bIsStarving = false;
    [SerializeField] private bool bCanRegenHunger = false;
    [SerializeField] private float maxHungerMeter = 100.0f;
    [SerializeField] private float hungerMeter;
    [SerializeField] private float hungerMeterDrainSpeed = 0.25f;
    [SerializeField] private float hungerMeterRegenSpeed = 10.0f;
    [SerializeField] private float hungerPercentage = 1.0f;
    [SerializeField] private Image hungerBar = null;

    [Space(15)]
    //Thirst
    [SerializeField] private bool bIsDehydrated = false;
    [SerializeField] private bool bCanRegenThirst = false;
    [SerializeField] private float maxThirstMeter = 100.0f;
    [SerializeField] private float thirstMeter;
    [SerializeField] private float thirstMeterDrainSpeed = 0.125f;
    [SerializeField] private float thirstMeterRegenSpeed = 10.0f;
    [SerializeField] private float thirstPercentage = 1.0f;
    [SerializeField] private Image thirstBar = null;

    [Header("Waypoints:")]
    [SerializeField] private WaypointManager waypointManager = null;
    [SerializeField] private bool bCanUseWaypointManager = true;

    [Header("Debug:")]
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Transform droneSpawnLocation;
    [SerializeField] public bool bHasDeployedDrone = false;
    [SerializeField] public GameObject spawnedDrone = null;
    // Setup
    void Start()
    {
        //Drone and movement setup
        motor = GetComponent<PlayerMotor>();
        droneSpawnLocation = transform.GetChild(2);
        droneSpawnLocation.position = droneSpawnPos + transform.position;
        waypointManager = transform.root.GetChild(2).GetComponent<WaypointManager>();

        AttributesSetup();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void AttributesSetup()
    {
        //UI setup
        Transform panelTransform = transform.root.GetChild(1).GetChild(0).GetChild(0);
        healthBar  = panelTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        staminaBar = panelTransform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
        thirstBar  = panelTransform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>();
        hungerBar  = panelTransform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();

        healthBar.fillAmount = healthPercentage;
        staminaBar.fillAmount = staminaPercentage;
        thirstBar.fillAmount = thirstPercentage;
        hungerBar.fillAmount = hungerPercentage;
        
        //Makes sure max variables are not negative
        
        //Health
        if (maxHealthMeter <= 0.0f)
        {
            maxHealthMeter = 100.0f;
        }

        //Stamina
        if (maxStaminaMeter <= 0.0f)
        {
            maxStaminaMeter = 100.0f;
        }

        //Hunger
        if (maxHungerMeter <= 0.0f)
        {
            maxHungerMeter = 100.0f;
        }

        //Thirst
        if (maxThirstMeter <= 0.0f)
        {
            maxThirstMeter = 100.0f;
        }

        //Attributes setup
        healthMeter = maxHealthMeter;
        staminaMeter = maxStaminaMeter;
        hungerMeter = maxHungerMeter;
        thirstMeter = maxThirstMeter;
        
        //Makes sure drain and regen variables do not go negative when set

        //Health
        if (healthMeterDrainSpeed <= 0.0f)
        {
            healthMeterDrainSpeed = 1.0f;
        }
        if (healthMeterRegenSpeed <= 0.0f)
        {
            healthMeterRegenSpeed = 0.5f;
        }

        //Stamina
        if (staminaMeterDrainSpeed <= 0.0f)
        {
            staminaMeterDrainSpeed = 10.0f;
        }
        if (staminaMeterRegenSpeed <= 0.0f)
        {
            staminaMeterRegenSpeed = 3.0f;
        }

        //Hunger
        if (hungerMeterDrainSpeed <= 0.0f)
        {
            hungerMeterDrainSpeed = 0.25f;
        }
        if (hungerMeterRegenSpeed <= 0.0f)
        {
            hungerMeterRegenSpeed = 10.0f;
        }

        //Thirst
        if (thirstMeterDrainSpeed <= 0.0f)
        {
            thirstMeterDrainSpeed = 0.125f;
        }
        if (thirstMeterRegenSpeed <= 0.0f)
        {
            thirstMeterRegenSpeed = 10.0f;
        }
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
                bCanMove = true;
            }
            else
            {
                waypointManager.waypointManagerPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                bCanMove = false;
            }
        }
        //Movement
        Move();
        Rotate();
        Jump();
        
        SpawnDrone();

        HealthMeter();
        healthPercentage = healthMeter / maxHealthMeter;

        StaminaMeter();
        staminaPercentage = staminaMeter / maxStaminaMeter;

        HungerMeter();
        hungerPercentage = hungerMeter / maxHungerMeter;

        ThirstMeter();
        thirstPercentage = thirstMeter / maxThirstMeter;
    }

    public void ChangeThirstAmount(float m_amount)
    {
        thirstMeter += m_amount;
    }

    void ThirstMeter()
    {
        thirstMeter -= Time.deltaTime * thirstMeterDrainSpeed;
        thirstBar.fillAmount = thirstPercentage;

        if (bCanRegenThirst)
        {
            thirstMeter += Time.deltaTime * thirstMeterRegenSpeed;
        }
        //Makes sure thirst does not go negative.
        if (thirstMeter <= 0.0f)
        {
            bIsDehydrated = true;
            healthMeter -= Time.deltaTime * healthMeterDrainSpeed;
            thirstMeter = 0.0f;

        }
        //Otherwise, keep normal
        else
        {
            bIsDehydrated = false;
        }

        //Makes sure thirst does not regenerate over the max limit
        if (thirstMeter >= maxThirstMeter)
        {
            thirstMeter = maxThirstMeter;
        }
    }

    //Function for changing hunger level, negatives allowed for deducting hunger
    public void ChangeHungerLevel(float m_amount)
    {
        hungerMeter += m_amount;
    }

    void HungerMeter()
    {
        hungerMeter -= Time.deltaTime * hungerMeterDrainSpeed;
        hungerBar.fillAmount = hungerPercentage;

        if (bCanRegenHunger)
        {
            hungerMeter += Time.deltaTime * hungerMeterRegenSpeed;
        }
        //Makes sure hunger does not go negative.
        if (hungerMeter <= 0.0f)
        {
            bIsStarving = true;
            healthMeter -= Time.deltaTime * healthMeterDrainSpeed;
            hungerMeter = 0.0f;

        }
        //Otherwise, keep normal
        else
        {
            bIsStarving = false;
        }

        //Makes sure hunger does not regenerate over the max limit
        if (hungerMeter >= maxHungerMeter)
        {
            hungerMeter = maxHungerMeter;
        }
    }
    //Function for changing stamina level, negatives allowed for deducting stamina
    public void ChangeStaminaLevel(float m_amount)
    {
        staminaMeter += m_amount;
    }


    void StaminaMeter()
    {
        staminaBar.fillAmount = staminaPercentage;

        //Stamina drains if starving or dehydrated.
        if(bIsStarving || bIsDehydrated)
        {
            staminaMeter -= Time.deltaTime * staminaMeterDrainSpeed;
        }

        if (bCanRegenStamina)
        {
            staminaMeter += Time.deltaTime * staminaMeterRegenSpeed;
        }
        //Makes sure stamina does not go negative.
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

        //Makes sure stamina does not regenerate over the max limit
        if (staminaMeter >= maxStaminaMeter)
        {
            staminaMeter = maxStaminaMeter;
        }
    }

    //Function for changing health level, negatives allowed for deducting health
    public void ChangeHealthLevel(float m_amount)
    {
        healthMeter += m_amount;
    }

    void HealthMeter()
    {
        healthBar.fillAmount = healthPercentage;

        if (bCanRegenHealth)
        {
            healthMeter += Time.deltaTime * healthMeterRegenSpeed;
        }

        //Makes sure health does not go negative.
        if (healthMeter <= 0.0f)
        {
            bIsDead = true;
            bCanSpawnDrone = false;
            if (bHasDeployedDrone)
            {
                Destroy(spawnedDrone);
            }
            healthMeter = 0.0f;

        }
        //Otherwise, keep normal
        else
        {
            bIsDead = false;
            bCanSpawnDrone = true;
        }

        if (bIsDead)
        {
            bCanMove = false;
            bCanRegenHealth = false;
        }
        else
        {
            bCanRegenHealth = true;
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
        if (motor.IsGrounded() && bCanSpawnDrone) {
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

            //Sprinting
            if (Input.GetButton("Run") && (xMove != 0.0f || zMove != 0.0f) && !bHasDeployedDrone)
            {
                //Disables stamina regen while running
                bCanRegenStamina = false;
                //Check for exhausted
                if (!bIsExhausted)
                {   
                    //drain stamina when player attempts to run
                    staminaMeter -= Time.deltaTime * staminaMeterDrainSpeed;
                    //Stop speed boost when stamina meter is below 10
                    if(!(staminaMeter <= 10.0f))
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
                bCanRegenStamina = true;
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