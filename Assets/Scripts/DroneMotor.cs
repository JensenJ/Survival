using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneMotor : MonoBehaviour
{
    //Variables to be taken from playerController;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private float thrust = 0.0f;

    //Camera settings and reference
    [SerializeField] private float cameraRotationLimit = 85.0f;
    [SerializeField] private Camera cam;
    private DroneController dc;
    private Rigidbody rb;

    private float minimumDistFromGround = 0.8f;
    private float emergencyThrusterForce = 500.0f;


    void Start()
    {
        dc = GetComponent<DroneController>();
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(1).GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        //Perform calculations every tick
        PerformMovement();
        PerformRotation();
        PerformJump();
    }

    public void Setup(float m_minimumDistFromGround, float m_emergencyThrusterForce)
    {
        minimumDistFromGround = m_minimumDistFromGround;
        emergencyThrusterForce = m_emergencyThrusterForce;
    }

    //Gets move velocity from player controller input.
    public void Move(Vector3 m_velocity)
    {
        velocity = m_velocity;
    }

    //Gets rotation from player controller input.
    public void Rotate(Vector3 m_rotation)
    {
        rotation = m_rotation;
    }

    //Gets camera rotation from player controller input.
    public void RotateCamera(float m_cameraRotation)
    {
        cameraRotationX = m_cameraRotation;
    }

    //Gets jump thrust and gravity from player controller input.
    public void Jump(float m_thrust)
    {
        thrust = m_thrust;
    }

    //Performs movement
    void PerformMovement()
    {
        //Checks whether player is moving
        if (velocity != Vector3.zero)
        {
            //If so, move to new position
            rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
        }
    }

    //Performs rotation
    void PerformRotation()
    {
        //Rotates to new rotation
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        //Makes sure camera is actually there and logs an error if not
        if (cam != null)
        {
            //Camera rotation
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
        }
        else
        {
            Debug.LogError("PerformRotation::No camera found");
        }
    }

    //Performs jump calculation
    void PerformJump()
    { 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity))
        {
            if (hit.distance > minimumDistFromGround)
            {
                //If thrusting
                if (thrust != 0)
                {
                    //add vertical force
                    rb.AddForce(new Vector3(0.0f, thrust, 0.0f) * Time.fixedDeltaTime);
                }
            }
            else
            {
                rb.AddForce(new Vector3(0.0f, emergencyThrusterForce, 0.0f) * Time.fixedDeltaTime);
            }

            //this means 30 minutes of use real-time before recharge assuming y-axis remains constant at 1
            //and that boost or any other drainage of energy.
            dc.ChangeEnergyLevel(-hit.distance / 100.0f);
        }
    }
}