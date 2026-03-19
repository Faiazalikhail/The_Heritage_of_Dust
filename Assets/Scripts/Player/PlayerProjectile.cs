using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, 0f);

        // flip sprite
        transform.localScale = new Vector3(dir, 1, 1);
    }

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}