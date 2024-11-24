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
    public List<Sprite> SpritePrefabs;
    public List<Sprite> HappyEmotes; // Liste des émojis heureux

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
    public event System.Action OnDespawn;

    public List<Transform> destinations;
    private NavMeshAgent agent;
    private int currentDestinationIndex = 0;

    private bool hasReachedDestination = false;

    private GameManager gameManager;
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private GameObject spriteCanvasInstance;
    private Image spriteImage; 
    public AudioClip spawnSound;

    void Awake()
    {
        if (!TryGetComponent(out agent))
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        agent.avoidancePriority = Random.Range(0, 100);

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        CreateSpriteCanvas();
    }

    private void CreateSpriteCanvas()
    {
        GameObject canvasObject = new GameObject("SpriteCanvas");
        canvasObject.transform.SetParent(transform);
        canvasObject.transform.localPosition = new Vector3(0, 1f, 0); 

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rectTransform = canvasObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);  

        // Ajoute un composant Image
        spriteImage = canvasObject.AddComponent<Image>();
        spriteImage.enabled = false; 
    }

    private void SpawnSpriteOnCanvas(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Aucun sprite fourni pour SpawnSpriteOnCanvas !");
            return;
        }

        if (spriteImage == null)
        {
            Debug.LogError("Le composant Image n'est pas configuré !");
            return;
        }

        spriteImage.sprite = sprite; 
        spriteImage.enabled = true; 
    }

    private void HideSpriteOnCanvas()
    {
        if (spriteImage != null)
        {
            spriteImage.enabled = false;
        }
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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

        StartCoroutine(ClientTimer());
    }

    void Update()
    {
        if (bubbleInstance != null)
        {
            bubbleInstance.transform.position = transform.position + Vector3.up * 1.8f;
        }

        if (patienceBarInstance != null)
        {
            patienceBarInstance.transform.position = transform.position + Vector3.up * 1.2f;
        }

        if (patienceBarScript != null)
        {
            patienceBarScript.UpdateBar((patience / basePatience) * 100);
        }

        HandleAnimations();
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

            ShowHappyEmote();

            FindObjectOfType<GameManager>().globalTotalClients++;
            gameManager.ClientSatisfied(); 

            GoToSpawnPoint();

            StartCoroutine(WaitUntilAtSpawnPoint());

            return true;
        }

        return false;
    }


    private IEnumerator WaitUntilAtSpawnPoint()
    {
        while (Vector3.Distance(transform.position, agent.destination) > 0.5f)
        {
            yield return null;
        }

        OnDespawn?.Invoke();
        Destroy(gameObject);
    }

    private void ShowHappyEmote()
    {
        if (patienceBarInstance != null)
        {
            patienceBarInstance.SetActive(false);
        }

        if (HappyEmotes != null && HappyEmotes.Count > 0)
        {
            int randomIndex = Random.Range(0, HappyEmotes.Count);
            Sprite happyEmote = HappyEmotes[randomIndex];
            SpawnSpriteOnCanvas(happyEmote);
        }
    }


    private void ChooseRandomDestination()
    {
        if (destinations.Count == 0)
            return;

        currentDestinationIndex = Random.Range(0, destinations.Count);
        agent.SetDestination(destinations[currentDestinationIndex].position);
        hasReachedDestination = false;
    }

    private void GoToSpawnPoint()
    {
        if (gameManager != null && gameManager.spawnPoints.Count > 0)
        {
            Transform spawnPoint = gameManager.spawnPoints[Random.Range(0, gameManager.spawnPoints.Count)];

            agent.SetDestination(spawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Aucun point de spawn défini !");
        }
    }


    private IEnumerator ClientTimer()
    {
        while (patience > 0)
        {
            patience -= Time.deltaTime;
            yield return null;
        }

        if (patience <= 0)
        {
            gameManager.SetClientLostByImpatience(true);

            if (bubbleInstance != null)
            {
                bubbleInstance.SetActive(false);
            }

            if (patienceBarInstance != null)
            {
                patienceBarInstance.SetActive(false);
            }

            SpawnRandomSprite();

            GoToSpawnPoint();

            while (Vector3.Distance(transform.position, agent.destination) > 0.5f)
            {
                yield return null;
            }

            OnDespawn?.Invoke();

            Destroy(gameObject);
        }
    }

    private void SpawnRandomSprite()
    {
        if (SpritePrefabs == null || SpritePrefabs.Count == 0)
        {
            Debug.LogWarning("Aucun sprite dans SpritePrefabs. Vérifiez l'assignation dans l'inspecteur !");
            return;
        }

        int randomIndex = Random.Range(0, SpritePrefabs.Count);
        Sprite randomSprite = SpritePrefabs[randomIndex];

        if (spriteImage != null)
        {
            SpawnSpriteOnCanvas(randomSprite);
        }
        else
        {
            Debug.LogError("SpriteImage non configuré correctement !");
        }
    }

    private void HandleAnimations()
    {
        if (animator != null)
        {
            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
    public bool IsSatisfied()
    {
        return inventoryManager.HasItem(demandedItem);  
    }
 

}
