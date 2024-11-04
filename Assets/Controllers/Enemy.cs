using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;
    public float patrolSpeed = 2f;
    private bool movingRight = true;

    [Header("Chase Settings")]
    public float chaseSpeed = 3f;
    public float detectionRange = 5f;
    private Transform player;
    private bool isChasing = false;

    [Header("Attack Settings")]
    public GameObject projectilePrefab; // Префаб снаряда
    public float attackCooldown = 1.5f; // Задержка между атаками
    private float lastAttackTime;       // Время последней атаки

    [Header("Detection Settings")]
    public LayerMask playerLayer;

    void Start()
    {
        if (leftPatrolPoint != null)
        {
            transform.position = leftPatrolPoint.position;
        }
    }

    void Update()
    {
        DetectPlayer();

        if (isChasing)
        {
            ChasePlayer();
            AttackPlayer();  // Атака во время преследования
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (leftPatrolPoint == null || rightPatrolPoint == null) return;

        Transform targetPoint = movingRight ? rightPatrolPoint : leftPatrolPoint;
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPoint.position.x, transform.position.y),
            patrolSpeed * Time.deltaTime
        );

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

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(player.position.x, transform.position.y),
            chaseSpeed * Time.deltaTime
        );
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        // Проверка на интервал времени для атаки
        if (Time.time > lastAttackTime + attackCooldown)
        {
            // Спавним префаб снаряда
            SpawnProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void SpawnProjectile()
    {
        // Спавним снаряд из позиции врага
        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

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