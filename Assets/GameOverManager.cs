using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;

    // Call this from PlayerStats when player dies
    public void TriggerGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Freeze the game
    }

    public void RestartGame()
    {
        // Unfreeze time BEFORE loading, or the next scene will be frozen
        Time.timeScale = 1f;

        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Make sure your menu scene is named exactly this
    }
}