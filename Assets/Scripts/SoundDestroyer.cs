using UnityEngine;

public class SoundDestroyer : MonoBehaviour
{
    private AudioSource audioSource;

    // Initialize the AudioSource reference
    public void Setup(AudioSource source)
    {
        audioSource = source;
    }

    void Update()
    {
        // If the AudioSource has finished playing and is not looping, destroy the GameObject
        if (!audioSource.isPlaying && !audioSource.loop)
        {
            Destroy(gameObject);
        }
    }
}
