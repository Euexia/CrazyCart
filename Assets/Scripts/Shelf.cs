using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public string acceptedItemName;
    public int maxCapacity = 10;
    private int currentCapacity = 0;

    // Liste des items visibles sur l'�tag�re
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
                ActivateNextItem(); // R�active un item existant.
                currentCapacity++;
                Debug.Log($"Ajout� : {item.itemName}");
            }
        }
        else
        {
            Debug.Log("Cet item n'est pas compatible avec cette �tag�re.");
        }
    }

    public void ActivateNextItem()
    {
        foreach (GameObject item in shelfItems)
        {
            if (!item.activeSelf) // Trouve un objet d�sactiv�
            {
                item.SetActive(true); // Le r�active
                Debug.Log($"Item {item.name} r�activ� sur l'�tag�re.");
                return;
            }
        }

        Debug.LogWarning("Aucun objet � r�activer sur l'�tag�re.");
    }


    public void TakeItemFromShelf()
    {
        for (int i = shelfItems.Count - 1; i >= 0; i--)
        {
            if (shelfItems[i].activeSelf) // On d�sactive le premier item actif trouv�.
            {
                shelfItems[i].SetActive(false);
                currentCapacity--;
                return;
            }
        }
        Debug.LogWarning("Aucun objet disponible pour �tre pris !");
    }
}
