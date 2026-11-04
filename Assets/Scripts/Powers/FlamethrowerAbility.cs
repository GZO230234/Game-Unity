
using UnityEngine;
using UnityEngine.InputSystem;

public class FlamethrowerAbility : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject flamethrowerPrefab;
    [SerializeField] private float attackInterval = 6f;
    [SerializeField] private float attackDuration = 2f;

    [Header("Daño")]
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private float damageInterval = 0.5f;

    [Header("Alcance")]
    [SerializeField] private float range = 5f;
    [SerializeField] private float coneAngle = 60f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;

    private float attackTimer;
    private FlamethrowerEffect activeEffect;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        attackTimer = attackInterval;
    }

    private void Update()
    {
        // Actualizar dirección y posición mientras el fuego está activo.
        if (activeEffect != null)
        {
            UpdateActiveFlamethrower();
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    private void Attack()
    {
        if (flamethrowerPrefab == null ||
            mainCamera == null ||
            Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Vector2 direction =
            (Vector2)(mouseWorldPosition - transform.position);

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        // El sprite original apunta hacia la izquierda.
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg + 180f;

        // Colocar el centro del efecto a mitad del alcance.
        Vector2 effectPosition =
            (Vector2)transform.position +
            direction * (range / 2f);

        GameObject fire = Instantiate(
            flamethrowerPrefab,
            effectPosition,
            Quaternion.Euler(0f, 0f, angle)
        );

        activeEffect =
            fire.GetComponent<FlamethrowerEffect>();

        if (activeEffect != null)
        {
            activeEffect.Initialize(
                transform.position,
                direction,
                enemyLayer,
                range,
                coneAngle,
                attackDuration,
                damage,
                damageInterval
            );
        }
    }

    private void UpdateActiveFlamethrower()
    {
        if (mainCamera == null || Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Vector2 direction =
            (Vector2)(mouseWorldPosition - transform.position);

        if (direction.sqrMagnitude < 0.001f)
            return;

        activeEffect.UpdateAim(
            transform.position,
            direction.normalized
        );
    }
}
