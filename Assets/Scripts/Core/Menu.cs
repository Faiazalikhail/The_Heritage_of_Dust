using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Sandbox level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}