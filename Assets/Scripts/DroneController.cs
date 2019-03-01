// Copyright (c) 2019 JensenJ
// NAME: DroneController
// PURPOSE: Controls player input, specifically for the drone.

using UnityEngine;

//Drone controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(Compass))]
[RequireComponent(typeof(DroneMotor))]
public class DroneController : MonoBehaviour
{
    //bools for enabling/disabling certain drone abilities.
    [Header("Movement Settings:")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private bool bCanBoost = true;
    [SerializeField] private bool bCanThrust = true;

    //basic movement settings
    [Space(15)]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float boostSpeed = 10.0f;
    [SerializeField] private float sensitivity = 3.0f;

    //Thrusting related
    [Space(15)]
    [SerializeField] private float thrust = 100.0f;
    [SerializeField] private float thrustMultiplier = 8f;
    [SerializeField] private float minimumDistFromGround = 0.8f;
    [SerializeField] private float emergencyThrusterForce = 500.0f;


    //Initial setup variables
    [Space(15)]
    [SerializeField] private float initialSpeed = 5.0f;
    [SerializeField] private float initialBoostSpeed = 10.0f;
    [SerializeField] private float initialThrustMultiplier = 8.0f;
    //Energy Meter
    
    [Header("Attributes:")]
    [SerializeField] private float maxEnergyMeter = 100.0f;
    [SerializeField] private float energyMeter;
    [SerializeField] private float energyMeterDrainSpeed = 20.0f;
    [SerializeField] private float energyPercentage = 100.0f;

    //Energy Depleted Settings
    [Space(15)]
    [SerializeField] private float energyDrainJumpMultiplier = 2.5f;
    [SerializeField] private float energyDrainBoostMultiplier = 2.0f;
    [SerializeField] private float speedWithDrainedEnergy = 1.0f;

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
    }

    void Update()
    {
        //Checking whether character can actually move before allowing movement calculations.
        if (bCanMove)
        {
            Move();
            Rotate();
            Thrust();
        }

        //Energy related stuff
        EnergyMeter();
        energyPercentage = (energyMeter / maxEnergyMeter) * 100;
    }

    //Function for changing energy level, negatives allowed for deducting energy
    public void ChangeEnergyLevel(float amount)
    {
        energyMeter += amount;
    }

    void EnergyMeter()
    { 
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
        float localThrust;
        //Check whether player is attempting to thrust, if so set local thrust to defined thrust
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

        //Apply thrust with given force
        motor.Thrust(localThrust);
        
    }

    void Move()
    {  
        //Get axis for x and z axis.
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        //get actual relative movement from transform.right/forward
        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        //Check whether player is attempting to boost
        if (Input.GetButton("Run"))
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
        else if(bIsBoosting && bCanBoost)
        {
            calculationSpeed = boostSpeed;
        }
        //Normal
        else
        {
            calculationSpeed = speed;
        }
        //Final velocity
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * calculationSpeed;

        //Apply movement on motor
        motor.Move(velocity);
    }

    void Rotate()
    {
        //Get axis for rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
        float cameraRotationX = xRot * sensitivity;
        //Apply rotations
        motor.Rotate(rotation);
        motor.RotateCamera(cameraRotationX);
    }
}