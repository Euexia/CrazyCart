using System;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public string acceptedItemName; 
    public int maxCapacity = 10;
    private int currentCapacity = 0;

    public bool IsFull()
    {
        return currentCapacity >= maxCapacity;
    }

    public void RefillShelf(ItemSO item, int quantity)
    {
        Debug.Log($"Tentative de remplir l'�tag�re avec : {item?.itemName ?? "null"} (quantit� : {quantity})");
        Debug.Log($"Nom accept� par l'�tag�re : {acceptedItemName}");

        if (item != null && string.Equals(item.itemName, acceptedItemName, StringComparison.OrdinalIgnoreCase))
        {
            currentCapacity += quantity;
            Debug.Log($"L'�tag�re a �t� remplie avec {quantity} {item.itemName}(s).");
        }
        else
        {
            Debug.Log("Cet item n'est pas compatible avec cette �tag�re.");
        }
    }

}
