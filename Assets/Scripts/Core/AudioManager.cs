using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip backgroundMusic;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume);
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayMusic()
    {
        audioSource.Play();
    }
}