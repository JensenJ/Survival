// Copyright (c) 2019 JensenJ
// NAME: PlayerController
// PURPOSE: Controls for player character (getting input)

using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private bool bCanMove = true;

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float jumpForce = 2.0f;
    PlayerMotor motor;

    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bCanMove)
        {
            Move();
            Rotate();
            Jump();
        }
    }

    void Move()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);
    }

    void Rotate()
    {
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
        float CameraRotationX = xRot * sensitivity;
        motor.Rotate(rotation);
        motor.RotateCamera(CameraRotationX);
    }

    void Jump()
    {
        float power;
        if (Input.GetButtonDown("Jump"))
        {
            power = jumpForce;
        }
        else
        {
            power = 0;
        }
        motor.Jump(power);
    }
}