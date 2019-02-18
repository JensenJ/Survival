using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    //Variables to be taken from playerController;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private float thrust = 0.0f;
    private float gravityStrength = 9.81f;

    //Camera settings and reference
    [SerializeField] private float cameraRotationLimit = 85.0f;
    [SerializeField] private Camera cam;
    private Rigidbody rb;


    void Start()
    {
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
    public void Jump(float m_thrust, float m_gravity)
    {
        thrust = m_thrust;
        gravityStrength = m_gravity;
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
        //If thrusting
        if (thrust != 0)
        {
            //add upwards force
            rb.AddForce(new Vector3(0.0f, thrust, 0.0f) * Time.fixedDeltaTime);
        }
        else
        {
            //add downwards force
            rb.AddForce(new Vector3(0.0f, -gravityStrength * 300, 0.0f) * Time.fixedDeltaTime);
        }
        //Layer mask for raytrace
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        //Stops collision with ground when moving vertically in thrust or falling.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
        {
            //Add upwards force for floating effect on player
            rb.AddForce(transform.up * (gravityStrength / (hit.distance / 9f)));
        }
    }
}