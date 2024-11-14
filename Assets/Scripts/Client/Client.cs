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
        // Assurez-vous que inventoryManager est assign�
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager n'a pas �t� trouv� dans la sc�ne.");
            return;
        }

        // Instancie la bulle de pens�e et la positionne au-dessus de la t�te du client
        BubbleInstance = Instantiate(BubblePrefab, transform);
        BubbleInstance.transform.localPosition = new Vector3(0, 2, 0);

        // Assure que le prefab de la bulle utilise un Canvas en World Space
        bubbleCanvas = BubbleInstance.GetComponentInChildren<Canvas>();
        bubbleCanvas.renderMode = RenderMode.WorldSpace;

        // Ajuste la taille du Canvas si n�cessaire
        bubbleCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 100);

        // R�cup�re le composant Image pour afficher l'ic�ne
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

        // Lance la coroutine pour faire dispara�tre le client apr�s un d�lai
        StartCoroutine(ClientTimer());
    }


    void Update()
    {
        // Maintient la bulle au-dessus de la t�te du client
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
        // V�rifier si l'inventaire est initialis�
        if (inventoryManager == null)
        {
            Debug.LogError("L'inventaire n'est pas initialis�. Assurez-vous que InventoryManager est assign�.");
            return false;
        }

        // V�rifier si demandedItem est bien affect�
        if (demandedItem == null)
        {
            Debug.LogWarning("Aucune demande sp�cifi�e pour le client.");
            return false;
        }

        bool hasItem = inventoryManager.HasItem(demandedItem);
        if (hasItem)
        {
            Debug.Log("Le joueur poss�de l'article demand� par le client : " + demandedItem.itemName);

            inventoryManager.RemoveItem(demandedItem);
            BubbleInstance.SetActive(false);

            // D�truire l'objet client apr�s la commande
            Destroy(gameObject, 1f);
        }
        else
        {
            Debug.Log("Le joueur ne poss�de pas l'article demand� par le client.");
        }

        return hasItem;
    }


    private IEnumerator ClientTimer()
    {
        // Attends le temps d�fini (par exemple, 10 secondes) avant de supprimer le client
        yield return new WaitForSeconds(timer);

        // Affiche un message ou effectue une action juste avant que le client ne disparaisse, si besoin
        Debug.Log("Le client a disparu apr�s " + timer + " secondes.");

        // D�truire l'objet client
        Destroy(gameObject);
    }
}
