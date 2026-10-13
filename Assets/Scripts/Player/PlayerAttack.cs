using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuracion del ataque")]
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private int damage = 1;

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
            Debug.Log("Ataque de area, objetivos: " + 
                        enemiesHit.Length);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position, 
            attackRadius);
    }
}