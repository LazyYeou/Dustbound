using UnityEngine;
using UnityEngine.SceneManagement; // Needed for loading Main Menu
using TMPro; // Needed for Text

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;       // The entire Pause Screen
    public GameObject statsPanel;       // The specific box showing stats
    public TextMeshProUGUI statsText;   // The text component inside Stats Panel

    [Header("Scene Management")]
    public string mainMenuSceneName = "MainMenu"; // Name of your menu scene

    private bool isPaused = false;
    private PlayerStats player;
    private WeaponController weapon;

    void Start()
    {
        // Auto-find references if they exist
        player = FindFirstObjectByType<PlayerStats>();
        weapon = FindFirstObjectByType<WeaponController>();

        // Ensure menu is closed at start
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        // Toggle Pause with ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // FREEZE TIME

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            UpdateStatsDisplay(); // Refresh numbers whenever we open
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // UNFREEZE TIME

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void LoadMainMenu()
    {
        // IMPORTANT: Always reset time before leaving scene, 
        // or the next scene will start frozen!
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ToggleStatsTab()
    {
        // Simple toggle to show/hide the stats text box
        bool isActive = statsPanel.activeSelf;
        statsPanel.SetActive(!isActive);
    }

    void UpdateStatsDisplay()
    {
        if (player == null || weapon == null) return;

        // Build a string with all the cool data
        string stats = "";

        stats += $"Level: {player.level}\n";
        stats += $"HP: {player.currentHealth:F0} / {player.maxHealth:F0}\n";
        stats += $"Damage Mult: x{player.damageMultiplier:F1}\n";
        stats += $"Crit Chance: {player.critChance * 100:F0}%\n";
        stats += $"Defense: {player.defense}\n";
        stats += $"Move Speed: {player.moveSpeed:F1}\n";
        stats += $"Exp Gain: {player.expEfficiency * 100:F0}%\n";
        stats += $"\n--- WEAPON ---\n";
        stats += $"Fire Rate: {weapon.fireRate:F2}s\n";
        stats += $"Bullets: {weapon.burstCount}\n";

        // Assign to text
        if (statsText != null) statsText.text = stats;
    }
}