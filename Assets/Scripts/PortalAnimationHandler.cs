using UnityEngine;

public class PortalAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private bool animationCompleted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
    }

    void Update()
    {
        if (animationCompleted)
        {
            Destroy(gameObject);
        }
    }

    public void OnAnimationEnd()
    {
        animationCompleted = true;
    }
}
