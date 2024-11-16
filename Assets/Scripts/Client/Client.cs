
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public ItemSO demandedItem; // L'objet demand� par le client
    public GameObject BubblePrefab; // Prefab pour afficher une bulle de pens�e
    public GameObject PatienceBarPrefab; // Prefab pour afficher la barre de patience

    private GameObject bubbleInstance;
    private GameObject patienceBarInstance;

    private Image bubbleImage;
    private PatienceBar patienceBarScript; // Script pour la gestion de la barre de progression

    public InventoryManager inventoryManager; // R�f�rence au gestionnaire d'inventaire
    public List<ItemSO> availableItems; // Liste des objets disponibles pour les demandes du client

    private Canvas bubbleCanvas;
    private ImprovementManager improvementManager;

    public float basePatience = 30f; // Temps de patience initial
    private float patience; // Temps restant de patience

    public event System.Action OnClientCompleted; // �v�nement d�clench� lorsque le client est satisfait

    void Start()
    {
        // Initialiser la patience en fonction des am�liorations
        improvementManager = FindObjectOfType<ImprovementManager>();
        patience = basePatience + (improvementManager?.clientPatienceBonus ?? 0f);

        // Initialiser l'inventaire si n�cessaire
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
                Debug.LogError("Le script ProgressBar n'est pas attach� au prefab PatienceBarPrefab !");
            }
        }

        // Instancier la bulle de pens�e
        if (BubblePrefab != null)
        {
            bubbleInstance = Instantiate(BubblePrefab, transform);
            bubbleInstance.transform.localPosition = new Vector3(0, 3, 0);

            bubbleCanvas = bubbleInstance.GetComponentInChildren<Canvas>();
            bubbleCanvas.renderMode = RenderMode.WorldSpace;

            bubbleImage = bubbleInstance.GetComponentInChildren<Image>();
            bubbleInstance.SetActive(false); // Masquer la bulle jusqu'� ce qu'une demande soit g�n�r�e
        }

        // G�n�rer une demande al�atoire
        if (availableItems.Count > 0)
        {
            GenerateRandomDemand(availableItems);
        }

        // Lancer le compte � rebours de patience
        StartCoroutine(ClientTimer());
    }

    void Update()
    {
        // Mettre � jour la position de la bulle et de la barre de patience
        if (bubbleInstance != null)
        {
            bubbleInstance.transform.position = transform.position + Vector3.up * 3f;
        }

        if (patienceBarInstance != null)
        {
            patienceBarInstance.transform.position = transform.position + Vector3.up * 2f;
        }

        // Mettre � jour la barre de progression
        if (patienceBarScript != null)
        {
            patienceBarScript.UpdateBar((patience / basePatience) * 100);
        }
    }

    /// <summary>
    /// G�n�re une demande al�atoire parmi les objets disponibles.
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
    /// Met � jour l'affichage de la bulle de pens�e avec l'objet demand�.
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
    /// V�rifie si le joueur poss�de l'objet demand�.
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

            OnClientCompleted?.Invoke(); // D�clenche l'�v�nement de satisfaction du client

            Destroy(gameObject, 1f); // D�truit le client apr�s un d�lai
            return true;
        }

        return false;
    }

    /// <summary>
    /// G�re le compte � rebours de patience.
    /// </summary>
    private IEnumerator ClientTimer()
    {
        while (patience > 0)
        {
            patience -= Time.deltaTime;
            yield return null;
        }

        // Si la patience atteint 0, d�truire le client
        Destroy(gameObject);
    }
}