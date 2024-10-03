using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameAssets : MonoBehaviour
{
    public static GameAssets i { get; private set; }

    void Awake()
    {
        // Check if an instance of GameAssets already exists
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject); // Keep this GameObject across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances if any
        }
    }

    public Transform damagePopup;

    public GameObject PlayerPrefab;

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}
