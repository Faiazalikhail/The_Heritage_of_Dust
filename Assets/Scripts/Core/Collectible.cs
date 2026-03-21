using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum CollectibleType
    {
        Money,
        Card,
        Chest
    }

    public CollectibleType type;

    public int value = 1; 

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player == null) return;

        switch (type)
        {
            case CollectibleType.Money:
                GameManager.Instance.AddMoney(value);
                break;

            case CollectibleType.Card:
                player.Heal(value);
                break;

            case CollectibleType.Chest:
                player.UpgradeFireRate(0.1f);
                break;
        }

        Destroy(gameObject);
    }
}