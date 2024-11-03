using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;   // Точки патрулирования
    public float patrolSpeed = 2f;     // Скорость патрулирования
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;

    [Header("Chase Settings")]
    public float chaseSpeed = 3f;      // Скорость преследования
    public float detectionRange = 5f;  // Радиус обнаружения игрока
    public float attackRange = 3f;     // Радиус атаки, если игрок находится достаточно близко
    private Transform player;

    [Header("Attack Settings")]
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform firePoint;         // Точка, откуда выпускается снаряд
    public float projectileSpeed = 5f;
    public float attackCooldown = 1.5f; // Задержка между атаками
    private float lastAttackTime;

    [Header("Detection Settings")]
    public LayerMask playerLayer;       // Слой игрока

    void Start()
    {
        // Устанавливаем начальную точку патрулирования
        currentPatrolIndex = 0;
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[currentPatrolIndex].position;
        }
    }

    void Update()
    {
        DetectPlayer();

        if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            ChasePlayer();
            AttackPlayer();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Движение к текущей точке патрулирования
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        // Переход к следующей точке, если достигли текущей
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void DetectPlayer()
    {
        // Проверяем, находится ли игрок в зоне обнаружения
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (playerCollider != null)
        {
            player = playerCollider.transform;
            isPatrolling = false;
        }
        else
        {
            player = null;
            isPatrolling = true;
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // Двигаемся в сторону игрока, если он в пределах зоны обнаружения
        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        // Поворачиваем NPC в сторону игрока
        Vector2 direction = (player.position - transform.position).normalized;
        transform.right = direction;

        // Атака, если игрок в пределах зоны атаки и прошло достаточно времени с момента последней атаки
        if (Vector2.Distance(transform.position, player.position) <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            ShootProjectile(direction);
            lastAttackTime = Time.time;
        }
    }

    private void ShootProjectile(Vector2 direction)
    {
        // Создаем снаряд и задаем его начальную скорость
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализируем радиусы обнаружения и атаки игрока
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}