
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuracion del ataque")]
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float damage = 1f;

    [Header("Deteccion")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Efecto visual")]
    [SerializeField] private AreaWaveEffect wavePrefab;

    private float attackTimer;

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            Attack();
            attackTimer = 0f;
        }
    }

    private void Attack()
    {
        if (wavePrefab != null)
        {
            AreaWaveEffect wave = Instantiate(
                wavePrefab,
                transform.position,
                Quaternion.identity
            );

            wave.Play(attackRadius);
        }

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D enemyCollider in enemiesHit)
        {
            EnemyHealth enemyHealth =
                enemyCollider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        if (enemiesHit.Length > 0)
        {
            Debug.Log(
                "Ataque de area, objetivos: " + enemiesHit.Length
            );
        }
    }

    public void IncreaseDamage(float percentage)
    {
        damage *= 1f + percentage;

        Debug.Log("Daño aumentado. Daño actual: " + damage);
    }

    public void IncreaseAttackSpeed(float percentage)
    {
        attackInterval *= 1f - percentage;

        // Evita que el intervalo llegue a cero.
        attackInterval = Mathf.Max(0.2f, attackInterval);

        Debug.Log(
            "Intervalo de ataque actual: " + attackInterval
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRadius
        );
    }
}
