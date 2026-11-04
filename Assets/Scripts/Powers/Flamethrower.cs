
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlamethrowerEffect : MonoBehaviour
{
    private float range;
    private float coneAngle;
    private float duration;
    private float damage;
    private float damageInterval;
    private LayerMask enemyLayer;

    private Vector2 origin;
    private Vector2 direction;

    public void Initialize(
        Vector2 startPosition,
        Vector2 shootDirection,
        LayerMask layer,
        float attackRange,
        float angle,
        float attackDuration,
        float damageAmount,
        float interval)
    {
        UpdateAim(startPosition, shootDirection);
        enemyLayer = layer;
        range = attackRange;
        coneAngle = angle;
        duration = attackDuration;
        damage = damageAmount;
        damageInterval = interval;

        StartCoroutine(BurnEnemies());
    }

    private IEnumerator BurnEnemies()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            DamageEnemies();

            yield return new WaitForSeconds(damageInterval);
            elapsedTime += damageInterval;
        }

        Destroy(gameObject);
    }

    private void DamageEnemies()
    {
        Vector2 center = origin + direction * (range / 2f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            range / 2f,
            enemyLayer
        );

        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemy == null || !damagedEnemies.Add(enemy))
                continue;

            Vector2 toEnemy =
                (Vector2)enemy.transform.position - origin;

            float distance = toEnemy.magnitude;

            if (distance > range)
                continue;

            float angle = Vector2.Angle(direction, toEnemy);

            if (angle <= coneAngle / 2f)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void UpdateAim(Vector2 playerPosition, Vector2 shootDirection)
    {
        origin = playerPosition;
        direction = shootDirection.normalized;

        // Mantener el fuego delante del jugador.
        transform.position = origin + direction * (range / 2f);

        // El sprite original apunta hacia la izquierda.
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg + 180f;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnDrawGizmosSelected()
{
    if (range <= 0f)
        return;

    Gizmos.color = Color.yellow;

    // Dibujar los límites del cono.
    Vector2 leftDirection = Quaternion.Euler(
        0f, 0f, -coneAngle / 2f
    ) * direction;

    Vector2 rightDirection = Quaternion.Euler(
        0f, 0f, coneAngle / 2f
    ) * direction;

    Vector3 leftPoint = origin + leftDirection * range;
    Vector3 rightPoint = origin + rightDirection * range;

    Gizmos.DrawLine(origin, leftPoint);
    Gizmos.DrawLine(origin, rightPoint);

    // Dibujar el arco del cono.
    Vector2 previousPoint = leftPoint;

    int segments = 30;

    for (int i = 1; i <= segments; i++)
    {
        float angle = Mathf.Lerp(
            -coneAngle / 2f,
            coneAngle / 2f,
            i / (float)segments
        );

        Vector2 currentDirection = Quaternion.Euler(
            0f, 0f, angle
        ) * direction;

        Vector2 currentPoint =
            origin + currentDirection * range;

        Gizmos.DrawLine(previousPoint, currentPoint);

        previousPoint = currentPoint;
    }

    // Marcar el origen del lanzallamas.
    Gizmos.DrawWireSphere(origin, 0.1f);
}
}
