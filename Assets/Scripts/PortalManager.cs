using UnityEngine;

public class PortalManager : MonoBehaviour
{

    public static GameObject CreatePortal(Vector3 position, GameObject prefabToInstantiate)
    {
        // Instantiate the portal prefab
        GameObject portalPrefab = PrefabManager.Instance.portalPrefab;
        GameObject portal = Instantiate(portalPrefab, position, Quaternion.identity);
        PortalScript portalScript = portal.GetComponent<PortalScript>();
        Debug.Log(portalScript == null);
        // Check if the Portal component is found
        if (portalScript != null)
        {
            // Call Respawn method and get the created prefab
            GameObject createdPrefab = portalScript.RespawnUnit(prefabToInstantiate);

            // Return the created prefab
            return createdPrefab;
        }

        // Return null if Portal component is not found
        return null;
    }

    // Call this method to activate the portal and see the animation
    public void ActivatePortal(GameObject portal)
    {
        portal.SetActive(true);
    }
}
