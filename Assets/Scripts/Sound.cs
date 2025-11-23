using UnityEngine;

[System.Serializable] // This makes it visible in the Inspector
public class Sound
{
    public string name;           // Example: "LevelUp"
    public AudioClip clip;        // Drag the .mp3/.wav here

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;    // The system will assign this automatically
}