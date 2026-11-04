
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClawAttackAbility : MonoBehaviour
{
    [Header("Configuración del ataque")]
    [SerializeField] private float attackInterval = 3.5f;
    [SerializeField] private int numberOfHits = 3;
    [SerializeField] private float damagePerHit = 0.5f;
    [SerializeField] private float hitDelay = 0.2f;

    [Header("Detección")]
    [SerializeField] private float attackDistance = 0.8f;
    [SerializeField] private Vector2 attackArea = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("Efecto visual")]
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Sprite[] slashSprites;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private float effectDuration = 0.18f;

    [SerializeField] private float slashSpacing = 0.25f;
    [SerializeField] private float slashRotation = 15f;
    [SerializeField] private float slashAppearDelay = 0.12f;

    private float attackTimer;
    private bool isAttacking;

    private void Start()
    {
        attackTimer = attackInterval;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval && !isAttacking)
        {
            attackTimer = 0f;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        for (int i = 0; i < numberOfHits; i++)
        {
            Vector2 direction = GetFacingDirection();

            ShowSlash(direction);
            DamageEnemies(direction);

            if (i < numberOfHits - 1)
            {
                yield return new WaitForSeconds(hitDelay);
            }
        }

        isAttacking = false;
    }

    private Vector2 GetFacingDirection()
    {
        if (playerSprite != null && playerSprite.flipX)
            return Vector2.left;

        return Vector2.right;
    }

    private void ShowSlash(Vector2 direction)
{
    StartCoroutine(ShowSlashCombo(direction));
}

private IEnumerator ShowSlashCombo(Vector2 direction)
    {
        if (slashPrefab == null || slashSprites == null)
            yield break;

        Vector2 basePosition = (Vector2)transform.position
            + direction * attackDistance;

        List<GameObject> activeSlashes = new List<GameObject>();

        for (int i = 0; i < slashSprites.Length; i++)
        {
            if (slashSprites[i] == null)
                continue;

            float verticalOffset =
                (i - (slashSprites.Length - 1) / 2f) * slashSpacing;

            Vector2 spawnPosition = basePosition
                + Vector2.up * verticalOffset;

            float angle = (i - 1) * slashRotation;

            if (direction.x < 0)
                angle = -angle;

            GameObject slash = Instantiate(
                slashPrefab,
                spawnPosition,
                Quaternion.Euler(0f, 0f, angle)
            );

            SpriteRenderer slashRenderer =
                slash.GetComponentInChildren<SpriteRenderer>();

            if (slashRenderer != null)
            {
                slashRenderer.sprite = slashSprites[i];
                slashRenderer.flipX = direction.x < 0;
            }

            activeSlashes.Add(slash);

            // Esperar antes de mostrar el siguiente zarpazo
            if (i < slashSprites.Length - 1)
            {
                yield return new WaitForSeconds(slashAppearDelay);
            }
        }

        // Esperar a que termine el combo
        yield return new WaitForSeconds(effectDuration);

        // Desaparecer todos juntos
        foreach (GameObject slash in activeSlashes)
        {
            if (slash != null)
                Destroy(slash);
        }
    }

    private void DamageEnemies(Vector2 direction)
    {
        Vector2 attackCenter = (Vector2)transform.position
            + direction * attackDistance;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackCenter,
            attackArea,
            0f,
            enemyLayer
        );

        // Evitar dañar dos veces al mismo enemigo
        // si tiene varios colliders.
        HashSet<EnemyHealth> damagedEnemies =
            new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(damagePerHit);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = Application.isPlaying
            ? GetFacingDirection()
            : Vector2.right;

        Vector2 center = (Vector2)transform.position
            + direction * attackDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, attackArea);
    }
}
