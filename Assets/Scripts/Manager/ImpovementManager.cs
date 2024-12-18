using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ImprovementManager : MonoBehaviour
{
    public static ImprovementManager Instance { get; private set; }

    public int improvementPoints = 0;
    public float moveSpeedBonus = 0f;
    public int inventoryCapacityBonus = 0;
    public float clientPatienceBonus = 0f;

    public TMP_Text improvementPointsText;
    public TMP_Text moveSpeedPointsText;
    public TMP_Text inventoryPointsText;
    public TMP_Text patiencePointsText;

    public Button upgradeMoveSpeedButton;
    public Button upgradeInventoryButton;
    public Button upgradePatienceButton;

    private int moveSpeedUpgradePoints = 0;
    private int inventoryUpgradePoints = 0;
    private int patienceUpgradePoints = 0;

    public GameObject particleEffectPrefab;
    public GameObject player;

    public GameObject improvementMenuCanvas; // Canvas d'amélioration

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        upgradeMoveSpeedButton.onClick.AddListener(UpgradeMoveSpeed);
        upgradeInventoryButton.onClick.AddListener(UpgradeInventoryCapacity);
        upgradePatienceButton.onClick.AddListener(UpgradeClientPatience);
        UpdateImprovementPointsText();
    }

    // Fonction pour afficher le menu d'amélioration et mettre le jeu en pause
    public void OpenImprovementMenu()
    {
        improvementMenuCanvas.SetActive(true); // Affiche le canvas d'amélioration
        Time.timeScale = 0f; // Met le jeu en pause
    }

    // Fonction pour fermer le menu d'amélioration et reprendre le jeu
    public void CloseImprovementMenu()
    {
        improvementMenuCanvas.SetActive(false); // Cache le canvas d'amélioration
        Time.timeScale = 1f; // Relance le jeu
    }

    public void ImproveInventoryCapacity()
    {
        inventoryCapacityBonus++;
        Debug.Log("Capacité d'inventaire améliorée !");
    }

    private void UpdateImprovementPointsText()
    {
        if (improvementPointsText != null)
        {
            improvementPointsText.text = "Points d'amélioration : " + improvementPoints.ToString();
        }

        if (moveSpeedPointsText != null)
            moveSpeedPointsText.text = "Points affectés : " + moveSpeedUpgradePoints;

        if (inventoryPointsText != null)
            inventoryPointsText.text = "Points affectés : " + inventoryUpgradePoints;

        if (patiencePointsText != null)
            patiencePointsText.text = "Points affectés : " + patienceUpgradePoints;
    }

    public void AddImprovementPoints(int points)
    {
        improvementPoints += points;
        UpdateImprovementPointsText();
    }

    public void UpgradeMoveSpeed()
    {
        if (UpgradeMoveSpeed(0.5f))
        {
            moveSpeedUpgradePoints++;
            UpdateImprovementPointsText();
            PlayParticleEffect();
        }
    }

    public void UpgradeInventoryCapacity()
    {
        if (UpgradeInventoryCapacity(1))
        {
            inventoryUpgradePoints++;
            UpdateImprovementPointsText();
            PlayParticleEffect();
        }
    }

    public void UpgradeClientPatience()
    {
        if (UpgradeClientPatience(5f))
        {
            patienceUpgradePoints++;
            UpdateImprovementPointsText();
            PlayParticleEffect();
        }
    }

    public bool UpgradeMoveSpeed(float amount)
    {
        if (improvementPoints > 0)
        {
            moveSpeedBonus += amount;
            improvementPoints--;
            UpdateImprovementPointsText();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UpgradeInventoryCapacity(int amount)
    {
        if (improvementPoints > 0)
        {
            inventoryCapacityBonus += amount;
            Debug.Log($"Capacité d'inventaire améliorée de {amount}. Bonus actuel : {inventoryCapacityBonus}");
            improvementPoints--;
            UpdateImprovementPointsText();
            return true;
        }
        else
        {
            Debug.Log("Pas assez de points d'amélioration pour augmenter la capacité.");
            return false;
        }
    }

    public bool UpgradeClientPatience(float amount)
    {
        if (improvementPoints > 0)
        {
            clientPatienceBonus += amount;
            improvementPoints--;
            UpdateImprovementPointsText();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PlayParticleEffect()
    {
        if (particleEffectPrefab != null && player != null)
        {
            GameObject particleEffect = Instantiate(particleEffectPrefab, player.transform.position, Quaternion.identity);
            particleEffect.transform.localScale *= 3f;
            particleEffect.transform.SetParent(player.transform);
            Destroy(particleEffect, 2f);
        }
        else
        {
            Debug.LogWarning("Le prefab de particules ou le joueur n'est pas assigné.");
        }
    }
}
