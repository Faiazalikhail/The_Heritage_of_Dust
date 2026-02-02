using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload scenes

public class GameSession : MonoBehaviour
{
    // Reloads the current scene
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}