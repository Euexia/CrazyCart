using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private List<ItemData> nearbyItems = new List<ItemData>();
    private Carton currentCarton;
    private Shelf nearbyShelf;
    private float interactionRange = 3f;

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (nearbyShelf != null && currentCarton != null)
            {
                RefillShelf(nearbyShelf);
            }
            else if (currentCarton != null)
            {
                PickupCarton();
            }
            else if (nearbyItems.Count > 0)
            {
                PickupItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugItemCompatibility();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (nearbyShelf != null)
            {
                if (currentCarton != null)
                {
                    Debug.Log("Interaction avec l'�tag�re avec le carton.");
                    RefillShelf(nearbyShelf);
                }
                else
                {
                    Debug.Log("Pas de carton �quip�, recherche dans l'inventaire...");

                    ItemSO itemToUse = inventoryManager.FindItemByName(nearbyShelf.acceptedItemName);
                    if (itemToUse != null)
                    {
                        nearbyShelf.RefillShelf(itemToUse, 1);
                        Debug.Log($"�tag�re remplie avec {itemToUse.itemName} depuis l'inventaire.");
                    }
                    else
                    {
                        Debug.Log("Aucun item compatible trouv� dans l'inventaire.");
                    }
                }
            }
            else
            {
                Debug.Log("Pas d'�tag�re � proximit�.");
            }
        }

        FindNearbyShelf();
    }


    void DebugItemCompatibility()
    {
        if (currentCarton != null && nearbyShelf != null)
        {
            Debug.Log($"Carton : {currentCarton.cartonItemSO.itemName}");
            Debug.Log($"�tag�re accepte : {nearbyShelf.acceptedItemName}");
            Debug.Log($"Compatibilit� : {string.Equals(currentCarton.cartonItemSO.itemName.Trim(), nearbyShelf.acceptedItemName.Trim(), System.StringComparison.OrdinalIgnoreCase)}");
        }
        else
        {
            Debug.Log("Aucun carton ou �tag�re d�tect�.");
        }
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
                Debug.Log("Carton quitt�.");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = collision.collider.GetComponent<Shelf>();
            Debug.Log("�tag�re d�tect�e.");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = null;
            Debug.Log("�tag�re quitt�e.");
        }
    }

    private void PickupItem()
    {
        if (nearbyItems.Count > 0)
        {
            ItemData itemToPickup = nearbyItems[0];
            if (inventoryManager.IsInventoryFull())
            {
                Debug.Log("Inventaire plein.");
                return;
            }

            inventoryManager.AddItem(itemToPickup.itemSO);
            Destroy(itemToPickup.gameObject);
            nearbyItems.RemoveAt(0);
        }
    }

    void PickupCarton()
    {
        if (currentCarton != null)
        {
            inventoryManager.PickupCarton(currentCarton); 
            currentCarton.gameObject.SetActive(false); 

            Debug.Log($"Carton {currentCarton.gameObject.name} ajout� � l'inventaire.");
        }
        else
        {
            Debug.Log("Aucun carton � prendre.");
        }
    }

    public void RefillShelf(Shelf shelf)
    {
        if (shelf.IsFull())
        {
            Debug.Log("L'�tag�re est d�j� pleine.");
            return;
        }

        ItemSO itemToUse = inventoryManager.FindItemByName(shelf.acceptedItemName);
        if (itemToUse != null)
        {
            shelf.RefillShelf(itemToUse, 1);
            Debug.Log($"L'�tag�re a �t� remplie avec {itemToUse.itemName}.");
        }
        else
        {
            Debug.Log("Impossible de trouver un carton compatible dans l'inventaire.");
        }
    }


    void FindNearbyShelf()
    {
        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        nearbyShelf = null;

        foreach (Shelf shelf in allShelves)
        {
            float distance = Vector3.Distance(transform.position, shelf.transform.position);
            if (distance <= interactionRange)
            {
                nearbyShelf = shelf;
                break;
            }
        }
    }
   

}
