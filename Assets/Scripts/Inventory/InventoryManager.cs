using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();
    public int baseCapacity = 1;
    private ImprovementManager improvementManager;

    public GameObject inventoryFullCanvas; // Canvas pour le message d'inventaire plein

    void Start()
    {
        improvementManager = FindObjectOfType<ImprovementManager>();

        // Assurez-vous que le Canvas pour le message est désactivé au départ
        if (inventoryFullCanvas != null)
        {
            inventoryFullCanvas.SetActive(false);
        }
    }

    public int GetMaxCapacity()
    {
        return baseCapacity + (improvementManager?.inventoryCapacityBonus ?? 0);
    }

    public bool IsInventoryFull()
    {
        int totalItems = 0;
        foreach (var item in items.Values)
        {
            totalItems += item;
        }
        return totalItems >= GetMaxCapacity();
    }

    public void AddItem(ItemSO itemSO)
    {
        if (IsInventoryFull())
        {
            Debug.Log("Capacité maximale atteinte. Tentative d'affichage du Canvas.");
            ShowInventoryFullMessage(); // Appel pour afficher le message
            return; // Ne pas ajouter d'objets si l'inventaire est plein
        }

        if (items.ContainsKey(itemSO))
        {
            items[itemSO]++;
        }
        else
        {
            items[itemSO] = 1;
        }

        Debug.Log("Objet ajouté à l'inventaire : " + itemSO.itemName);
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

    public void ShowInventoryFullMessage()
    {
        if (inventoryFullCanvas != null)
        {
            Debug.Log("Tentative d'affichage du Canvas : Inventaire plein.");
            inventoryFullCanvas.SetActive(true); // Activer le Canvas
            Debug.Log("Canvas activé : " + inventoryFullCanvas.activeSelf);

            // Cacher le Canvas après 2 secondes
            Invoke(nameof(HideInventoryFullMessage), 4f);
        }
        else
        {
            Debug.LogError("Le Canvas pour 'Inventaire plein' n'est pas assigné dans l'inspecteur !");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowInventoryFullMessage(); // Appeler directement pour tester
        }
    }



    private void HideInventoryFullMessage()
    {
        if (inventoryFullCanvas != null)
        {
            inventoryFullCanvas.SetActive(false);
        }
    }

    public bool HasItem(ItemSO item)
    {
        return items.ContainsKey(item) && items[item] > 0;
    }
}
