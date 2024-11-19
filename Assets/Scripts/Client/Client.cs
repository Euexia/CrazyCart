using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Client : MonoBehaviour
{
    public ItemSO demandedItem;
    public GameObject BubblePrefab;
    public GameObject PatienceBarPrefab;

    private GameObject bubbleInstance;
    private GameObject patienceBarInstance;

    private Image bubbleImage;
    private PatienceBar patienceBarScript;

    public InventoryManager inventoryManager;
    public List<ItemSO> availableItems;

    private Canvas bubbleCanvas;
    private ImprovementManager improvementManager;

    public float basePatience = 30f;
    private float patience;

    public event System.Action OnClientCompleted;
    public event System.Action OnDespawn; // Ajout de l'�v�nement OnDespawn

    public List<Transform> destinations;
    private NavMeshAgent agent;
    private int currentDestinationIndex = 0;

    private bool hasReachedDestination = false; // Flag pour savoir si le client est arriv� � destination

    void Awake()
    {
        if (!TryGetComponent(out agent))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }
    }

    void Start()
    {
        improvementManager = FindObjectOfType<ImprovementManager>();
        patience = basePatience + (improvementManager?.clientPatienceBonus ?? 0f);

        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        if (PatienceBarPrefab != null)
        {
            patienceBarInstance = Instantiate(PatienceBarPrefab, transform);
            patienceBarInstance.transform.localPosition = new Vector3(0, 1.5f, 0);
            patienceBarScript = patienceBarInstance.GetComponent<PatienceBar>();
            if (patienceBarScript != null)
            {
                patienceBarScript.UpdateBar((patience / basePatience) * 100);
            }
            else
            {
                Debug.LogError("Le script PatienceBar n'est pas attach� au prefab PatienceBarPrefab !");
            }
        }

        if (BubblePrefab != null)
        {
            bubbleInstance = Instantiate(BubblePrefab, transform);
            bubbleInstance.transform.localPosition = new Vector3(0, 3, 0);
            bubbleCanvas = bubbleInstance.GetComponentInChildren<Canvas>();
            bubbleCanvas.renderMode = RenderMode.WorldSpace;
            bubbleImage = bubbleInstance.GetComponentInChildren<Image>();
            bubbleInstance.SetActive(false);
        }

        if (availableItems.Count > 0)
        {
            GenerateRandomDemand(availableItems);
        }

        if (destinations != null && destinations.Count > 0)
        {
            ChooseRandomDestination();
        }
        else
        {
            Debug.LogWarning("Aucune destination assign�e !");
        }

        StartCoroutine(ClientTimer());
    }

    void Update()
    {
        if (bubbleInstance != null)
        {
            bubbleInstance.transform.position = transform.position + Vector3.up * 3f;
        }

        if (patienceBarInstance != null)
        {
            patienceBarInstance.transform.position = transform.position + Vector3.up * 1.5f;
        }

        if (patienceBarScript != null)
        {
            patienceBarScript.UpdateBar((patience / basePatience) * 100);
        }

        // V�rification si le client a atteint sa destination
        if (!hasReachedDestination && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            hasReachedDestination = true; // Le client est arriv�
            agent.isStopped = true; // Arr�ter le NavMeshAgent pour �viter de continuer � bouger l�g�rement
            agent.velocity = Vector3.zero; // Arr�ter compl�tement la vitesse
            agent.ResetPath(); // R�initialiser le path pour s'assurer qu'il ne bouge plus

            // D�sactivation compl�te du NavMeshAgent si vous ne voulez plus qu'il effectue des calculs de chemin
            agent.enabled = false;

            // Applique la rotation pour regarder la cam�ra
            LookAtCamera();
        }
    }

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

    private void UpdateThoughtBubble()
    {
        if (demandedItem != null && bubbleImage != null)
        {
            bubbleImage.sprite = demandedItem.icon;
            bubbleInstance.SetActive(true);
        }
    }

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

            OnClientCompleted?.Invoke();

            OnDespawn?.Invoke(); // D�clenchement de l'�v�nement OnDespawn
            Destroy(gameObject, 1f);
            return true;
        }

        return false;
    }

    private void ChooseRandomDestination()
    {
        if (destinations.Count == 0)
            return;

        currentDestinationIndex = Random.Range(0, destinations.Count);

        agent.SetDestination(destinations[currentDestinationIndex].position);
        hasReachedDestination = false; // R�initialiser la destination pour qu'il commence � se d�placer
    }

    private IEnumerator ClientTimer()
    {
        while (patience > 0)
        {
            patience -= Time.deltaTime;
            yield return null;
        }

        OnDespawn?.Invoke(); // D�clenchement de l'�v�nement OnDespawn lorsque la patience atteint 0
        Destroy(gameObject);
    }

    private void LookAtCamera()
    {
        Camera mainCamera = Camera.main; // Utiliser la cam�ra principale
        if (mainCamera != null)
        {
            // Faire en sorte que le client regarde la cam�ra
            Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, transform.position.y, mainCamera.transform.position.z);
            transform.LookAt(targetPosition);
        }
    }
}
