using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Patrol Settings")]
    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;
    public float patrolSpeed = 2f;
    private bool movingRight = true;

    [Header("Chase Settings")]
    public float chaseSpeed = 3f;
    public float detectionRange = 5f;
    public float stoppingDistance = 1.5f;
    private Transform player;
    private bool isChasing = false;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float firePointOffset = 0.5f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Detection Settings")]
    public LayerMask playerLayer;

    [Header("Attack Zone Timeout Settings")]
    public float attackZoneTimeout = 2f; // Time before the NPC stops chasing the player
    private float lastSeenPlayerTime;

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (leftPatrolPoint != null)
        {
            transform.position = leftPatrolPoint.position;
        }
    }

    void Update()
    {
        if (isAttacking) return; // Skip actions if attacking

        DetectPlayer(); // Check for player presence

        if (isChasing)
        {
            lastSeenPlayerTime = Time.time; // Reset the timer when the player is detected
            ChasePlayer();
            if (CanAttackPlayer()) AttackPlayer();
        }
        else if (Time.time - lastSeenPlayerTime > attackZoneTimeout)
        {
            // Return to patrolling if the player has been absent for too long
            Patrol();
        }
    }

    private void Patrol()
    {
        if (leftPatrolPoint == null || rightPatrolPoint == null) return;

        Transform targetPoint = movingRight ? rightPatrolPoint : leftPatrolPoint;

        // Move towards the patrol point
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPoint.position.x, transform.position.y),
            patrolSpeed * Time.deltaTime
        );

        // Enable running animation
        animator.SetBool("isRunning", true);
        UpdateDirection(targetPoint.position.x - transform.position.x);

        // Change direction when reaching the patrol point
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.1f)
        {
            movingRight = !movingRight;
        }
    }

    private void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider != null)
        {
            player = playerCollider.transform;
            isChasing = true;
        }
        else
        {
            player = null;
            isChasing = false;
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            // Move towards the player
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.position.x, transform.position.y),
                chaseSpeed * Time.deltaTime
            );

            movingRight = player.position.x > transform.position.x;
            animator.SetBool("isRunning", true); // Enable running animation
            UpdateDirection(player.position.x - transform.position.x);
        }
        else
        {
            animator.SetBool("isRunning", false); // Stop running animation
        }
    }

    private bool CanAttackPlayer()
    {
        return player != null && Time.time > lastAttackTime + attackCooldown;
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        isAttacking = true;
        animator.SetTrigger("isAttacking");

        // Launch the projectile after a delay to sync with the animation
        StartCoroutine(PerformAttack());
        lastAttackTime = Time.time;
    }

    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(0.5f); // Delay to sync with animation
        SpawnProjectile();
        isAttacking = false; // Allow other actions
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void UpdateDirection(float direction)
    {
        if (direction != 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (direction > 0 ? 1 : -1);
            transform.localScale = localScale;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, 1f); // Destroy the object after a delay
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (leftPatrolPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(leftPatrolPoint.position, 0.2f);
        }
        if (rightPatrolPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(rightPatrolPoint.position, 0.2f);
        }
    }
}
