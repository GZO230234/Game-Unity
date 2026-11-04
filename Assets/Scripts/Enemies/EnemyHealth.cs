
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;

    [Header("Experiencia")]
    [SerializeField] private int xpReward = 2;

    private int currentHealth;
    private HealthBar healthBar;
    private float damageRemainder;

    private void Awake()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || currentHealth <= 0)
            return;

        // Acumulamos el daño decimal entre ataques.
        damageRemainder += damage;

        int wholeDamage = Mathf.FloorToInt(damageRemainder);
        damageRemainder -= wholeDamage;

        // Si todavía no se acumula un punto completo,
        // no reducimos la vida visible.
        if (wholeDamage <= 0)
            return;

        currentHealth -= wholeDamage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log(
            gameObject.name + " recibió " + wholeDamage +
            " de daño. Vida restante: " + currentHealth
        );

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");

        EnemyGroupMember groupMember =
            GetComponent<EnemyGroupMember>();

        if (groupMember != null)
        {
            groupMember.ReportDeath();
        }

        PlayerExperience experience =
            FindFirstObjectByType<PlayerExperience>();

        if (experience != null)
        {
            experience.AddXP(xpReward);
        }

        Destroy(gameObject);
    }
}
