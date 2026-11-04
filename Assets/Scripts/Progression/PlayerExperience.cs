
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    [Header("Nivel y experiencia")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int currentXP = 0;

    [Header("Barra de experiencia")]
    [SerializeField] private Slider xpBar;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;

    private int xpToNextLevel;

    private void Start()
    {
        xpToNextLevel = GetXPRequiredForNextLevel(currentLevel);
        UpdateXPBar();
    }

    public void AddXP(int amount)
    {
        if (amount <= 0)
            return;

        currentXP += amount;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;

            xpToNextLevel =
                GetXPRequiredForNextLevel(currentLevel);

            Debug.Log("¡Subiste al nivel " + currentLevel + "!");

            if (LevelUpManager.Instance != null)
            {
                LevelUpManager.Instance.QueueLevelUp();
            }
        }

        UpdateXPBar();

        Debug.Log(
            "XP: " + currentXP + "/" + xpToNextLevel
        );

        Debug.Log("¡Subiste al nivel " + currentLevel + "!");
    }

    private int GetXPRequiredForNextLevel(int level)
    {
        switch (level)
        {
            case 1: return 10;
            case 2: return 15;
            case 3: return 22;
            case 4: return 30;
            case 5: return 40;
            case 6: return 52;
            case 7: return 66;
            case 8: return 82;
            case 9: return 100;
            default:
                return Mathf.RoundToInt(
                    100f * Mathf.Pow(1.2f, level - 9)
                );
        }
    }

    private void UpdateXPBar()
    {
        if (xpBar == null)
            return;

        xpBar.minValue = 0;
        xpBar.maxValue = xpToNextLevel;
        xpBar.value = currentXP;
    }
}
