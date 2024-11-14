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

    // Ссылка на Animator
    private Animator animator;

    // Флаг, чтобы остановить движение во время атаки
    private bool isAttacking = false;

    void Start()
    {
        // Подключаем Animator
        animator = GetComponent<Animator>();
        
        // Устанавливаем начальное положение патрулирования
        if (leftPatrolPoint != null)
        {
            transform.position = leftPatrolPoint.position;
        }
    }

    void Update()
    {
        // Если мы атакуем, движение запрещено
        if (isAttacking)
        {
            animator.SetBool("isRunning", false); // Останавливаем анимацию бега
            return; // Не выполняем движение или патрулирование, пока атака идет
        }

        UpdateFirePointPosition();
        DetectPlayer();

        if (isChasing)
        {
            ChasePlayer();
            AttackPlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void UpdateFirePointPosition()
    {
        if (firePoint != null)
        {
            float direction = movingRight ? 1 : -1;
            firePoint.localPosition = new Vector3(direction * firePointOffset, 0, 0);
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

        // Обновление анимации бега и направления
        animator.SetBool("isRunning", true);
        UpdateDirection(targetPoint.position.x - transform.position.x);

        // Переход к другой точке патруля
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
            animator.SetBool("isRunning", false);
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.position.x, transform.position.y),
                chaseSpeed * Time.deltaTime
            );

            movingRight = player.position.x > transform.position.x;
            animator.SetBool("isRunning", true);
            UpdateDirection(player.position.x - transform.position.x);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            isAttacking = true; // Устанавливаем флаг атаки
            animator.SetTrigger("isAttacking");
            SpawnProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void SpawnProjectile()
    {
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        isAttacking = false; // Сбрасываем флаг атаки сразу после выпуска снаряда
    }

    private void UpdateDirection(float direction)
    {
        // Отражаем врага по горизонтали в зависимости от направления движения
        if (direction != 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (direction > 0 ? 1 : -1);
            transform.localScale = localScale;
        }
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