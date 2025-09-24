using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float walkspeed;

    public Vector2 moveInput;
    public Rigidbody myRigidbody;

    void Update()
    {
        Run();
    }

    void Run()
    { 
        Vector3 playerVelocity = new Vector3(moveInput.x * walkspeed, myRigidbody.linearVelocity.y, moveInput.y * walkspeed);
        myRigidbody.linearVelocity = transform.TransformDirection(playerVelocity);
    }

    void OnMove (InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
}
