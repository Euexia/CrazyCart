
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public ItemSO demandedItem;
    public GameObject BubblePrefab;
    private GameObject BubbleInstance;
    private Image BubbleImage;
    public InventoryManager inventoryManager;
    public List<ItemSO> availableItems;

    private Canvas bubbleCanvas;

    public float basePatience = 30f;
    private float patience;
    private ImprovementManager improvementManager;

    public float timer = 30f;

    public event System.Action OnClientCompleted;

    // Start is called before the first frame update
    void Start()
    {
        improvementManager = FindObjectOfType<ImprovementManager>();
        patience = basePatience + (improvementManager?.clientPatienceBonus ?? 0f);
        StartCoroutine(ClientTimer());

        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        if (inventoryManager == null)
        {
            return;
        }

        BubbleInstance = Instantiate(BubblePrefab, transform);
        BubbleInstance.transform.localPosition = new Vector3(0, 2, 0);

        bubbleCanvas = BubbleInstance.GetComponentInChildren<Canvas>();
        bubbleCanvas.renderMode = RenderMode.WorldSpace;
        bubbleCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);

        BubbleImage = BubbleInstance.GetComponentInChildren<Image>();
        BubbleInstance.SetActive(false);

        if (availableItems.Count > 0)
        {
            GenerateRandomDemand(availableItems);
        }

        StartCoroutine(ClientTimer());

    }

    // Update is called once per frame
    void Update()
    {
        if (BubbleInstance != null)
        {
            Vector3 positionAboveHead = transform.position + Vector3.up * 1f;
            BubbleInstance.transform.position = positionAboveHead;
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
        if (demandedItem != null && BubbleImage != null)
        {
            BubbleImage.sprite = demandedItem.icon;
            BubbleInstance.SetActive(true);
        }
    }

    public bool CheckIfPlayerHasItem()
    {
        if (inventoryManager == null)
        {
            return false;
        }

        if (demandedItem == null)
        {
            return false;
        }

        bool hasItem = inventoryManager.HasItem(demandedItem);
        if (hasItem)
        {
            inventoryManager.RemoveItem(demandedItem);
            BubbleInstance.SetActive(false);

            OnClientCompleted?.Invoke();

            Destroy(gameObject, 1f);
        }

        return hasItem;
    }

    private IEnumerator ClientTimer()
    {
        yield return new WaitForSeconds(patience);
        Destroy(gameObject);

    }
}