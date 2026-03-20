using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 8f;

    private Rigidbody2D rb;
    private float direction;

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


    void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                PlayerController player = col.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(1);
                }
            }

            Destroy(gameObject);
        }

    void OnTriggerEnter2D(Collider2D collision)
    {
        RoboEnemy enemy = collision.GetComponent<RoboEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}