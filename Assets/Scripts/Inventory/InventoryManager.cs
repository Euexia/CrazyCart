using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();
    public int baseCapacity = 1;
    private ImprovementManager improvementManager;

    void Start()
    {
        improvementManager = FindObjectOfType<ImprovementManager>();
    }

    public int GetMaxCapacity()
    {
        return baseCapacity + (improvementManager?.inventoryCapacityBonus ?? 0);
    }

    public void AddItem(ItemSO itemSO)
    {
        // Vérifier si le nombre total d'objets dans l'inventaire dépasse la capacité maximale
        int totalItems = 0;
        foreach (var item in items.Values)
        {
            totalItems += item;
        }

        if (totalItems >= GetMaxCapacity())
        {
            Debug.Log("Capacité maximale atteinte.");
            return; // Ne pas ajouter d'objets si la capacité maximale est atteinte
        }

        if (items.ContainsKey(itemSO))
        {
            items[itemSO]++;
        }
        else
        {
            items[itemSO] = 1;
        }

        UpdateInventoryUI();
    }


    public void RemoveItem(ItemSO itemSO)
    {
        if (items.ContainsKey(itemSO))
        {
            items[itemSO]--;

            if (items[itemSO] <= 0)
            {
                items.Remove(itemSO);
            }

            UpdateInventoryUI();
        }
    }

    public void UpdateInventoryUI()
    {
        inventoryUI.UpdateUI(items);
    }

    public bool HasItem(ItemSO item)
    {
        return items.ContainsKey(item) && items[item] > 0;
    }
}
