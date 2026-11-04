
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuracion del enemigo")]
    public Transform player;
    public float detectionRadius = 5f;
    public float speed = 2f;
    public int damageAmount = 1;

    [Header("Animaciones")]
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private SpriteRenderer enemySprite;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float stunTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Buscar componentes en el objeto o sus hijos.
        if (enemyAnimator == null)
            enemyAnimator = GetComponentInChildren<Animator>();

        if (enemySprite == null)
            enemySprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            movement = Vector2.zero;
            UpdateAnimations();
            return;
        }

        if (player == null)
        {
            movement = Vector2.zero;
            UpdateAnimations();
            return;
        }

        float distanceToPlayer =
            Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction =
                (player.position - transform.position).normalized;

            movement = direction;
        }
        else
        {
            movement = Vector2.zero;
        }

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        // Actualizar animación Idle / Walk.
        if (enemyAnimator != null)
        {
            enemyAnimator.SetFloat("Speed", movement.magnitude);
        }

        // Girar el sprite según la dirección horizontal.
        if (enemySprite != null)
        {
            if (movement.x < -0.01f)
                enemySprite.flipX = true;
            else if (movement.x > 0.01f)
                enemySprite.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.MovePosition(
            rb.position + movement * speed * Time.fixedDeltaTime
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("El enemigo choco con: " + collision.gameObject.name);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            else
            {
                Debug.Log("Choque pero no hay script playerhealth");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void Stun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
        movement = Vector2.zero;
        UpdateAnimations();
    }
}
