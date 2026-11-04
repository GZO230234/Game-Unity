
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (fill == null || maxHealth <= 0) return;

        fill.fillAmount = Mathf.Clamp01(
            (float)currentHealth / maxHealth
        );
    }
}
