using UnityEngine;

//Player controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(DroneMotor))]
public class DroneController : MonoBehaviour
{
    //bools for enabling/disabling certain character abilities.
    [Header("Movement Settings:")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private bool bCanBoost = true;
    [SerializeField] private bool bCanThrust = true;

    //basic movement 
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

    //Energy Meter
    [Space(15)]
    [SerializeField] private float maxEnergyMeter = 100.0f;
    [SerializeField] private float energyMeter;
    [SerializeField] private float energyMeterDrainSpeed = 20.0f;
    [SerializeField] private float energyMeterRecoverySpeed = 20.0f;
    [SerializeField] private float energyPercentage = 100.0f;

    [Space(15)]
    [SerializeField] private float energyDrainJumpMultiplier = 2.5f;
    [SerializeField] private float energyDrainBoostMultiplier = 2.0f;
    [SerializeField] private float speedWithDrainedEnergy = 1.0f;

    [Space(15)]
    [SerializeField] private float initialSpeed = 5.0f;
    [SerializeField] private float initialBoostSpeed = 10.0f;
    [SerializeField] private float initialThrustMultiplier = 8.0f;

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

        EnergyMeter();
        energyPercentage = (energyMeter / maxEnergyMeter) * 100;
    }

    public void ChangeEnergyLevel(float amount)
    {
        energyMeter += amount;
    }

    void EnergyMeter()
    {
        //drain stamina if player is sprinting and restore if not
        if (bIsBoosting)
        {
            energyMeter -= Time.deltaTime * energyMeterDrainSpeed * energyDrainBoostMultiplier;
        }

        if (bIsThrusting)
        {
            energyMeter -= Time.deltaTime * energyMeterDrainSpeed * energyDrainJumpMultiplier;
        }

        //Makes sure stamina does not go negative and also disables sprinting if out of stamina.
        if(energyMeter <= 0.0f)
        {
            speed = speedWithDrainedEnergy;
            boostSpeed = speedWithDrainedEnergy;
            thrustMultiplier = speedWithDrainedEnergy;

            energyMeter = 0.0f;
        }
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
        //Check whether player is attempting to jump, if so set local thrust to defined thrust
        if (Input.GetButton("MoveUp"))
        {
            bIsThrusting = true;
            localThrust = thrust * thrustMultiplier;
        }
        else if (Input.GetButton("MoveDown"))
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

        //Apply jump with given force
        motor.Jump(localThrust);
        
    }

    void Move()
    {  
        //Get axis for x and z axis.
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        //get actual relative movement from transform.right/forward
        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        //Check whether player is attempting to sprint
        if (Input.GetButton("MoveFast"))
        {
            bIsBoosting = true;
        }
        else
        {
            bIsBoosting = false;
        }

        float calculationSpeed;
        //Set speed based on various factors.
        //Is jumping and is sprinting
        if (bIsThrusting && bCanThrust && bIsBoosting && bCanBoost)
        {
            calculationSpeed = (thrustMultiplier / 2) + (boostSpeed / 2);
        }
        //Jumping
        else if (bIsThrusting && bCanThrust)
        {
            calculationSpeed = thrustMultiplier;
        }
        //Sprinting
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