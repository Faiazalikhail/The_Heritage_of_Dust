using UnityEngine;

public class Door : MonoBehaviour
{
    public int requiredKeys = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        UIManager ui = FindFirstObjectByType<UIManager>();

        if (ui == null) return;

        if (ui.GetKeys() >= requiredKeys)
        {
            Debug.Log("YOU WIN");

            ui.ShowWin();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.Log("Need key!");
        }
    }
}