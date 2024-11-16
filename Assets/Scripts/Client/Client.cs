
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public ItemSO demandedItem; // L'objet demandé par le client
    public GameObject BubblePrefab; // Prefab pour afficher une bulle de pensée
    public GameObject PatienceBarPrefab; // Prefab pour afficher la barre de patience

    private GameObject bubbleInstance;
    private GameObject patienceBarInstance;

    private Image bubbleImage;
    private PatienceBar patienceBarScript; // Script pour la gestion de la barre de progression

    public InventoryManager inventoryManager; // Référence au gestionnaire d'inventaire
    public List<ItemSO> availableItems; // Liste des objets disponibles pour les demandes du client

    private Canvas bubbleCanvas;
    private ImprovementManager improvementManager;

    public float basePatience = 30f; // Temps de patience initial
    private float patience; // Temps restant de patience

    public event System.Action OnClientCompleted; // Événement déclenché lorsque le client est satisfait

    void Start()
    {
        // Initialiser la patience en fonction des améliorations
        improvementManager = FindObjectOfType<ImprovementManager>();
        patience = basePatience + (improvementManager?.clientPatienceBonus ?? 0f);

        // Initialiser l'inventaire si nécessaire
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        // Instancier la barre de patience
        if (PatienceBarPrefab != null)
        {
            patienceBarInstance = Instantiate(PatienceBarPrefab, transform);
            patienceBarInstance.transform.localPosition = new Vector3(0, 2, 0);

            patienceBarScript = patienceBarInstance.GetComponent<PatienceBar>();
            if (patienceBarScript != null)
            {
                patienceBarScript.UpdateBar((patience / basePatience) * 100);
            }
            else
            {
                Debug.LogError("Le script ProgressBar n'est pas attaché au prefab PatienceBarPrefab !");
            }
        }

        // Instancier la bulle de pensée
        if (BubblePrefab != null)
        {
            bubbleInstance = Instantiate(BubblePrefab, transform);
            bubbleInstance.transform.localPosition = new Vector3(0, 3, 0);

            bubbleCanvas = bubbleInstance.GetComponentInChildren<Canvas>();
            bubbleCanvas.renderMode = RenderMode.WorldSpace;

            bubbleImage = bubbleInstance.GetComponentInChildren<Image>();
            bubbleInstance.SetActive(false); // Masquer la bulle jusqu'à ce qu'une demande soit générée
        }

        // Générer une demande aléatoire
        if (availableItems.Count > 0)
        {
            GenerateRandomDemand(availableItems);
        }

        // Lancer le compte à rebours de patience
        StartCoroutine(ClientTimer());
    }

    void Update()
    {
        // Mettre à jour la position de la bulle et de la barre de patience
        if (bubbleInstance != null)
        {
            bubbleInstance.transform.position = transform.position + Vector3.up * 3f;
        }

        if (patienceBarInstance != null)
        {
            patienceBarInstance.transform.position = transform.position + Vector3.up * 2f;
        }

        // Mettre à jour la barre de progression
        if (patienceBarScript != null)
        {
            patienceBarScript.UpdateBar((patience / basePatience) * 100);
        }
    }

    /// <summary>
    /// Génère une demande aléatoire parmi les objets disponibles.
    /// </summary>
    public void GenerateRandomDemand(List<ItemSO> availableItems)
    {
        if (availableItems == null || availableItems.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, availableItems.Count);
        demandedItem = availableItems[randomIndex];

        UpdateThoughtBubble();
    }

    /// <summary>
    /// Met à jour l'affichage de la bulle de pensée avec l'objet demandé.
    /// </summary>
    private void UpdateThoughtBubble()
    {
        if (demandedItem != null && bubbleImage != null)
        {
            bubbleImage.sprite = demandedItem.icon;
            bubbleInstance.SetActive(true);
        }
    }

    /// <summary>
    /// Vérifie si le joueur possède l'objet demandé.
    /// </summary>
    public bool CheckIfPlayerHasItem()
    {
        if (inventoryManager == null || demandedItem == null)
        {
            return false;
        }

        if (inventoryManager.HasItem(demandedItem))
        {
            inventoryManager.RemoveItem(demandedItem);
            bubbleInstance.SetActive(false);

            OnClientCompleted?.Invoke(); // Déclenche l'événement de satisfaction du client

            Destroy(gameObject, 1f); // Détruit le client après un délai
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gère le compte à rebours de patience.
    /// </summary>
    private IEnumerator ClientTimer()
    {
        while (patience > 0)
        {
            patience -= Time.deltaTime;
            yield return null;
        }

        // Si la patience atteint 0, détruire le client
        Destroy(gameObject);
    }
}