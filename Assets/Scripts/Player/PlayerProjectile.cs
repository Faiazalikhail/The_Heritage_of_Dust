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
        // Check if we hit anything that ISN'T the player
        if (!other.CompareTag("Player"))
        {
            // This handles hitting walls or barriers
            Destroy(gameObject);
        }

        // Use 'other' here because that is the name in the parentheses above
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage();
            Destroy(gameObject);
        }
    }
}