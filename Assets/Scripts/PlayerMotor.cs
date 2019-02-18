using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private float thrust = 0.0f;
    private float gravityStrength = 9.81f;

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
        PerformMovement();
        PerformRotation();
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

    public void RotateCamera(float m_cameraRotation)
    {
        cameraRotationX = m_cameraRotation;
    }

    public void Thrust(float m_thrust)
    {
        thrust = m_thrust;
    }

    //Performs movement
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
        }

        print(thrust);
        if(thrust != 0)
        {
            //Upwards
            rb.AddForce(new Vector3(0.0f, thrust, 0.0f) * Time.fixedDeltaTime);
        }
        else
        {
            //Gravity
            rb.AddForce(new Vector3(0.0f, (-gravityStrength * 200), 0.0f) * Time.fixedDeltaTime);
        }
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, layerMask))
        {
            //Negative Gravity (Up)
            rb.AddForce(transform.up * (gravityStrength / (hit.distance / 5f)));
        }
    }

    //Performs rotation
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
        }
    }
}