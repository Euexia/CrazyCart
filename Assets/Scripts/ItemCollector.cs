using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private ItemData currentNearbyItem; 

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
            Debug.Log("Ajout de l'objet " + currentNearbyItem.itemSO.itemName + " à l'inventaire.");
            inventoryManager.AddItem(currentNearbyItem.itemSO);
            Debug.Log("Objet ramassé : " + currentNearbyItem.itemSO.itemName);
            Destroy(currentNearbyItem.gameObject);
            currentNearbyItem = null; 
        }
        else
        {
            Debug.LogWarning("Impossible de ramasser l'objet, car aucun objet n'est à proximité.");
        }
    }
}
