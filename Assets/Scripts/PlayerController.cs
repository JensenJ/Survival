using UnityEngine;

//Player controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    //bools for enabling/disabling certain character abilities.
    [Header("Movement Settings:")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private bool bCanSprint = true;
    [SerializeField] private bool bCanJump = true;

    //basic movement 
    [Space(15)]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float sensitivity = 3.0f;

    //Jumping related
    [Space(15)]
    [SerializeField] private float thrust = 1000.0f;
    [SerializeField] private float gravityStrength = 10.0f;
    [SerializeField] private float jumpSpeed = 8f;

    //Sprint meter/Stamina
    [Space(15)]
    [SerializeField] private float maxSprintMeter = 100.0f;
    [SerializeField] private float sprintMeter;
    [SerializeField] private float sprintMeterDrainMultiplier = 20.0f;
    [SerializeField] private float sprintMeterRecoveryMultiplier = 20.0f;

    //Jump meter / fuel
    [Space(15)]
    [SerializeField] private float maxJumpMeter = 100.0f;
    [SerializeField] private float jumpMeter;
    [SerializeField] private float jumpMeterDrainMultiplier = 50.0f;
    [SerializeField] private float jumpMeterRecoveryMultiplier = 50.0f;

    //Variables for checking whether player is currently doing an action. 
    private bool bIsSprinting = false;
    private bool bIsJumping = false;

    private PlayerMotor motor;

    void Start()
    {
        //Setting defaults.
        motor = GetComponent<PlayerMotor>();
        jumpMeter = maxJumpMeter;
        sprintMeter = maxSprintMeter;
    }

    void Update()
    {
        //Checking whether character can actually move before allowing movement calculations.
        if (bCanMove)
        {
            Move();
            Rotate();
            Jump();
        }

        SprintMeter();
        JumpMeter();
    }

    void SprintMeter()
    {
        //drain stamina if player is sprinting and restore if not
        if (bIsSprinting)
        {
            sprintMeter -= Time.deltaTime * sprintMeterDrainMultiplier;
        }
        else
        {
            sprintMeter += Time.deltaTime * sprintMeterRecoveryMultiplier;
        }

        //Makes sure stamina does not go negative and also disables sprinting if out of stamina.
        if(sprintMeter <= 0.0f)
        {
            bCanSprint = false;
            sprintMeter = 0.0f;
        }
        else
        {
            bCanSprint = true;
        }

        //Makes sure sprint meter does not regenerate over the max limit
        if(sprintMeter >= maxSprintMeter)
        {
            sprintMeter = maxSprintMeter;
        }
    }
    
    void JumpMeter()
    {
        //drain fuel if player is jumping and restore if not
        if(bIsJumping)
        {
            jumpMeter -= Time.deltaTime * jumpMeterDrainMultiplier;
        }
        else
        {
            jumpMeter += Time.deltaTime * jumpMeterRecoveryMultiplier;
        }

        //Makes sure fuel does not go negative and also disables jumping if out of fuel
        if (jumpMeter <= 0.0f)
        {
            bCanJump = false;
            jumpMeter = 0.0f;
        }
        else
        {
            bCanJump = true;
        }

        //Makes sure jump meter does not regenerate over the max limit.
        if (jumpMeter >= maxJumpMeter)
        {
            jumpMeter = maxJumpMeter;
        }
    }

    void Jump()
    {
        float localThrust;
        //Check whether player is attempting to jump, if so set local thrust to defined thrust
        if (Input.GetButton("Jump"))
        {
            bIsJumping = true;
            localThrust = thrust;
        }
        //Other wise set thrust to 0
        else
        {
            bIsJumping = false;
            localThrust = 0.0f;
        }

        //Check whether player can actually jump
        if (!bCanJump)
        {
            localThrust = 0.0f;
        }
        //Apply jump with given force
        motor.Jump(localThrust, gravityStrength);
        
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
        if (Input.GetButton("Sprint"))
        {
            bIsSprinting = true;
        }
        else
        {
            bIsSprinting = false;
        }

        float calculationSpeed;
        //Set speed based on various factors.
        //Is jumping and is sprinting
        if (bIsJumping && bCanJump && bIsSprinting && bCanSprint)
        {
            calculationSpeed = (jumpSpeed / 2) + (sprintSpeed / 2);
        }
        //Jumping
        else if (bIsJumping && bCanJump)
        {
            calculationSpeed = jumpSpeed;
        }
        //Sprinting
        else if(bIsSprinting && bCanSprint)
        {
            calculationSpeed = sprintSpeed;
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