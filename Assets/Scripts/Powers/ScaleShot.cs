
using UnityEngine;

public class ScaleShotAbility : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject scalePrefab;
    [SerializeField] private int projectileCount = 12;
    [SerializeField] private float shootInterval = 3.5f;
    [SerializeField] private float spawnOffset = 0.5f;

    private float shootTimer;

    private void Start()
    {
        shootTimer = shootInterval;
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            ShootScales();
            shootTimer = 0f;
        }
    }

    private void ShootScales()
    {
        if (scalePrefab == null)
            return;

        float angleStep = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * angleStep;

            float radians = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(
                Mathf.Cos(radians),
                Mathf.Sin(radians)
            );

            Vector2 spawnPosition =
                (Vector2)transform.position +
                direction * spawnOffset;

            GameObject projectile = Instantiate(
                scalePrefab,
                spawnPosition,
                Quaternion.identity
            );

            ScaleProjectile scale =
                projectile.GetComponent<ScaleProjectile>();

            if (scale != null)
            {
                scale.Launch(direction);
            }
        }
    }
}
