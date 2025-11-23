using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameTimer : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI timerText;

    [Header("Settings")]
    public float timeElapsed;
    public bool isTimerRunning = true;

    void Update()
    {
        if (isTimerRunning)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        // Math to calculate Minutes and Seconds
        // Mathf.FloorToInt removes decimals (e.g., 60.5 becomes 60)
        int minutes = Mathf.FloorToInt(timeElapsed / 60);
        int seconds = Mathf.FloorToInt(timeElapsed % 60);

        // Format string: "{0:00}" ensures it always shows two digits (e.g., 05 instead of 5)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }
}