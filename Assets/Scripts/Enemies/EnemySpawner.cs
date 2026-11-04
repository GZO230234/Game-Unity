
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private Transform player;

    [Header("Oleadas")]
    [SerializeField] private int initialNormalEnemies = 3;
    [SerializeField] private int initialRangedEnemies = 1;
    [SerializeField] private int killsToNextGroup = 3;

    [Header("Dificultad por tiempo")]
    [SerializeField] private float difficultyStepSeconds = 120f;
    [SerializeField] private int maxNormalEnemies = 10;
    [SerializeField] private int maxRangedEnemies = 3;

    [Header("Intervalo entre grupos")]
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float minimumSpawnInterval = 2f;
    [SerializeField] private float timeToMaximumDifficulty = 600f;

    [Header("Posiciones")]
    [SerializeField] private float groupRadius = 1.5f;
    [SerializeField] private float minDistanceFromPlayer = 8f;

    [SerializeField] private BoxCollider2D worldBounds;
    [SerializeField] private float spawnMargin = 1f;

    private float gameStartTime;
    private float lastGroupSpawnTime;
    private int currentGroupId;
    private int killsInCurrentGroup;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
        {
            Debug.LogError("EnemySpawner no encuentra al jugador.");
            enabled = false;
            return;
        }

        if (enemyPrefab == null || rangedEnemyPrefab == null)
        {
            Debug.LogError("Asigna ambos prefabs en EnemySpawner.");
            enabled = false;
            return;
        }

        gameStartTime = Time.time;

        SpawnNextGroup();
        StartCoroutine(ManageGroups());
    }

    private IEnumerator ManageGroups()
    {
        while (true)
        {
            // Esperar las eliminaciones necesarias.
            while (killsInCurrentGroup < killsToNextGroup)
                yield return null;

            // También respetar el intervalo mínimo entre grupos.
            while (Time.time - lastGroupSpawnTime < GetSpawnInterval())
                yield return null;

            SpawnNextGroup();
        }
    }

    private void SpawnNextGroup()
    {
        currentGroupId++;
        killsInCurrentGroup = 0;
        lastGroupSpawnTime = Time.time;

        float elapsed = Time.time - gameStartTime;

        int difficultyLevel = Mathf.FloorToInt(
            elapsed / Mathf.Max(1f, difficultyStepSeconds)
        );

        int normalCount = Mathf.Min(
            initialNormalEnemies + difficultyLevel,
            maxNormalEnemies
        );

        int rangedCount = Mathf.Min(
            initialRangedEnemies + Mathf.FloorToInt(difficultyLevel / 3f),
            maxRangedEnemies
        );

        Vector2 center = GetSpawnCenter();

        Debug.Log(
            $"Grupo {currentGroupId}: {normalCount} normales, " +
            $"{rangedCount} rangers. Tiempo: {elapsed:F0}s"
        );

        for (int i = 0; i < normalCount; i++)
            SpawnEnemy(enemyPrefab, center);

        for (int i = 0; i < rangedCount; i++)
            SpawnEnemy(rangedEnemyPrefab, center);
    }

    private void SpawnEnemy(GameObject prefab, Vector2 center)
    {
        Vector2 position =
            center + Random.insideUnitCircle * groupRadius;

        // Comprobar que la posición esté dentro del mapa.
        position = GetValidSpawnPosition(position);

        GameObject enemy = Instantiate(
            prefab,
            position,
            Quaternion.identity
        );

        // Registrar a qué grupo pertenece este enemigo.
        EnemyGroupMember member =
            enemy.AddComponent<EnemyGroupMember>();

        member.Initialize(this, currentGroupId);

        // Asignar el jugador al enemigo normal.
        EnemyController controller =
            enemy.GetComponent<EnemyController>();

        if (controller != null)
            controller.player = player;
    }

    public void RegisterEnemyKill(int groupId)
    {
        // Los enemigos supervivientes de grupos anteriores
        // no cuentan para activar el siguiente grupo.
        if (groupId != currentGroupId)
            return;

        killsInCurrentGroup++;

        Debug.Log(
            $"Eliminaciones del grupo actual: " +
            $"{killsInCurrentGroup}/{killsToNextGroup}"
        );
    }

    private float GetSpawnInterval()
    {
        float elapsed = Time.time - gameStartTime;

        float progress = Mathf.Clamp01(
            elapsed / Mathf.Max(1f, timeToMaximumDifficulty)
        );

        return Mathf.Lerp(
            initialSpawnInterval,
            minimumSpawnInterval,
            progress
        );
    }

    private Vector2 GetSpawnCenter()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        if (direction == Vector2.zero)
            direction = Vector2.right;

        return (Vector2)player.position
            + direction * minDistanceFromPlayer;
    }

    private Vector2 GetValidSpawnPosition(Vector2 position)
    {
        if (worldBounds == null)
        {
            Debug.LogError(
                "No se ha asignado World Bounds en EnemySpawner."
            );

            return position;
        }

        Bounds bounds = worldBounds.bounds;

        float marginX = Mathf.Min(spawnMargin, bounds.extents.x);
        float marginY = Mathf.Min(spawnMargin, bounds.extents.y);

        float minX = bounds.min.x + marginX;
        float maxX = bounds.max.x - marginX;

        float minY = bounds.min.y + marginY;
        float maxY = bounds.max.y - marginY;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        return position;
    }
}
