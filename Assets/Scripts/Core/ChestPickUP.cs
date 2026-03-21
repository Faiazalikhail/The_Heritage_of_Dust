using UnityEngine;

public class ChestPickup : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager ui = FindFirstObjectByType<UIManager>();

            if (ui != null)
            {
                ui.AddKey(value);
            }

            Destroy(gameObject);
        }
    }
}