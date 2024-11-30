using UnityEngine;

public class InstantKillPlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check player collider with the platform
        if (collision.CompareTag("Player"))
        {
            PlayerScript2D player = collision.GetComponent<PlayerScript2D>();
            if (player != null)
            {
                player.Die(); // Death metod call
            }
        }
    }
}