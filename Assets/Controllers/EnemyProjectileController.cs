using UnityEngine;

public class EnemyProjectileController : MonoBehaviour
{
    public float speed = 5f; // Speed of the projectile
    public float lifetime = 3f; // Time before the projectile is destroyed
    public int damage = 5; // Damage dealt by the projectile
    private Transform player;

    void Start()
    {
        // Find the player object using its tag
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Destroy the projectile after a set lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player != null)
        {
            // If the player exists, move towards the player
            MoveTowardsTarget(player);
        }
        else
        {
            // Otherwise, move forward in a straight line
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    private void MoveTowardsTarget(Transform target)
    {
        if (target == null) return;

        // Calculate direction towards the target
        Vector2 direction = (target.position - transform.position).normalized;

        // Calculate the angle for rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the projectile to face the target
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move the projectile in the calculated direction
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collided object has a HealthController, apply damage
        if (collision.TryGetComponent(out HealthController healthController))
        {
            healthController.TakeDamage(damage);
        }

        // Destroy the projectile if it hits an obstacle
        if (collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }

        // Destroy the projectile after any collision
        Destroy(gameObject);
    }
}