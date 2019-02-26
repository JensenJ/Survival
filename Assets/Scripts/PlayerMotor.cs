// Copyright (c) 2019 JensenJ
// NAME: PlayerMotor
// PURPOSE: Apply physics to player character in order to move.

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{

    private Rigidbody rb;
    private CapsuleCollider cc;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private float jumpForce = 2f;

    [SerializeField] private float raycastDistance = 1.1f;
    [SerializeField] private float cameraRotationLimit = 85.0f;
    [SerializeField] private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = transform.GetChild(1).GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        PerformMove();
        PerformRotate();
    }

    public void Move(Vector3 m_velocity)
    {
        velocity = m_velocity;
    }

    public void Rotate(Vector3 m_rotation)
    {
        rotation = m_rotation;
    }

    public void Jump(float m_jumpForce)
    {
        jumpForce = m_jumpForce;
        if (IsGrounded())
        {
            rb.AddForce(0.0f, jumpForce, 0.0f, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, raycastDistance);
    }

    public void RotateCamera(float m_cameraRotation)
    {
        cameraRotationX = m_cameraRotation;
    }

    void PerformMove()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
        }
    }

    void PerformRotate()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if(cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0.0f, 0.0f);
        }
        else
        {
            Debug.LogError("PerformRotation::No camera found");
        }
    }
}
