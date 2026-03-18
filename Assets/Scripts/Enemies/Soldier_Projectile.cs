using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, 0f);
        transform.localScale = new Vector3(dir, 1, 1);
    }

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SimpleMover player = collision.gameObject.GetComponent<SimpleMover>();
            if (player != null) player.TakeDamage();
        }

        Destroy(gameObject);
    }
}