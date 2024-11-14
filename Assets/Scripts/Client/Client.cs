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

    private float timer = 30f;

    void Start()
    {
        // Assurez-vous que inventoryManager est assigné
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager n'a pas été trouvé dans la scène.");
            return;
        }

        // Instancie la bulle de pensée et la positionne au-dessus de la tête du client
        BubbleInstance = Instantiate(BubblePrefab, transform);
        BubbleInstance.transform.localPosition = new Vector3(0, 2, 0);

        // Assure que le prefab de la bulle utilise un Canvas en World Space
        bubbleCanvas = BubbleInstance.GetComponentInChildren<Canvas>();
        bubbleCanvas.renderMode = RenderMode.WorldSpace;

        // Ajuste la taille du Canvas si nécessaire
        bubbleCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);

        // Récupère le composant Image pour afficher l'icône
        BubbleImage = BubbleInstance.GetComponentInChildren<Image>();
        BubbleInstance.SetActive(false);

        if (availableItems.Count > 0)
        {
            GenerateRandomDemand(availableItems);
        }
        else
        {
            Debug.LogWarning("Aucun article disponible dans la liste.");
        }

        // Lance la coroutine pour faire disparaître le client après un délai
        StartCoroutine(ClientTimer());
    }


    void Update()
    {
        // Maintient la bulle au-dessus de la tête du client
        if (BubbleInstance != null)
        {
            Vector3 positionAboveHead = transform.position + Vector3.up * 1f;
            positionAboveHead.x = transform.position.x;

            BubbleInstance.transform.position = positionAboveHead;
        }
    }

    public void GenerateRandomDemand(List<ItemSO> availableItems)
    {
        if (availableItems == null || availableItems.Count == 0)
        {
            Debug.LogWarning("Aucun article disponible pour la demande !");
            return;
        }

        int randomIndex = Random.Range(0, availableItems.Count);
        demandedItem = availableItems[randomIndex];
        Debug.Log("Le client demande : " + demandedItem.itemName);

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
        // Vérifier si l'inventaire est initialisé
        if (inventoryManager == null)
        {
            Debug.LogError("L'inventaire n'est pas initialisé. Assurez-vous que InventoryManager est assigné.");
            return false;
        }

        // Vérifier si demandedItem est bien affecté
        if (demandedItem == null)
        {
            Debug.LogWarning("Aucune demande spécifiée pour le client.");
            return false;
        }

        bool hasItem = inventoryManager.HasItem(demandedItem);
        if (hasItem)
        {
            Debug.Log("Le joueur possède l'article demandé par le client : " + demandedItem.itemName);

            inventoryManager.RemoveItem(demandedItem);
            BubbleInstance.SetActive(false);

            // Détruire l'objet client après la commande
            Destroy(gameObject, 1f);
        }
        else
        {
            Debug.Log("Le joueur ne possède pas l'article demandé par le client.");
        }

        return hasItem;
    }


    private IEnumerator ClientTimer()
    {
        // Attends le temps défini (par exemple, 10 secondes) avant de supprimer le client
        yield return new WaitForSeconds(timer);

        // Affiche un message ou effectue une action juste avant que le client ne disparaisse, si besoin
        Debug.Log("Le client a disparu après " + timer + " secondes.");

        // Détruire l'objet client
        Destroy(gameObject);
    }
}
