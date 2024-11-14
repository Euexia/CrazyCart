using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();

    // Ajout d'un ItemSO � l'inventaire
    public void AddItem(ItemSO itemSO)
    {
        // V�rifie si l'item est d�j� pr�sent dans l'inventaire
        if (items.ContainsKey(itemSO))
        {
            items[itemSO]++;  // Augmente la quantit� si l'item est d�j� dans l'inventaire
        }
        else
        {
            items[itemSO] = 1;  // Ajoute l'item pour la premi�re fois
        }

        // Met � jour l'interface utilisateur
        UpdateInventoryUI();
    }

    public void RemoveItem(ItemSO itemSO)
    {
        if (items.ContainsKey(itemSO))
        {
            items[itemSO]--;  // R�duit la quantit� de l'item

            // Si la quantit� atteint z�ro, on supprime l'item de l'inventaire
            if (items[itemSO] <= 0)
            {
                items.Remove(itemSO);
            }

            // Met � jour l'UI apr�s modification
            UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning("Item non trouv� dans l'inventaire : " + itemSO.itemName);
        }
    }

    public void UpdateInventoryUI()
    {
        // Ici vous mettez � jour l'UI avec les items stock�s dans le dictionnaire
        inventoryUI.UpdateUI(items);
    }



    public bool HasItem(ItemSO item)
    {
        return items.ContainsKey(item) && items[item] > 0;
    }

}
