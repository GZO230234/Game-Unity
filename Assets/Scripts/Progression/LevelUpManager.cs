
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    [Header("Ventana")]
    [SerializeField] private GameObject levelUpPanel;

    [Header("Carta izquierda")]
    [SerializeField] private Button leftButton;
    [SerializeField] private TMP_Text leftTitle;
    [SerializeField] private TMP_Text leftDescription;

    [Header("Carta derecha")]
    [SerializeField] private Button rightButton;
    [SerializeField] private TMP_Text rightTitle;
    [SerializeField] private TMP_Text rightDescription;

    [Header("Referencias del jugador")]
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerMovement playerMovement;

    private int pendingLevelUps;
    private bool isChoosing;

    private enum UpgradeType
    {
        Damage,
        MaxHealth,
        Regeneration,
        MovementSpeed,
        AttackSpeed
    }

    private readonly string[] upgradeNames =
    {
        "Más daño",
        "Más vida máxima",
        "Regeneración",
        "Más velocidad",
        "Ataques más rápidos"
    };

    private readonly string[] upgradeDescriptions =
    {
        "Aumenta el daño de tus ataques un 25%.",
        "Aumenta tu vida máxima un 20%.",
        "Recupera 1 punto de vida cada 3 segundos.",
        "Aumenta tu velocidad de movimiento un 15%.",
        "Reduce el intervalo entre ataques un 15%."
    };

    private UpgradeType leftUpgrade;
    private UpgradeType rightUpgrade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (leftButton != null)
            leftButton.onClick.AddListener(() => ChooseUpgrade(leftUpgrade));

        if (rightButton != null)
            rightButton.onClick.AddListener(() => ChooseUpgrade(rightUpgrade));
    }

    private void Start()
    {
        if (playerAttack == null)
            playerAttack = FindFirstObjectByType<PlayerAttack>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    public void QueueLevelUp()
    {
        pendingLevelUps++;

        if (!isChoosing)
            ShowChoices();
    }

    private void ShowChoices()
    {
        if (pendingLevelUps <= 0)
            return;

        if (levelUpPanel == null ||
            leftButton == null || rightButton == null ||
            leftTitle == null || rightTitle == null ||
            leftDescription == null || rightDescription == null)
        {
            Debug.LogError("Faltan referencias en LevelUpManager.");
            return;
        }

        isChoosing = true;
        Time.timeScale = 0f;

        levelUpPanel.SetActive(true);

        leftUpgrade = (UpgradeType)Random.Range(0, 5);

        do
        {
            rightUpgrade = (UpgradeType)Random.Range(0, 5);
        }
        while (rightUpgrade == leftUpgrade);

        leftTitle.text = upgradeNames[(int)leftUpgrade];
        leftDescription.text = upgradeDescriptions[(int)leftUpgrade];

        rightTitle.text = upgradeNames[(int)rightUpgrade];
        rightDescription.text = upgradeDescriptions[(int)rightUpgrade];

        leftButton.interactable = true;
        rightButton.interactable = true;
    }

    private void ChooseUpgrade(UpgradeType chosenUpgrade)
    {
        Debug.Log("Mejora elegida: " + upgradeNames[(int)chosenUpgrade]);

        ApplyUpgrade(chosenUpgrade);

        pendingLevelUps--;

        if (pendingLevelUps > 0)
        {
            ShowChoices();
        }
        else
        {
            levelUpPanel.SetActive(false);
            isChoosing = false;
            Time.timeScale = 1f;
        }
    }

    private void ApplyUpgrade(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.Damage:
                if (playerAttack != null)
                    playerAttack.IncreaseDamage(0.25f);
                else
                    Debug.LogError("No se encontro PlayerAttack.");
                break;

            case UpgradeType.MaxHealth:
                if (playerHealth != null)
                    playerHealth.IncreaseMaxHealth(0.20f);
                else
                    Debug.LogError("No se encontro PlayerHealth.");
                break;

            case UpgradeType.Regeneration:
                if (playerHealth != null)
                    playerHealth.IncreaseRegeneration(1f);
                else
                    Debug.LogError("No se encontro PlayerHealth.");
                break;

            case UpgradeType.MovementSpeed:
                if (playerMovement != null)
                    playerMovement.IncreaseMoveSpeed(0.15f);
                else
                    Debug.LogError("No se encontro PlayerMovement.");
                break;

            case UpgradeType.AttackSpeed:
                if (playerAttack != null)
                    playerAttack.IncreaseAttackSpeed(0.15f);
                else
                    Debug.LogError("No se encontro PlayerAttack.");
                break;
        }
    }
}
