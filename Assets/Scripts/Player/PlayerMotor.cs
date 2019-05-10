// Copyright (c) 2019 JensenJ
// NAME: PlayerMotor
// PURPOSE: Apply physics to player character in order to move.

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    //Local variables
    private Rigidbody rb;
    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private float jumpForce = 2f;
    private Camera cam;

    //Enterable variables
    [SerializeField] [Range(0, 2)] private float raycastDistance = 1.1f;
    [SerializeField] [Range(0, 90)]private float cameraRotationLimit = 85.0f;

    // Setup
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(1).GetComponent<Camera>();
    }

    // Update movement
    void FixedUpdate()
    {
        PerformMove();
        PerformRotate();
    }

    //Function for setting rigidbody constraints
    public void Freeze(RigidbodyConstraints m_constraints)
    {
        rb.constraints = m_constraints;
    }

    //Sets movement velocity from player controller
    public void Move(Vector3 m_velocity)
    {
        velocity = m_velocity;
    }
    
    //Sets rotation from player controller
    public void Rotate(Vector3 m_rotation)
    {
        rotation = m_rotation;
    }

    // jump function called from player controller
    public void Jump(float m_jumpForce, bool m_isSwimming)
    {
        //Swimming
        jumpForce = m_jumpForce;
        if (m_isSwimming)
        {
            rb.AddForce(0.0f, jumpForce, 0.0f, ForceMode.Impulse);
        }
        //Jumping
        else
        {
            //Prevents double jumps
            if (IsGrounded())
            {
                rb.AddForce(0.0f, jumpForce, 0.0f, ForceMode.Impulse);
            }
        }
    }

    //Checks whether player is on the ground
    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, raycastDistance);
    }

    //Sets cam rotation from player controller
    public void RotateCamera(float m_cameraRotation)
    {
        cameraRotationX = m_cameraRotation;
    }

    //Perform movement
    void PerformMove()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
        }
    }

    //Perform rotations on player and camera
    void PerformRotate()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        //Check for camera before doing rotations
        if(cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
        }
        else
        {
            //Error log
            Debug.LogError("PerformRotation::No camera found");
        }
    }
}
