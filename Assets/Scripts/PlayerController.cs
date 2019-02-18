using UnityEngine;

//Player controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings:")]
    [SerializeField] private bool bCanMove = true;
    [SerializeField] private bool bCanSprint = true;
    [SerializeField] private bool bCanJump = true;

    [Space(15)]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float sensitivity = 3.0f;

    [Space(15)]
    [SerializeField] private float thrust = 1000.0f;
    [SerializeField] private float gravityStrength = 10.0f;
    [SerializeField] private float jumpSpeed = 8f;

    [Space(15)]
    [SerializeField] private float maxSprintMeter = 100.0f;
    [SerializeField] private float sprintMeter;
    [SerializeField] private float sprintMeterDrainMultiplier = 20.0f;
    [SerializeField] private float sprintMeterRecoveryMultiplier = 20.0f;

    [Space(15)]
    [SerializeField] private float maxJumpMeter = 100.0f;
    [SerializeField] private float jumpMeter;
    [SerializeField] private float jumpMeterDrainMultiplier = 50.0f;
    [SerializeField] private float jumpMeterRecoveryMultiplier = 50.0f;
    private bool bIsSprinting = false;
    private bool bIsJumping = false;

    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        jumpMeter = maxJumpMeter;
        sprintMeter = maxSprintMeter;
    }

    void Update()
    {
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
        if (bIsSprinting)
        {
            sprintMeter -= Time.deltaTime * sprintMeterDrainMultiplier;
        }
        else
        {
            sprintMeter += Time.deltaTime * sprintMeterRecoveryMultiplier;
        }

        if(sprintMeter <= 0.0f)
        {
            bCanSprint = false;
            sprintMeter = 0.0f;
        }
        else
        {
            bCanSprint = true;
        }

        if(sprintMeter >= maxSprintMeter)
        {
            sprintMeter = maxSprintMeter;
        }
    }
    
    void JumpMeter()
    {
        if(bIsJumping)
        {
            jumpMeter -= Time.deltaTime * jumpMeterDrainMultiplier;
        }
        else
        {
            jumpMeter += Time.deltaTime * jumpMeterRecoveryMultiplier;
        }

        if (jumpMeter <= 0.0f)
        {
            bCanJump = false;
            jumpMeter = 0.0f;
        }
        else
        {
            bCanJump = true;
        }

        if (jumpMeter >= maxJumpMeter)
        {
            jumpMeter = maxJumpMeter;
        }
    }

    void Jump()
    {
        float localThrust;
        if (Input.GetButton("Jump"))
        {
            bIsJumping = true;
            localThrust = thrust;
        }
        else
        {
            bIsJumping = false;
            localThrust = 0.0f;
        }

        if (!bCanJump)
        {
            localThrust = 0.0f;
        }
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

        if (Input.GetButton("Sprint"))
        {
            bIsSprinting = true;
        }
        else
        {
            bIsSprinting = false;
        }

        float calculationSpeed;
        //Final movement velocity
        if (bIsJumping && bCanJump && bIsSprinting && bCanSprint)
        {
            calculationSpeed = (jumpSpeed / 2) + (sprintSpeed / 2);
        }
        else if (bIsJumping && bCanJump)
        {
            calculationSpeed = jumpSpeed;
        }
        else if(bIsSprinting && bCanSprint)
        {
            calculationSpeed = sprintSpeed;
        }
        else
        {
            calculationSpeed = speed;
        }
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
        //Apply rotation
        motor.Rotate(rotation);
        motor.RotateCamera(cameraRotationX);
    }
}
