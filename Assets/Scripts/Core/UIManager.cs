using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moneyText;

    private int money = 0;

    void Update()
    {
        healthText.text = "Health: " + player.currentHealth;
        moneyText.text = "Money: " + money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }
}