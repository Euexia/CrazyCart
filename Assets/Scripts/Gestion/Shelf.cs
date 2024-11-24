using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public string acceptedItemName;
    public int maxCapacity = 10;
    private int currentCapacity = 0;

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
                ActivateNextItem(); 
                currentCapacity++;
            }
        }
    }

    public void ActivateNextItem()
    {
        foreach (GameObject item in shelfItems)
        {
            if (!item.activeSelf) 
            {
                item.SetActive(true);
                return;
            }
        }
    }

    public void TakeItemFromShelf()
    {
        for (int i = shelfItems.Count - 1; i >= 0; i--)
        {
            if (shelfItems[i].activeSelf) 
            {
                shelfItems[i].SetActive(false);
                currentCapacity--;
                return;
            }
        }
    }
}
