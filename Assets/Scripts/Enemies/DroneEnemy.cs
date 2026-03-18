using UnityEngine;

public class DroneEnemy : BaseEnemy
{
    [Header("Hover")]
    public float speed = 2f;
    private int direction = 1;
    private Rigidbody2D rb;

    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Attack")]
    public Transform player;
    public float detectionRange = 4f;
    public float dropSpeed = 12f;
    public float attackDelay = 0.5f;

    private float attackTimer = 0f;

    private bool isAttacking = false;
    private bool isDropping = false;
    private bool isDeadState = false;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (isDeadState) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // -------- ATTACK --------
        if (distance <= detectionRange && !isAttacking && !isDropping)
        {
            isAttacking = true;
            attackTimer = attackDelay;

            anim.SetBool("IsAttacking", true);
            rb.linearVelocity = Vector2.zero;
        }

        // -------- DELAY --------
        if (isAttacking && !isDropping)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                isDropping = true;
                isAttacking = false;
                anim.SetBool("IsAttacking", false);
            }

            return;
        }

        // -------- DROP --------
        if (isDropping)
        {
            rb.linearVelocity = new Vector2(0, -dropSpeed);
            return;
        }

        // -------- PATROL --------
        rb.linearVelocity = new Vector2(direction * speed, 0f);
        transform.localScale = new Vector3(direction, 1, 1);

        if (transform.position.x > rightPoint.position.x)
            direction = -1;
        else if (transform.position.x < leftPoint.position.x)
            direction = 1;

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SimpleMover player = collision.gameObject.GetComponent<SimpleMover>();
            if (player != null) player.TakeDamage();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            ResetDrone();
        }
    }

    void ResetDrone()
    {
        isDropping = false;
        isAttacking = false;

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        anim.SetBool("IsAttacking", false);
    }

    protected override void Die()
    {
        isDeadState = true;

        anim.SetBool("isDead", true);
        rb.gravityScale = 2f;
        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject, 2f);
    }
}