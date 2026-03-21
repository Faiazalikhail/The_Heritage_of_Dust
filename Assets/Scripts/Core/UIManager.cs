using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI chestText;

    public GameObject gameOverText;
    public GameObject winText;

    private int money = 0;
    private int keys = 0;

    void Update()
    {
        healthText.text = "Health: " + player.currentHealth;
        moneyText.text = "Money: " + money;
        chestText.text = "Keys: " + keys;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void AddKey(int amount)
    {
        keys += amount;
    }

    public int GetKeys()
    {
        return keys;
    }

    public void ShowGameOver()
    {
        gameOverText.SetActive(true);
    }

    public void ShowWin()
    {
        winText.SetActive(true);
    }
}