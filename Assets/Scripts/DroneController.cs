// Copyright (c) 2019 JensenJ
// NAME: DroneController
// PURPOSE: Controls player input, specifically for the drone.

using UnityEngine;
using UnityEngine.UI;

//Drone controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(DroneMotor))]
public class DroneController : MonoBehaviour
{
    //bools for enabling/disabling certain drone abilities.
    [Header("Movement Settings:")]
    [SerializeField] public bool bCanMove = true;
    [SerializeField] private bool bCanBoost = true;
    [SerializeField] private bool bCanThrust = true;

    //basic movement settings
    private float speed = 5.0f;
    private float boostSpeed = 10.0f;
    private float sensitivity = 3.0f;

    //Thrusting related
    [Space(15)]
    [SerializeField] [Range(0, 500)] private float thrust = 100.0f;
    private float thrustMultiplier = 8f;
    [SerializeField] [Range(0, 2)] private float minimumDistFromGround = 0.8f;
    [SerializeField] [Range(0, 1000)]private float emergencyThrusterForce = 500.0f;

    //Initial setup variables
    [Space(15)]
    [SerializeField] [Range(0, 15)] private float initialSpeed = 5.0f;
    [SerializeField] [Range(0, 30)] private float initialBoostSpeed = 10.0f;
    [SerializeField] [Range(0, 10)] private float initialThrustMultiplier = 8.0f;
    //Energy Meter
    
    [Header("Attributes:")]
    [SerializeField] [Range(0, 20000)] private float maxEnergyMeter = 100.0f;
    [SerializeField] [Range(0, 20)] private float energyMeterDrainSpeed = 20.0f;
    [SerializeField] [Range(0, 1)] private float energyPercentage = 1.0f;
    private float energyMeter;
    private Image energyBar = null;

    //Energy Depleted Settings
    [Space(15)]
    [SerializeField] [Range(0, 5)] private float energyDrainJumpMultiplier = 2.5f;
    [SerializeField] [Range(0, 5)] private float energyDrainBoostMultiplier = 2.0f;
    [SerializeField] [Range(0, 5)] private float speedWithDrainedEnergy = 1.0f;

    //Variables for checking whether player is currently doing an action. 
    private bool bIsBoosting = false;
    private bool bIsThrusting = false;


    private DroneMotor motor;

    void Start()
    {
        //Setting defaults.
        motor = GetComponent<DroneMotor>();
        energyMeter = maxEnergyMeter;
        speed = initialSpeed;
        boostSpeed = initialBoostSpeed;
        thrustMultiplier = initialThrustMultiplier;
        motor.Setup(minimumDistFromGround, emergencyThrusterForce);

        Transform panelTransform = transform.root.GetChild(1).GetChild(0).GetChild(1);
        energyBar = panelTransform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        //movement calculations.
        Move();
        Rotate();
        Thrust();
        //Energy related stuff
        EnergyMeter();
        energyPercentage = energyMeter / maxEnergyMeter;
    }

    //Function for changing energy level, negatives allowed for deducting energy
    public void ChangeEnergyLevel(float amount)
    {
        energyMeter += amount;
    }

    void EnergyMeter()
    {

        energyBar.fillAmount = energyPercentage;
        //Makes sure energy does not go negative and also slows drone if out
        if(energyMeter <= 0.0f)
        {
            speed = speedWithDrainedEnergy;
            boostSpeed = speedWithDrainedEnergy;
            thrustMultiplier = speedWithDrainedEnergy;

            energyMeter = 0.0f;
        }
        //Otherwise, keep normal speed
        else
        {
            speed = initialSpeed;
            boostSpeed = initialBoostSpeed;
            thrustMultiplier = initialThrustMultiplier;
        }

        //Makes sure sprint meter does not regenerate over the max limit
        if(energyMeter >= maxEnergyMeter)
        {
            energyMeter = maxEnergyMeter;
        }
    }

    void Thrust()
    {
        float localThrust = 0.0f;
        //Check whether player is attempting to thrust, if so set local thrust to defined thrust
        if (bCanMove)
        {
            if (Input.GetButton("Jump"))
            {
                bIsThrusting = true;
                localThrust = thrust * thrustMultiplier;
                energyMeter -= Time.deltaTime * energyMeterDrainSpeed * energyDrainJumpMultiplier;
            }
            else if (Input.GetButton("Crouch"))
            {
                bIsThrusting = true;
                localThrust = -thrust * thrustMultiplier;
            }
            //Other wise set thrust to 0
            else
            {
                bIsThrusting = false;
                localThrust = 0.0f;
            }
        }
        //Apply thrust with given force
        motor.Thrust(localThrust);
        
    }

    void Move()
    {
        Vector3 velocity = Vector3.zero;

        if (bCanMove)
        {
            //Get axis for x and z axis.
            float xMove = Input.GetAxisRaw("Horizontal");
            float zMove = Input.GetAxisRaw("Vertical");

            //get actual relative movement from transform.right/forward
            Vector3 moveHorizontal = transform.right * xMove;
            Vector3 moveVertical = transform.forward * zMove;

            //Check whether player is attempting to boost
            if (Input.GetButton("Run") && xMove != 0.0f && zMove != 0.0f)
            {
                bIsBoosting = true;
                //drain stamina if player is boosting and restore if not
                energyMeter -= Time.deltaTime * energyMeterDrainSpeed * energyDrainBoostMultiplier;
            }
            else
            {
                bIsBoosting = false;
            }

            float calculationSpeed;
            //Set speed based on various factors.
            //Is thrusting and is boosting
            if (bIsThrusting && bCanThrust && bIsBoosting && bCanBoost)
            {
                calculationSpeed = (thrustMultiplier / 2) + (boostSpeed / 2);
            }
            //thrusting
            else if (bIsThrusting && bCanThrust)
            {
                calculationSpeed = thrustMultiplier;
            }
            //boosting
            else if (bIsBoosting && bCanBoost)
            {
                calculationSpeed = boostSpeed;
            }
            //Normal
            else
            {
                calculationSpeed = speed;
            }
            //Final velocity
            velocity = (moveHorizontal + moveVertical).normalized * calculationSpeed;

        }
        //Apply movement on motor
        motor.Move(velocity);
    }

    void Rotate()
    {
        Vector3 rotation = Vector3.zero;
        float cameraRotationX = 0.0f;

        //Get axis for rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        if (bCanMove)
        {
            rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
            cameraRotationX = xRot * sensitivity;
        }
        //Apply rotations

        motor.Rotate(rotation);
        motor.RotateCamera(cameraRotationX);
    }
}