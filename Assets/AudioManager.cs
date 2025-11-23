using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // The Array requested: A raw list of AudioSource components
    public AudioSource[] soundSources;

    void Awake()
    {
        // Singleton Pattern (Standard for Managers)
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Play(string soundName)
    {
        // Loop through the array to find the AudioSource with the matching name
        foreach (AudioSource source in soundSources)
        {
            // We check the name of the GameObject holding the AudioSource
            if (source.gameObject.name == soundName)
            {
                // Optional: Randomize pitch slightly for SFX to avoid "ear fatigue"
                if (!source.loop)
                    source.pitch = Random.Range(0.9f, 1.1f);

                source.Play();
                return; // Found it, stop looking
            }
        }

        Debug.LogWarning("Sound not found: " + soundName);
    }

    // Optional: Helper to stop a looping sound (like music)
    public void Stop(string soundName)
    {
        foreach (AudioSource source in soundSources)
        {
            if (source.gameObject.name == soundName)
            {
                source.Stop();
                return;
            }
        }
    }
}