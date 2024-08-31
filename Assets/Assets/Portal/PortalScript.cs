using System.Collections;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    private GameObject prefabToCreate;
    private float respawnDelay = 2f; // Delay before respawn
    private float respawnTimer = 0f;
    private bool shouldRespawn = false;

    private Animator animator;

    public GameObject RespawnUnit(GameObject prefab)
    {
        respawnTimer = 0f;
        prefabToCreate = Instantiate(prefab, transform.position, Quaternion.identity);
        prefabToCreate.SetActive(false); // Start inactive if needed
        shouldRespawn = true; // Set this to true to start the respawn process
        return prefabToCreate;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from this GameObject");
        }
    }

    void Update()
    {
        if (shouldRespawn)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
            {
                if (prefabToCreate != null)
                {
                    prefabToCreate.SetActive(true);
                    animator.SetTrigger("Close");
                    Debug.Log("Close animation triggered");
                    shouldRespawn = false;
                }
                else
                {
                    Debug.LogError("prefabToCreate is null");
                }
            }
        }
    }

    public void onCloseAnimationEnd()
    {
        Destroy(gameObject);
    }
}
