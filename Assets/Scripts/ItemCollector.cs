using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private ItemData currentNearbyItem; // Stocke l'objet proche que l'on peut ramasser

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            Debug.Log("ItemCollector prêt - Inventaire détecté.");
        }
        else
        {
            Debug.LogError("Aucun InventoryManager trouvé dans la scène !");
        }
    }

    void Update()
    {
        // Vérifie si la touche E est pressée et qu'un objet est proche
        if (Input.GetKeyDown(KeyCode.E) && currentNearbyItem != null)
        {
            Debug.Log("Tentative de ramassage d'objet...");
            PickupItem();
        }
        else if (Input.GetKeyDown(KeyCode.E) && currentNearbyItem == null)
        {
            Debug.LogWarning("Aucun objet à ramasser à proximité.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'objet proche est un "Item" et le stocke
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null)
            {
                currentNearbyItem = itemData;
                Debug.Log("Objet à proximité détecté : " + itemData.itemSO.itemName);
            }
            else
            {
                Debug.LogError("Aucun ItemData trouvé sur l'objet : " + other.name);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Réinitialise currentNearbyItem quand l'objet quitte la zone de détection
        if (other.CompareTag("Item") && currentNearbyItem != null)
        {
            Debug.Log("Objet hors de portée : " + currentNearbyItem.itemSO.itemName);
            currentNearbyItem = null;
        }
    }

    private void PickupItem()
    {
        if (currentNearbyItem != null)
        {
            // Ajoute l'item à l'inventaire et détruit l'objet ramassé
            Debug.Log("Ajout de l'objet " + currentNearbyItem.itemSO.itemName + " à l'inventaire.");
            inventoryManager.AddItem(currentNearbyItem.itemSO);
            Debug.Log("Objet ramassé : " + currentNearbyItem.itemSO.itemName);
            Destroy(currentNearbyItem.gameObject);
            currentNearbyItem = null; // Réinitialise après le ramassage
        }
        else
        {
            Debug.LogWarning("Impossible de ramasser l'objet, car aucun objet n'est à proximité.");
        }
    }
}
