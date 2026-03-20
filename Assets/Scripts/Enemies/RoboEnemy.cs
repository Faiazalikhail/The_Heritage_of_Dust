using UnityEngine;

public class RoboEnemy : MonoBehaviour
{
    // =============================
    // HEALTH SYSTEM
    // =============================
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    // =============================
    // MOVEMENT
    // =============================
    [Header("Movement")]
    public float speed = 2f;

    public Transform leftPoint;
    public Transform rightPoint;

    private int direction = 1;

    private Rigidbody2D rb;
    private Animator anim;

    // =============================
    // EDGE DETECTION
    // =============================
    [Header("Edge Detection")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;

    // =============================
    // WALL DETECTION
    // =============================
    [Header("Wall Detection")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.3f;

    private float turnCooldown = 0.2f;
    private float turnTimer = 0f;

    // =============================
    // PLAYER DETECTION
    // =============================
    [Header("Player Detection")]
    public Transform player;
    public float detectionRange = 5f;

    private bool isPlayerDetected;

    // =============================
    // SHOOTING
    // =============================
    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float fireTimer = 0f;

    // =============================
    // UNITY METHODS
    // =============================
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // INIT HEALTH
        currentHealth = maxHealth;
    }

    void Update()
    {
        // STOP EVERYTHING IF DEAD
        if (isDead) return;

        DetectPlayer();

        if (isPlayerDetected)
        {
            StopAndFacePlayer();
            HandleShooting();
        }
        else
        {
            Patrol();
        }

        UpdateAnimation();
    }

    // =============================
    // PATROL
    // =============================
    void Patrol()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        transform.localScale = new Vector3(direction, 1, 1);

        RaycastHit2D groundInfo = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        RaycastHit2D wallInfo = Physics2D.Raycast(
            wallCheck.position,
            new Vector2(direction, 0),
            wallCheckDistance,
            groundLayer
        );

        turnTimer -= Time.deltaTime;

        if ((!groundInfo || wallInfo) && turnTimer <= 0f)
        {
            direction *= -1;
            turnTimer = turnCooldown;
        }
    }

    void UpdateAnimation()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    // =============================
    // PLAYER DETECTION
    // =============================
    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        isPlayerDetected = distance <= detectionRange;
    }

    void StopAndFacePlayer()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (player.position.x > transform.position.x)
            direction = 1;
        else
            direction = -1;

        transform.localScale = new Vector3(direction, 1, 1);
    }

    // =============================
    // SHOOTING
    // =============================
    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float dir = transform.localScale.x;

        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();

        if (proj != null)
        {
            proj.SetDirection(dir);
        }

        anim.SetTrigger("Shoot");
    }

    // =============================
    // DAMAGE SYSTEM
    // =============================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        Debug.Log("ENEMY TOOK DAMAGE");


        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hurt");
        }
    }

    

    void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        anim.SetTrigger("Death");

        // disable behavior (no more movement/shooting)
        this.enabled = false;

        // destroy after animation
        Destroy(gameObject, 1.5f);
    }

    // =============================
    // DEBUG VISUALS
    // =============================
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * direction * wallCheckDistance);
        }
    }

    
}