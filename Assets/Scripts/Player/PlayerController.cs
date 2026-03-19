using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // =====================================================
    // 🔹 CORE COMPONENTS
    // =====================================================
    private Rigidbody2D rb;
    private StateMachine stateMachine;

    // =====================================================
    // 🔹 MOVEMENT
    // =====================================================
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float horizontalInput;

    [Header("Run")]
    public float runMultiplier = 1.5f;

    

    // =====================================================
    // 🔹 JUMP SYSTEM
    // =====================================================
    [Header("Jump")]
    public float jumpForce = 10f;
    public int maxJumps = 2;

    private int jumpCount = 0;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool isGrounded;

    // =====================================================
    // 🔹 WALL SYSTEM
    // =====================================================
    [Header("Wall Check")]
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckDistance = 0.2f;

    public bool isTouchingWall;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 2f;

    // =====================================================
    // 🔹 SHOOTING SYSTEM
    // =====================================================
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Fire Rate")]
    public float fireRate = 0.5f;

    private float fireTimer = 0f;

    // =====================================================
    // 🔹 HEALTH SYSTEM
    // =====================================================
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    // =====================================================
    // 🔹 STATES
    // =====================================================
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerJumpState jumpState;
    public PlayerAttackState attackState;
    public PlayerHurtState hurtState;
    public PlayerDeadState deadState;

    // =====================================================
    // 🔹 Animator
    // =====================================================

    private Animator anim;

    // =====================================================
    // 🔹 UNITY METHODS
    // =====================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new StateMachine();

        // Initialize states
        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        attackState = new PlayerAttackState(this, stateMachine);
        hurtState = new PlayerHurtState(this, stateMachine);
        deadState = new PlayerDeadState(this, stateMachine);

        // Initialize health
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();
    }

    void Start()
    {
        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        // -------------------------------------------------
        // INPUT
        // -------------------------------------------------
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // -------------------------------------------------
        // GROUND CHECK
        // -------------------------------------------------
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0; // reset jumps on landing
        }

        // -------------------------------------------------
        // WALL CHECK
        // -------------------------------------------------
        bool leftWall = Physics2D.Raycast(
            wallCheckLeft.position,
            Vector2.left,
            wallCheckDistance,
            groundLayer
        );

        bool rightWall = Physics2D.Raycast(
            wallCheckRight.position,
            Vector2.right,
            wallCheckDistance,
            groundLayer
        );

        isTouchingWall = leftWall || rightWall;

        // -------------------------------------------------
        // STATE MACHINE UPDATE
        // -------------------------------------------------
        stateMachine.Update();

        // -------------------------------------------------
        // JUMP INPUT
        // -------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((isGrounded || isTouchingWall) && jumpCount < maxJumps)
            {
                stateMachine.ChangeState(jumpState);
            }
        }

        // -------------------------------------------------
        // ATTACK INPUT
        // -------------------------------------------------
        if (Input.GetKey(KeyCode.LeftControl))
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                Shoot();
                fireTimer = 0f;
            }
        }
        else
        {
            fireTimer = fireRate; // instant fire when pressed again
        }

        // -------------------------------------------------
        // WALL SLIDE
        // -------------------------------------------------
        if (!isGrounded && isTouchingWall && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -wallSlideSpeed
            );
        }

        // -------------------------------------------------
        // DEBUG (REMOVE LATER)
        // -------------------------------------------------
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }

        // upgrade fire rate for testing

        // ANIMATION UPDATE
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", isGrounded);
    }

    // =====================================================
    // 🔹 MOVEMENT HELPERS
    // =====================================================
    public void SetVelocity(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (jumpCount >= maxJumps) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;

        anim.SetBool("isGrounded", false);
    }

    // =====================================================
    // 🔹 SHOOTING
    // =====================================================
    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        float dir = transform.localScale.x;

        PlayerProjectile proj = bullet.GetComponent<PlayerProjectile>();

        if (proj != null)
        {
            proj.SetDirection(dir);
        }

        anim.SetTrigger("Shoot");
    }

    public void UpgradeFireRate(float amount)
    {
        fireRate -= amount;
        fireRate = Mathf.Clamp(fireRate, 0.1f, 1f);
    }

    // =====================================================
    // 🔹 DAMAGE SYSTEM
    // =====================================================
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            stateMachine.ChangeState(deadState);
        }
        else
        {
            stateMachine.ChangeState(hurtState);
        }

        anim.SetBool("isHurt", true);
    }
}