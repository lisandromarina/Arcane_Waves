using UnityEngine;

public class EnemyRange : Enemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 100f;
    [SerializeField] private float rangedAttackCooldown = 2f; // Time between ranged attacks

    protected void Start()
    {
        base.Start();

    }

    protected override void DamageTrigger()
    {
        if (targetTransform == null || !IsTargetAlive(targetTransform)) return;

        // Store the target's position at the moment of attack
        Vector3 targetPositionAtShot = targetTransform.position;

        // Calculate the direction from the shooter to the target
        Vector3 directionToTarget = (targetPositionAtShot - transform.position).normalized;

        // Define a maximum distance for the projectile to travel
        float maxProjectileDistance = 1000f; // Set this to the desired maximum range

        // Calculate the extended target position further along the direction
        Vector3 extendedTargetPosition = transform.position + directionToTarget * maxProjectileDistance;

        // Instantiate a projectile towards the extended target position
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileComponent = projectile.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            // Set the projectile's extended target position and speed
            projectileComponent.SetTarget(extendedTargetPosition);
            projectileComponent.SetSpeed(projectileSpeed);
            projectileComponent.SetColliderTargets(attackTags);
            projectileComponent.SetDamage(CalculateDamage());
        }
        else
        {
            Debug.LogWarning("Projectile component is missing on the instantiated object.");
        }

        Debug.Log("Attacking range");

    }

    private bool IsTargetAlive(Transform target)
    {
        Health targetHealth = target.GetComponent<Health>();
        return targetHealth != null && targetHealth.IsAlive;
    }

}