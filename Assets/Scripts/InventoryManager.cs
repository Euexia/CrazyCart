using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();

    // Ajout d'un ItemSO à l'inventaire
    public void AddItem(ItemSO itemSO)
    {
        // Vérifie si l'item est déjà présent dans l'inventaire
        if (items.ContainsKey(itemSO))
        {
            items[itemSO]++;  // Augmente la quantité si l'item est déjà dans l'inventaire
        }
        else
        {
            items[itemSO] = 1;  // Ajoute l'item pour la première fois
        }

        // Met à jour l'interface utilisateur
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Ici vous mettez à jour l'UI avec les items stockés dans le dictionnaire
        inventoryUI.UpdateUI(items);
    }
}
