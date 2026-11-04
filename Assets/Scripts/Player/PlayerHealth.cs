
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 10;
    private int currentHealth;
    private bool isDead = false;

    [Header("Regeneracion")]
    [SerializeField] private float regenerationAmount = 0f;
    [SerializeField] private float regenerationInterval = 3f;

    private float regenerationTimer;

    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        UpdateHealthBar();
    }

    private void Update()
    {
        if (isDead || regenerationAmount <= 0f)
            return;

        regenerationTimer += Time.deltaTime;

        if (regenerationTimer >= regenerationInterval)
        {
            RegenerateHealth();
            regenerationTimer = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Te han herido, vida restante: " + currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void IncreaseMaxHealth(float percentage)
    {
        int previousMaxHealth = maxHealth;

        maxHealth = Mathf.CeilToInt(maxHealth * (1f + percentage));

        int healthIncrease = maxHealth - previousMaxHealth;

        // También recupera la cantidad de vida máxima añadida.
        currentHealth = Mathf.Min(currentHealth + healthIncrease, maxHealth);

        UpdateHealthBar();

        Debug.Log("Vida maxima aumentada: " + maxHealth);
    }

    public void IncreaseRegeneration(float amount)
    {
        regenerationAmount += amount;

        Debug.Log("Regeneracion actual: " + regenerationAmount +
                  " puntos cada " + regenerationInterval + " segundos.");
    }

    private void RegenerateHealth()
    {
        if (currentHealth >= maxHealth)
            return;

        int amount = Mathf.CeilToInt(regenerationAmount);

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        UpdateHealthBar();

        Debug.Log("Vida recuperada: " + currentHealth + "/" + maxHealth);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Moriste.");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("No se encontro el GameManager.");
        }

        Destroy(gameObject);
    }
}
