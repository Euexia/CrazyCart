using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private List<ItemData> nearbyItems = new List<ItemData>(); // Liste des objets à proximité

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearbyItems.Count > 0) // Vérifie s'il y a des objets à ramasser
        {
            if (inventoryManager.IsInventoryFull())
            {
                inventoryManager.ShowInventoryFullMessage();
            }
            else
            {
                PickupItem();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null && !nearbyItems.Contains(itemData)) // Évite les doublons
            {
                nearbyItems.Add(itemData);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null && nearbyItems.Contains(itemData))
            {
                nearbyItems.Remove(itemData);
            }
        }
    }

    private void PickupItem()
    {
        if (nearbyItems.Count > 0) // Vérifie s'il y a encore des objets
        {
            ItemData itemToPickup = nearbyItems[0]; // Prend le premier objet de la liste
            inventoryManager.AddItem(itemToPickup.itemSO);
            Destroy(itemToPickup.gameObject); // Détruit l'objet dans la scène
            nearbyItems.RemoveAt(0); // Supprime l'objet de la liste
        }
    }
}
