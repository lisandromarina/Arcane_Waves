using UnityEngine;

public class FadeOutHandler : MonoBehaviour
{
    private AudioSource audioSource;
    private float fadeDuration;
    private float startVolume;
    private float fadeOutSpeed;

    public void Setup(AudioSource audioSource, float fadeDuration)
    {
        this.audioSource = audioSource;
        this.fadeDuration = fadeDuration;
        startVolume = audioSource.volume;
        fadeOutSpeed = startVolume / fadeDuration;  // Calculate the speed at which the volume should fade
    }

    void Update()
    {
        if (audioSource != null && audioSource.volume > 0)
        {
            // Reduce the volume over time
            audioSource.volume -= fadeOutSpeed * Time.deltaTime;

            // Once the volume reaches zero, stop the sound and destroy the object
            if (audioSource.volume <= 0)
            {
                audioSource.Stop();
                audioSource.volume = startVolume;  // Reset volume in case it's reused
                Destroy(gameObject);  // Destroy the sound object
            }
        }
    }
}
