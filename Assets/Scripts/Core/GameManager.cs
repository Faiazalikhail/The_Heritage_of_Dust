using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int money = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Money: " + money);
    }
}