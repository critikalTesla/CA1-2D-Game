using UnityEngine;

public class PlayerProjectileController : MonoBehaviour
{
    public float speed = 10f;          // Speed of the projectile
    public float lifetime = 3f;        // Lifetime of the projectile
    public int damage = 5;             // Damage dealt by the projectile
    public GameObject player;          // Reference to the player object (can be assigned in the inspector)

    private Vector3 direction;         // Direction of the projectile's movement

    void Start()
    {
        if (player == null)
        {
            // If the player reference is not set in the inspector, try to find the object tagged "Player"
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("Player object not found. Make sure the player is tagged as 'Player'.");
                return; // Stop execution if the player object is not found
            }
        }

        // Determine the horizontal direction based on the player's facing direction
        if (player != null)
        {
            // If the player is facing right, the projectile moves right
            // If the player is facing left, the projectile moves left
            direction = (player.transform.localScale.x > 0) ? Vector3.right : Vector3.left;
        }

        // Destroy the projectile after the specified lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player == null) return; // Stop updates if the player object is not found

        // Move the projectile horizontally
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // If the projectile collides with an object that has a HealthController, apply damage
        if (collision.TryGetComponent(out HealthController healthController))
        {
            healthController.TakeDamage(damage);
        }

        // Destroy the projectile after a collision
        Destroy(gameObject);
    }
}
