using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Ground Detection")]
    public Transform feetPos;
    public float checkRadius = 0.3f;
    public LayerMask whatIsGround;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public GameObject shootEffect;
    public float fireRate = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private float moveInput;

    private bool inFireFrame = false;
    private float fireTimer = 0f;
    private GameObject currentEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // ---- Ground Check ----
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        moveInput = Input.GetAxisRaw("Horizontal");

        // ---- Movement ----
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        // ---- Flip Sprite ----
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // ---- Shooting Logic ----
        HandleShooting();

        // ---- Animator Parameters ----
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsJumping", !isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void HandleShooting()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            anim.SetBool("IsShooting", true);

            if (inFireFrame)
            {
                fireTimer += Time.deltaTime;

                if (fireTimer >= fireRate)
                {
                    FireProjectile();
                    fireTimer = 0f;
                }

                // Freeze animation at fire frame
                anim.speed = 0f;
            }
        }
        else
        {
            anim.speed = 1f;
            anim.SetBool("IsShooting", false);
            fireTimer = 0f;
        }
    }

    // Called from Animation Event on fire frame
    public void EnterFireFrame()
    {
        inFireFrame = true;

        // First shot fires instantly
        FireProjectile();
        fireTimer = 0f;
    }

    // Called from Animation Event after fire frame
    public void ExitFireFrame()
    {
        inFireFrame = false;
    }

    void FireProjectile()
    {
        // Spawn bullet
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        float dir = transform.localScale.x;
        bullet.GetComponent<PlayerProjectile>().SetDirection(dir);

        // Clear previous effect
        if (currentEffect != null)
            Destroy(currentEffect);

        if (shootEffect != null)
        {
            currentEffect = Instantiate(shootEffect, firePoint.position, Quaternion.identity, firePoint);
            Destroy(currentEffect, 0.4f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}