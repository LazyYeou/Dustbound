using UnityEngine;  
using UnityEngine.SceneManagement;  

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;  
    private bool isPaused = false;  

    void Update()
    {
        // Deteksi tombol pause (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed!");
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);  // Tampilkan panel
        Time.timeScale = 0f;  // Freeze waktu game (enemy/player berhenti)
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);  // Sembunyikan panel
        Time.timeScale = 1f;  // Lanjutkan waktu game
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;  // Pastikan waktu normal sebelum exit
                              // Load scene menu utama (ganti "MainMenu" dengan nama scene Anda)
        SceneManager.LoadScene("MainMenu");
        // Atau quit aplikasi: Application.Quit(); (untuk build, bukan editor)
    }
}
