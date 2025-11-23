using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public Rigidbody2D rb;
    public Vector2 lastDirection;

    private Vector2 _moveDirection;
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        lastDirection = new Vector2(1, 0);
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            lastDirection = _moveDirection;
        }
    }

    void Move()
    {
        rb.linearVelocity = _moveDirection * movementSpeed;
        if (_moveDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipY = false;
        }
    }

    void Logging(float inputX, float inputY, Vector2 moveDir)
    {
        Debug.Log($"x: {inputX} y: {inputY} dir{moveDir}");
    }
}
