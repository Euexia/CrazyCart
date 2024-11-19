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

    public List<Transform> destinations;  
    private NavMeshAgent agent;
    private int currentDestinationIndex = 0;

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
                Debug.LogError("Le script PatienceBar n'est pas attaché au prefab PatienceBarPrefab !");
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
            Debug.LogWarning("Aucune destination assignée !");
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

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            ChooseRandomDestination(); 
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
    }

    private IEnumerator ClientTimer()
    {
        while (patience > 0)
        {
            patience -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
