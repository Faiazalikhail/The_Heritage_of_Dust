using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private StateMachine stateMachine;

    // States
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;

    public float horizontalInput;

    [Header("Jump")]
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool isGrounded;

    public PlayerJumpState jumpState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();

        // Initialize states
        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);

        jumpState = new PlayerJumpState(this, stateMachine);
    }

    void Start()
    {
        stateMachine.Initialize(idleState);
    }

    void Update()
    {

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
            );

        horizontalInput = Input.GetAxisRaw("Horizontal");

        stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            stateMachine.ChangeState(jumpState);
        }
    }

    void FixedUpdate()
    {
        // Movement handled in states
    }

    public void SetVelocity(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    public void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}