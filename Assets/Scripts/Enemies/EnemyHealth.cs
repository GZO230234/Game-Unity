using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <=0)
            return;

        currentHealth -= damage;

        Debug.Log(
            gameObject.name + " recibio " + damage +
            " de dano. vida restante: " + currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        Destroy(gameObject);
    }
}