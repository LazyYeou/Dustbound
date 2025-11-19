using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public Rigidbody2D rb;

    private Vector2 _lastDirection;
    private Vector2 _moveDirection;
    void Start()
    {
        _lastDirection = new Vector2(1, 0);
    }

    void Update()
    {
        ProcessInput();

    }

    void FixedUpdate()
    {
        Move();
    }

    void ProcessInput()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        _moveDirection = new Vector2(inputX, inputY).normalized;

        if (inputX != 0 || inputY != 0)
        {
            _lastDirection = _moveDirection;
        }
    }

    void Move()
    {
        rb.linearVelocity = _moveDirection * movementSpeed;
    }

    void Logging(float inputX, float inputY, Vector2 moveDir)
    {
        Debug.Log($"x: {inputX} y: {inputY} dir{moveDir}");
    }
}
