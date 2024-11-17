using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private List<ItemData> nearbyItems = new List<ItemData>(); // Liste des objets à proximité
    private Carton currentCarton; // Carton que le joueur porte actuellement

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        // Vérifie si le joueur appuie sur "E"
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentCarton != null)
            {
                // Interagir avec le carton (ajouter le carton à l'inventaire)
                PickupCarton();
            }
            else if (nearbyItems.Count > 0)
            {
                // Si le joueur n'a pas de carton et qu'il y a des items proches, récupérer le premier item
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
        else if (other.CompareTag("Carton"))
        {
            Carton carton = other.GetComponent<Carton>();
            if (carton != null && currentCarton == null) // Le joueur ne porte pas déjà de carton
            {
                currentCarton = carton; // Le carton est pris par le joueur
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
                Debug.Log("Carton hors de portée.");
            }
        }
    }

    private void PickupItem()
    {
        if (nearbyItems.Count > 0)
        {
            ItemData itemToPickup = nearbyItems[0]; // Prend le premier objet de la liste
            inventoryManager.AddItem(itemToPickup.itemSO);
            Destroy(itemToPickup.gameObject); // Détruit l'objet dans la scène
            nearbyItems.RemoveAt(0); // Supprime l'objet de la liste
        }
    }

    private void PickupCarton()
    {
        if (currentCarton != null)
        {
            // Ajouter le carton dans l'inventaire
            inventoryManager.AddCarton(currentCarton); // Ajouter le carton lui-même, sans se préoccuper de son contenu
            Debug.Log($"Carton {currentCarton.gameObject.name} ajouté à l'inventaire.");

            // Désactive le carton dans la scène après l'avoir pris
            currentCarton.gameObject.SetActive(false); // Si tu veux le garder dans la scène, tu peux désactiver le mesh renderer ou autre
            currentCarton = null; // Réinitialise la référence du carton
        }
    }

}
