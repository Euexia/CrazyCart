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
            Debug.Log("ItemCollector pr�t - Inventaire d�tect�.");
        }
        else
        {
            Debug.LogError("Aucun InventoryManager trouv� dans la sc�ne !");
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
            Debug.LogWarning("Aucun objet � ramasser � proximit�.");
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
            Debug.Log("Ajout de l'objet " + currentNearbyItem.itemSO.itemName + " � l'inventaire.");
            inventoryManager.AddItem(currentNearbyItem.itemSO);
            Debug.Log("Objet ramass� : " + currentNearbyItem.itemSO.itemName);
            Destroy(currentNearbyItem.gameObject);
            currentNearbyItem = null; 
        }
        else
        {
            Debug.LogWarning("Impossible de ramasser l'objet, car aucun objet n'est � proximit�.");
        }
    }
}
