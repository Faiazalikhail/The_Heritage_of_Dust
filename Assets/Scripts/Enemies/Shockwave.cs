using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float speed = 5f;
    private float direction;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        rb.linearVelocity = new Vector2(direction * speed, 0f);
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(1);
        }
    }
}