using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private List<ItemData> nearbyItems = new List<ItemData>();
    private Carton currentCarton;
    private Shelf nearbyShelf;
    private float interactionRange = 3f;
    private List<GameObject> shelfItems = new List<GameObject>();

    private float placeCartonRange = 3f; // Distance maximale pour reposer un carton

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Ramasser un carton
            if (currentCarton == null && nearbyItems.Count > 0)
            {
                PickupItem();
            }
            else if (currentCarton != null && nearbyShelf == null)
            {
                PickupCarton();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Recharger une étagère
            if (nearbyShelf != null)
            {
                RefillShelf(nearbyShelf);
            }
            else
            {
                Debug.Log("Aucune étagère à proximité.");
            }
        }

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            PlaceCarton();
        }

        FindNearbyShelf();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null && !nearbyItems.Contains(itemData))
            {
                nearbyItems.Add(itemData);
            }
        }
        else if (other.CompareTag("Carton"))
        {
            Carton carton = other.GetComponent<Carton>();
            if (carton != null && currentCarton == null)
            {
                currentCarton = carton;
                Debug.Log($"Carton pris: {carton.gameObject.name}");
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
        else if (other.CompareTag("Carton"))
        {
            Carton carton = other.GetComponent<Carton>();
            if (carton == currentCarton)
            {
                currentCarton = null;
                Debug.Log("Carton quitté.");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = collision.collider.GetComponent<Shelf>();
            Debug.Log("Étagère détectée.");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = null;
            Debug.Log("Étagère quittée.");
        }
    }

    private void PickupItem()
    {
        if (nearbyItems.Count > 0)
        {
            ItemData itemToPickup = nearbyItems[0];
            if (inventoryManager.IsInventoryFull())
            {
                inventoryManager.ShowInventoryFullMessage(); 
                return;
            }

            inventoryManager.AddItem(itemToPickup.itemSO);

            itemToPickup.gameObject.SetActive(false);
            nearbyItems.RemoveAt(0);

            Debug.Log($"Item {itemToPickup.itemSO.itemName} ramassé.");
        }
    }

    void PickupCarton()
    {
        if (currentCarton != null)
        {
            inventoryManager.PickupCarton(currentCarton);
            currentCarton.gameObject.SetActive(false);

            Debug.Log($"Carton {currentCarton.gameObject.name} ajouté à l'inventaire.");
        }
        else
        {
            Debug.Log("Aucun carton à prendre.");
        }
    }

    public void RefillShelf(Shelf shelf)
    {
        if (currentCarton == null || currentCarton.containedItem == null)
        {
            Debug.Log("Pas de carton ou contenu invalide.");
            return;
        }

        if (!string.Equals(currentCarton.containedItem.itemSO.itemName, shelf.acceptedItemName, StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Le carton ne correspond pas à l'étagère ({shelf.acceptedItemName}).");
            return;
        }

        if (shelf.IsFull())
        {
            Debug.Log("L'étagère est déjà pleine.");
            return;
        }

        // Ajouter l'item dans l'étagère
        shelf.RefillShelf(currentCarton.containedItem.itemSO, 1);
        Debug.Log($"Étagère rechargée avec {currentCarton.containedItem.itemSO.itemName}.");
    }

    public void ActivateNextItem()
    {
        foreach (GameObject item in shelfItems)
        {
            if (!item.activeSelf) // Trouve un objet désactivé
            {
                item.SetActive(true); // Le réactive
                Debug.Log($"Item {item.name} réactivé sur l'étagère.");
                return;
            }
        }

        Debug.LogWarning("Aucun objet à réactiver sur l'étagère.");
    }

    void FindNearbyShelf()
    {
        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        Shelf closestShelf = null;
        float closestDistance = interactionRange; // Seuil pour la proximité

        foreach (Shelf shelf in allShelves)
        {
            float distance = Vector3.Distance(transform.position, shelf.transform.position);
            if (distance <= closestDistance)
            {
                closestShelf = shelf;
                closestDistance = distance;
            }
        }

        nearbyShelf = closestShelf;
    }

    void PlaceCarton()
    {
        if (currentCarton != null)
        {
            float distanceToOriginalPosition = Vector3.Distance(transform.position, currentCarton.OriginalPosition);
            if (distanceToOriginalPosition > placeCartonRange)
            {
                Debug.Log("Trop loin de l'emplacement d'origine pour reposer le carton.");
                return;
            }

            // Réinitialiser la position du carton
            currentCarton.ResetToOriginalPosition();

            // Supprimer le carton de l'inventaire
            if (inventoryManager != null)
            {
                inventoryManager.RemoveCarton(currentCarton);
            }

            // Libérer la référence
            currentCarton = null;

            Debug.Log("Carton reposé à son emplacement d'origine et retiré de l'inventaire.");
        }
        else
        {
            Debug.Log("Aucun carton à reposer.");
        }
    }


}
