using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    [Header("Prefabs")]
    public GameObject tankPrefab;
    /*public GameObject magePrefab;
    public GameObject warriorPrefab;*/

    void Awake()
    {
        // Singleton pattern to ensure only one instance of PrefabManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}