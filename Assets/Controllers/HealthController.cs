using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Maximum health
    private int currentHealth;  // Current health

    // Event for death notifications (e.g., triggering animations)
    public event System.Action OnDeath;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
    }

    // Method to take damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Decrease health by the damage amount

        if (currentHealth <= 0)
        {
            currentHealth = 0; // Clamp health to zero
            Die(); // Call the death method
        }
    }

    // Optional method for healing
    public void Heal(int healingAmount)
    {
        currentHealth += healingAmount; // Increase health by the healing amount
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Clamp health to the maximum value
        }
    }

    // Method to handle the object's death
    private void Die()
    {
        OnDeath?.Invoke(); // Invoke the death event if there are subscribers
        Destroy(gameObject); // Destroy the object (e.g., the enemy)
    }

    // Method to get the current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Method to get the maximum health
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
