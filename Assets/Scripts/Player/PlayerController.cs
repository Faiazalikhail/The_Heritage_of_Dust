using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // =====================================================
    // 🔹 CORE COMPONENTS
    // =====================================================
    private Rigidbody2D rb;
    private Animator anim;
    private StateMachine stateMachine;

    // =====================================================
    // 🔹 MOVEMENT
    // =====================================================
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float runMultiplier = 1.5f;
    public float horizontalInput;

    // =====================================================
    // 🔹 JUMP SYSTEM
    // =====================================================
    [Header("Jump")]
    public float jumpForce = 15f;
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
    public bool isWallSliding;

    // =====================================================
    // 🔹 SHOOTING SYSTEM
    // =====================================================
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Fire Rate")]
    public float fireRate = 1f;
    private float fireTimer;

    // =====================================================
    // 🔹 HEALTH SYSTEM
    // =====================================================
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    bool isInvincible = false;

    // =====================================================
    // 🔹 STATES
    // =====================================================
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerJumpState jumpState;
    public PlayerHurtState hurtState;
    public PlayerDeadState deadState;

    // =====================================================
    // 🔹 UNITY METHODS
    // =====================================================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stateMachine = new StateMachine();

        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        hurtState = new PlayerHurtState(this, stateMachine);
        deadState = new PlayerDeadState(this, stateMachine);

        currentHealth = maxHealth;
        fireTimer = fireRate; // instant first shot
    }

    void Start()
    {
        stateMachine.Initialize(idleState);
        currentHealth = 2;
    }

    void Update()
    {
        CheckSurroundings();
        HandleInput();
        ApplyWallSlidePhysics();

        stateMachine.Update();

        UpdateAnimator();

        HandleDebugInput();
    }

    // =====================================================
    // 🔹 INPUT
    // =====================================================
    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((isGrounded || isTouchingWall) && jumpCount < maxJumps)
            {
                stateMachine.ChangeState(jumpState);
            }
        }

        // SHOOT 
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Shoot();
            fireTimer = fireRate;
        }

        // HOLD → continuous fire
        if (Input.GetKey(KeyCode.LeftControl))
        {
            fireTimer -= Time.deltaTime;

            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }  
    }

    // =====================================================
    // 🔹 ENVIRONMENT CHECK
    // =====================================================
    private void CheckSurroundings()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        bool leftWall = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, groundLayer);
        bool rightWall = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, groundLayer);

        isTouchingWall = leftWall || rightWall;
    }

    // =====================================================
    // 🔹 WALL SLIDE
    // =====================================================
    private void ApplyWallSlidePhysics()
    {
        isWallSliding = (!isGrounded && isTouchingWall && rb.linearVelocity.y < 0);

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
    }

    // =====================================================
    // 🔹 ANIMATION
    // =====================================================
    private void UpdateAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallSliding", isWallSliding);

        anim.SetInteger("jumpCount", jumpCount);
        anim.SetBool("isShooting", Input.GetKey(KeyCode.LeftControl));
    }

    // =====================================================
    // 🔹 ACTIONS
    // =====================================================
    public void SetVelocity(float x)
    {
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= runMultiplier;
        }

        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (jumpCount >= maxJumps) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpCount++;
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float dir = transform.localScale.x;

        PlayerProjectile proj = bullet.GetComponent<PlayerProjectile>();

        if (proj != null)
        {
            proj.SetDirection(dir);
        }

        anim.SetTrigger("Shoot");
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            // DEATH
            anim.SetBool("isHurt", false);
            anim.SetBool("isDead", true);

            stateMachine.ChangeState(deadState);

            StartCoroutine(HandleDeath());
        }
        else
        {
            // HURT 
            anim.SetTrigger("Hurt"); 
            stateMachine.ChangeState(hurtState);
        }
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.1f);

        FadeController fade = FindFirstObjectByType<FadeController>();

        if (fade != null)
        {
            fade.FadeToBlack(3f);
        }

        yield return new WaitForSeconds(3f);

        UIManager ui = FindFirstObjectByType<UIManager>();

        if (ui != null)
        {
            ui.ShowGameOver();
        }

        Time.timeScale = 0f;
    }

    public void UpgradeFireRate(float amount)
    {
        fireRate = Mathf.Clamp(fireRate - amount, 0.1f, 1f);
    }

    private void HandleDebugInput()
    {
        // Take 1 damage
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("DEBUG: Take Damage");
            TakeDamage(1);
        }

        // Heal 1
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
            Debug.Log("DEBUG: Heal → " + currentHealth);
        }

        // Instant death
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("DEBUG: Instant Death");
            TakeDamage(999);
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public GameObject gameOverText;

    public void ShowGameOver()
    {
        gameOverText.SetActive(true);
    }
}