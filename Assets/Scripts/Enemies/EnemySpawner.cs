
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuracion de enemigos")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemiesPerGroup = 3;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float groupRadius = 1.5f;

    [Header("Distancia respecto al jugador")]
    [SerializeField] private Transform player;
    [SerializeField] private float minDistanceFromPlayer = 8f;

    private void Start()
    {
        StartCoroutine(SpawnGroups());
    }

    private IEnumerator SpawnGroups()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null || enemyPrefab == null)
                continue;

            Vector2 center = GetSpawnCenter();

            for (int i = 0; i < enemiesPerGroup; i++)
            {
                Vector2 offset = Random.insideUnitCircle * groupRadius;
                Vector2 spawnPosition = center + offset;

                GameObject enemy = Instantiate(
                    enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

                EnemyController controller =
                    enemy.GetComponent<EnemyController>();

                if (controller != null)
                {
                    controller.player = player;
                }
            }
        }
    }

    private Vector2 GetSpawnCenter()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        if (direction == Vector2.zero)
            direction = Vector2.right;

        return (Vector2)player.position
            + direction * minDistanceFromPlayer;
    }
}
