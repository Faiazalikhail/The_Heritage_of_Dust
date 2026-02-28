using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float speed = 10f;
    private float direction = 1f; // 1 = right, -1 = left

    public void SetDirection(float dir)
    {
        direction = dir;
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}