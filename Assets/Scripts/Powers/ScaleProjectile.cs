
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ScaleProjectile : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float maxDistance = 7f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Vector2 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        startPosition = rb.position;
    }

    private void FixedUpdate()
    {
        Vector2 newPosition =
            rb.position + direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);

        float distance = Vector2.Distance(
            startPosition,
            newPosition
        );

        if (distance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
