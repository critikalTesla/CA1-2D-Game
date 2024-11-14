using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;        // Максимальное здоровье врага
    private int currentHealth;         // Текущее здоровье врага

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

    private Animator animator;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth; // Устанавливаем начальное здоровье
        if (leftPatrolPoint != null)
        {
            transform.position = leftPatrolPoint.position;
        }
    }

    void Update()
    {
        if (isAttacking)
        {
            animator.SetBool("isRunning", false);
            return;
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

        animator.SetBool("isRunning", true);
        UpdateDirection(targetPoint.position.x - transform.position.x);

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
            isAttacking = true;
            animator.SetTrigger("isAttacking");
            SpawnProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void SpawnProjectile()
    {
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        isAttacking = false;
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

    // Метод получения урона
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Проверка на смерть
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Здесь можно добавить анимацию смерти или эффекты
        Destroy(gameObject); // Уничтожаем объект врага
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверка на столкновение с объектом, наносящим урон (например, снаряд)
        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(10); // Получаем урон в размере 10 единиц (или другое значение)
            Destroy(collision.gameObject); // Уничтожаем снаряд
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