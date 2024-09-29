using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;

    [Header("Prefabs")]
    public GameObject portalPrefab;
    public GameObject projectilKingGoblin;

    public GameObject playerPrefab;
    //ALLIES HERE
    public GameObject tankPrefab;
    public GameObject spaceCadet;
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

    public List<GameObject> GetAllies()
    {
        return new List<GameObject> { playerPrefab, tankPrefab, spaceCadet /*, magePrefab, warriorPrefab*/ };
    }
}