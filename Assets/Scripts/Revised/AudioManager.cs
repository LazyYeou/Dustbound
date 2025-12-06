using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] soundSources;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Play(string soundName)
    {
        foreach (AudioSource source in soundSources)
        {
            if (source.gameObject.name == soundName)
            {
                if (!source.loop)
                    source.pitch = Random.Range(0.9f, 1.1f);

                source.Play();
                return;
            }
        }

        Debug.LogWarning("Sound not found: " + soundName);
    }

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