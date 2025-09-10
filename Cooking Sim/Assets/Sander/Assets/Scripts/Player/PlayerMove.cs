using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float walkspeed = 10f;

    Vector2 moveInput;
    Rigidbody myRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
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
