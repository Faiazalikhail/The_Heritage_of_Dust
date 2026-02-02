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

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Physics Checks
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Move (Using Unity 6 linearVelocity)
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // 3. Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
        }

        // 4. Flip Sprite
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // 5. ANIMATION UPDATES
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsJumping", !isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit has the tag "Trap"
        if (other.CompareTag("Trap"))
        {
            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}