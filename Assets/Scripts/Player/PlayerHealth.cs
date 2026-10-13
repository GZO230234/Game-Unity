using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Te han herido, vida restante: :" + currentHealth);

        if (currentHealth <=0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Moriste.");
        Destroy(gameObject);
    }
}