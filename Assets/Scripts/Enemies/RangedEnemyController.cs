
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class RangedEnemyController : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform player;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float distanceTolerance = 0.5f;

    [Header("Disparo")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootInterval = 2f;
    [SerializeField] private float projectileSpawnOffset = 0.4f;
    [SerializeField] private float shootAnimationDuration = 0.50f;

    [Header("Animaciones")]
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private SpriteRenderer enemySprite;

    private Rigidbody2D rb;
    private Vector2 movement;
    private float shootTimer;
    private bool isShooting;
    private float stunTimer = 0f;
    private Coroutine shootingCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (enemyAnimator == null)
            enemyAnimator = GetComponentInChildren<Animator>();

        if (enemySprite == null)
            enemySprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        shootTimer = shootInterval;
    }

    private void Update()
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

        Vector2 toPlayer =
            (Vector2)player.position - rb.position;

        float distance = toPlayer.magnitude;

        // Calcular movimiento para mantener la distancia.
        if (distance > preferredDistance + distanceTolerance)
        {
            movement = toPlayer.normalized;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            movement = -toPlayer.normalized;
        }
        else
        {
            movement = Vector2.zero;
        }

        // No moverse mientras reproduce el disparo.
        if (isShooting)
            movement = Vector2.zero;

        // Orientar al enemigo hacia el jugador.
        UpdateFacing(toPlayer);

        // Actualizar animaciones de movimiento.
        UpdateAnimations();

        // Controlar intervalo de disparo.
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval && !isShooting)
        {
            Shoot(toPlayer);
            shootTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(
                rb.position +
                movement * speed * Time.fixedDeltaTime
            );
        }
    }

    private void UpdateAnimations()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetFloat("Speed", movement.magnitude);
        }
    }

    private void UpdateFacing(Vector2 direction)
    {
        if (enemySprite == null)
            return;

        if (direction.x < -0.01f)
            enemySprite.flipX = true;
        else if (direction.x > 0.01f)
            enemySprite.flipX = false;
    }

    private void Shoot(Vector2 direction)
    {
        Debug.Log("SE EJECUTÓ SHOOT()");

        if (projectilePrefab == null ||
            direction.sqrMagnitude == 0f ||
            isShooting)
            return;

        direction.Normalize();

        shootingCoroutine = StartCoroutine(ShootAnimation(direction));
    }

    private IEnumerator ShootAnimation(Vector2 direction)
    {
        isShooting = true;
        movement = Vector2.zero;

        // Iniciar animación de disparo.
        if (enemyAnimator != null)
        {
            Debug.Log("SE ACTIVÓ EL TRIGGER SHOOT");
            enemyAnimator.SetTrigger("Shoot");
        }

        // Esperar un momento hasta el instante del disparo.
        yield return new WaitForSeconds(0.25f);

        // Crear un único proyectil.
        Vector2 spawnPosition = rb.position
            + direction * projectileSpawnOffset;

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        EnemyProjectile projectileScript =
            projectile.GetComponent<EnemyProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Launch(direction);
        }
        else
        {
            Debug.LogError("El prefab no tiene EnemyProjectile");
            Destroy(projectile);
        }

        // Esperar a que termine la pose de disparo.
        yield return new WaitForSeconds(
            shootAnimationDuration - 0.25f
        );

        isShooting = false;
    }

    public void Stun(float duration)
{
    stunTimer = Mathf.Max(stunTimer, duration);

    movement = Vector2.zero;

    // Interrumpir el disparo si está en proceso.
    if (shootingCoroutine != null)
    {
        StopCoroutine(shootingCoroutine);
        shootingCoroutine = null;
    }

    isShooting = false;

    UpdateAnimations();
}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            preferredDistance
        );
    }
}
