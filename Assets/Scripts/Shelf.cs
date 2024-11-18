using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public string acceptedItemName;
    public int maxCapacity = 10;
    private int currentCapacity = 0;

    // Liste des items visibles sur l'étagère
    public List<GameObject> shelfItems = new List<GameObject>();

    public bool IsFull()
    {
        return currentCapacity >= maxCapacity;
    }

    public void RefillShelf(ItemSO item, int quantity)
    {
        if (item != null && item.itemName == acceptedItemName)
        {
            for (int i = 0; i < quantity; i++)
            {
                ActivateNextItem(); // Réactive un item existant.
                currentCapacity++;
                Debug.Log($"Ajouté : {item.itemName}");
            }
        }
        else
        {
            Debug.Log("Cet item n'est pas compatible avec cette étagère.");
        }
    }

    public void ActivateNextItem()
    {
        foreach (GameObject item in shelfItems)
        {
            if (!item.activeSelf) // Si un item est inactif, on le réactive.
            {
                item.SetActive(true);
                return;
            }
        }
        Debug.LogWarning("Aucun objet à réactiver !");
    }

    public void TakeItemFromShelf()
    {
        for (int i = shelfItems.Count - 1; i >= 0; i--)
        {
            if (shelfItems[i].activeSelf) // On désactive le premier item actif trouvé.
            {
                shelfItems[i].SetActive(false);
                currentCapacity--;
                return;
            }
        }
        Debug.LogWarning("Aucun objet disponible pour être pris !");
    }
}
