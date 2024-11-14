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
            Debug.Log("ItemCollector pr�t - Inventaire d�tect�.");
        }
        else
        {
            Debug.LogError("Aucun InventoryManager trouv� dans la sc�ne !");
        }
    }

    void Update()
    {
        // V�rifie si la touche E est press�e et qu'un objet est proche
        if (Input.GetKeyDown(KeyCode.E) && currentNearbyItem != null)
        {
            Debug.Log("Tentative de ramassage d'objet...");
            PickupItem();
        }
        else if (Input.GetKeyDown(KeyCode.E) && currentNearbyItem == null)
        {
            Debug.LogWarning("Aucun objet � ramasser � proximit�.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // V�rifie si l'objet proche est un "Item" et le stocke
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null)
            {
                currentNearbyItem = itemData;
                Debug.Log("Objet � proximit� d�tect� : " + itemData.itemSO.itemName);
            }
            else
            {
                Debug.LogError("Aucun ItemData trouv� sur l'objet : " + other.name);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // R�initialise currentNearbyItem quand l'objet quitte la zone de d�tection
        if (other.CompareTag("Item") && currentNearbyItem != null)
        {
            Debug.Log("Objet hors de port�e : " + currentNearbyItem.itemSO.itemName);
            currentNearbyItem = null;
        }
    }

    private void PickupItem()
    {
        if (currentNearbyItem != null)
        {
            // Ajoute l'item � l'inventaire et d�truit l'objet ramass�
            Debug.Log("Ajout de l'objet " + currentNearbyItem.itemSO.itemName + " � l'inventaire.");
            inventoryManager.AddItem(currentNearbyItem.itemSO);
            Debug.Log("Objet ramass� : " + currentNearbyItem.itemSO.itemName);
            Destroy(currentNearbyItem.gameObject);
            currentNearbyItem = null; // R�initialise apr�s le ramassage
        }
        else
        {
            Debug.LogWarning("Impossible de ramasser l'objet, car aucun objet n'est � proximit�.");
        }
    }
}
