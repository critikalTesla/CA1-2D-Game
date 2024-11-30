using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private float HorizontalMove = 0f;
    private bool FacingRight = true;
    private float lastJumpTime;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Player Movement Settings")]
    [Range(0, 10f)] public float speed = 1f;
    [Range(0, 15f)] public float jumpForce = 8f;
    [Range(0, 10f)] public float dashDistance = 3f;
    public float dashCooldown = 1f; // Dash cooldown in seconds

    [Header("Player Animation Settings")]
    public Animator animator;

    [Space]
    [Header("Ground Checker Settings")]
    public bool isGrounded = false;
    [Range(-5f, 5f)] public float checkGroundOffsetY = -1.8f;
    [Range(0, 5f)] public float checkGroundRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Player Health Settings")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileInterval = 0.5f;
    private float lastProjectileTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Shooting projectiles when holding the left mouse button
        if (Input.GetMouseButton(0) && Time.time - lastProjectileTime > projectileInterval)
        {
            ShootProjectile();
            lastProjectileTime = Time.time;
        }

        // Dashing forward using the "E" key
        if (Input.GetKeyDown(KeyCode.E) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        // Jumping when the spacebar is pressed, and cooldown has passed
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && Time.time - lastJumpTime > 0.2f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
        }

        // Moving left and right
        HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        animator.SetFloat("HorizontalMove", Mathf.Abs(HorizontalMove));

        // Jump animation
        animator.SetBool("Jumping", rb.velocity.y > 0.1f || !isGrounded);

        // Flipping the player when changing direction
        if (HorizontalMove < 0 && FacingRight)
        {
            Flip();
        }
        else if (HorizontalMove > 0 && !FacingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Moving the player
        Vector2 targetVelocity = new Vector2(HorizontalMove, rb.velocity.y);
        rb.velocity = targetVelocity;

        // Checking if the player is grounded
        CheckGround();
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

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Vector2 dashPosition = FacingRight ? Vector2.right : Vector2.left;
        rb.MovePosition(rb.position + dashPosition * dashDistance);

        yield return new WaitForSeconds(0.1f); // Short delay for the dash

        isDashing = false;

        // Dash cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
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

    public void Die()
    {
        Debug.Log("Player has died!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the level
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If colliding with an enemy projectile, decrease health
        if (collision.CompareTag("EnemyProjectile"))
        {
            TakeDamage(5); // Damage dealt by the enemy projectile
        }
    }
}
