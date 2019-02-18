using UnityEngine;

//Player controller class is mainly for player input, movement is handled by motor.
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sensitivity = 3.0f;
    [SerializeField] private float thrust = 1000.0f;
    //[SerializeField] private float gravityStrength = 10.0f;

    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        //Get axis for x and z axis.
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        //get actual relative movement from transform.right/forward
        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        //Final movement velocity
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        //Apply movement on motor
        motor.Move(velocity);

        //Get axis for rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 rotation = new Vector3(0.0f, yRot, 0.0f) * sensitivity;
        float cameraRotationX = xRot * sensitivity;
        //Apply rotation
        motor.Rotate(rotation);
        motor.RotateCamera(cameraRotationX);

        if (Input.GetButton("Jump"))
        {
            motor.Thrust(thrust);
        }
        else
        {
            motor.Thrust(0.0f);
        }
    }
}
