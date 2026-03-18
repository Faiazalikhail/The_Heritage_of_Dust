using UnityEngine;

public class SoldierEnemy : BaseEnemy
{
    [Header("Movement")]
    public float speed = 2f;
    private Rigidbody2D rb;
    private int direction = -1;

    [Header("Wall Detection")]
    public Transform frontCheck;
    public float checkDistance = 0.5f;
    public LayerMask wallLayer;

    [Header("Attack")]
    public Transform player;
    public float attackRange = 3f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float fireTimer = 0f;
    private bool isTurning = false;
    private bool isAttacking = false;
    private bool inFireFrame = false;

    public void EnterFireFrame() { inFireFrame = true; }
    public void ExitFireFrame() { inFireFrame = false; }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim.SetBool("IsAttacking", false);
    }

    void Update()
    {
        if (health <= 0) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // -------- ATTACK --------
        isAttacking = distance <= attackRange;

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("IsAttacking", true);

            if (inFireFrame)
            {
                fireTimer += Time.deltaTime;

                if (fireTimer >= fireRate)
                {
                    Fire();
                    fireTimer = 0f;
                }
            }

            return;
        }
        else
        {
            anim.SetBool("IsAttacking", false);
        }

        // -------- TURN BLOCK --------
        if (isTurning)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // -------- MOVE --------
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        transform.localScale = new Vector3(direction, 1, 1);

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        // -------- WALL CHECK --------
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, Vector2.right * direction, checkDistance, wallLayer);

        if (hit.collider != null)
        {
            Turn();
        }
    }

    void Turn()
    {
        isTurning = true;
        direction *= -1;

        anim.SetBool("isTurning", true);
        Invoke(nameof(StopTurning), 0.3f);
    }

    void StopTurning()
    {
        isTurning = false;
        anim.SetBool("isTurning", false);
    }

    void Fire()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float dir = transform.localScale.x;

        EnemyProjectile proj = bullet.GetComponent<EnemyProjectile>();

        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }

    protected override void Die()
    {
        anim.SetBool("isDead", true);
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }
}