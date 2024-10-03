using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager {
    // Start is called before the first frame update

    public enum Sound
    {
        PlayMainMenuMusic,
        BattleMusic,
        MeleeAttack
    }

    public static void PlaySound(Sound sound, bool loop = false, float volume = 1f)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        AudioClip clipToPlay = GetAudioClip(sound);

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.volume = volume;
            audioSource.loop = loop;  // Set loop to true or false based on the parameter
            audioSource.Play();

            SoundDestroyer soundDestroyer = soundGameObject.AddComponent<SoundDestroyer>();
            soundDestroyer.Setup(audioSource);  // Pass the AudioSource to the destroyer
        }
        else
        {
            Debug.LogWarning("Audio clip is null, check your GameAssets setup.");
        }
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
