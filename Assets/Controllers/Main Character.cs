using System.Collections;
using UnityEngine;

public class PlayerScript2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private float HorizontalMove = 0f;
    private bool FacingRight = true;
    private float jumpCooldown = 0.2f;
    private float lastJumpTime;

    [Header("Player Movement Settings")]
    [Range(0, 10f)] public float speed = 1f;
    [Range(0, 15f)] public float jumpForce = 8f;

    [Header("Player Animation Settings")]
    public Animator animator;

    [Header("Ground Checker Settings")]
    public bool isGrounded = false;
    [Range(-5f, 5f)] public float checkGroundOffsetY = -1.8f;
    [Range(0, 5f)] public float checkGroundRadius = 0.3f;
    public LayerMask groundLayer; 

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;   // Префаб снаряда
    public Transform firePoint;           // Точка вылета снаряда
    public float fireCooldown = 0.5f;     // Интервал между выстрелами
    private float lastFireTime;

    [Header("Dash Settings")]
    public float dashDistance = 5f;       // Дистанция рывка
    public float dashCooldown = 2f;       // Интервал между рывками
    private float lastDashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleShooting();
        HandleDash();
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = new Vector2(HorizontalMove, rb.velocity.y);
        rb.velocity = targetVelocity;
        CheckGround();
    }

    private void HandleMovement()
    {
        HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        animator.SetFloat("HorizontalMove", Mathf.Abs(HorizontalMove));

        if (HorizontalMove < 0 && FacingRight)
        {
            Flip();
        }
        else if (HorizontalMove > 0 && !FacingRight)
        {
            Flip();
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && Time.time - lastJumpTime > jumpCooldown)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }
        animator.SetBool("Jumping", rb.velocity.y > 0.1f || !isGrounded);
    }

    private void HandleShooting()
    {
        // Выстрел при нажатии левой кнопки мыши с интервалом fireCooldown
        if (Input.GetMouseButtonDown(0) && Time.time > lastFireTime + fireCooldown)
        {
            ShootProjectile();
            lastFireTime = Time.time;
        }
    }

    private void HandleDash()
    {
        // Рывок вперед при нажатии клавиши E с интервалом dashCooldown
        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastDashTime + dashCooldown)
        {
            Dash();
            lastDashTime = Time.time;
        }
    }

    private void ShootProjectile()
    {
        // Создаем снаряд в позиции firePoint с направлением, зависящим от ориентации игрока
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            float direction = FacingRight ? 1 : -1;
            projectileRb.velocity = new Vector2(direction * 10f, 0); // Скорость снаряда можно изменить на свое значение
        }
    }

    private void Dash()
    {
        // Выполняем рывок вперед на dashDistance в зависимости от ориентации игрока
        float direction = FacingRight ? 1 : -1;
        Vector2 dashPosition = new Vector2(transform.position.x + direction * dashDistance, transform.position.y);
        
        rb.MovePosition(dashPosition);
        animator.SetTrigger("Dash");  // Предполагается, что у вас есть триггер Dash в Animator
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), 
            checkGroundRadius, groundLayer);

        isGrounded = colliders.Length > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius);
    }
}