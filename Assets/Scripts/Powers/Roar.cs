
using UnityEngine;
using UnityEngine.InputSystem;

public class RoarAbility : MonoBehaviour
{
    [Header("Configuración del rugido")]
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private float cooldown = 60f;

    [Header("Detección")]
    [SerializeField] private Camera mainCamera;

    [Header("Interfaz")]
    [SerializeField] private UnityEngine.UI.Image cooldownOverlay;
    [SerializeField] private TMPro.TMP_Text cooldownText;

    private float cooldownTimer;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        cooldownTimer = cooldown;
    }

    private void Update()
    {
        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }

        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            cooldownTimer >= cooldown)
        {
            UseRoar();
            cooldownTimer = 0f;
        }

        UpdateCooldownUI();
    }

    private void UseRoar()
    {
        if (mainCamera == null)
            return;

        Debug.Log("¡El dinosaurio ha rugido!");

        EnemyController[] normalEnemies =
            FindObjectsByType<EnemyController>(
                FindObjectsSortMode.None
            );

        foreach (EnemyController enemy in normalEnemies)
        {
            if (IsVisible(enemy.transform.position))
            {
                enemy.Stun(stunDuration);
            }
        }

        RangedEnemyController[] rangedEnemies =
            FindObjectsByType<RangedEnemyController>(
                FindObjectsSortMode.None
            );

        foreach (RangedEnemyController enemy in rangedEnemies)
        {
            if (IsVisible(enemy.transform.position))
            {
                enemy.Stun(stunDuration);
            }
        }
    }

    private bool IsVisible(Vector3 position)
    {
        Vector3 viewportPosition =
            mainCamera.WorldToViewportPoint(position);

        return viewportPosition.z > 0f &&
               viewportPosition.x >= 0f &&
               viewportPosition.x <= 1f &&
               viewportPosition.y >= 0f &&
               viewportPosition.y <= 1f;
    }

    private void UpdateCooldownUI()
{
    float remainingTime = Mathf.Max(
        0f,
        cooldown - cooldownTimer
    );

    bool isOnCooldown = remainingTime > 0f;

    if (cooldownOverlay != null)
    {
        cooldownOverlay.fillAmount =
            remainingTime / cooldown;

        cooldownOverlay.enabled = isOnCooldown;
    }

    if (cooldownText != null)
    {
        cooldownText.text = isOnCooldown
            ? Mathf.CeilToInt(remainingTime).ToString()
            : "";
    }
}
}
