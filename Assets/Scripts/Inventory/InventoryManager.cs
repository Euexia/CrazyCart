using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public InventoryUI inventoryUI;
    private Dictionary<ItemSO, int> items = new Dictionary<ItemSO, int>();
    public int baseCapacity = 1;
    private ImprovementManager improvementManager;

    public GameObject inventoryFullCanvas;
    public List<Carton> cartons = new List<Carton>();

    private Dictionary<Carton, bool> cartonStates = new Dictionary<Carton, bool>();

    void Start()
    {
        if (inventoryFullCanvas != null)
        {
            inventoryFullCanvas.SetActive(false);
        }
    }
    void Awake()
    {
        if (improvementManager == null)
        {
            improvementManager = FindObjectOfType<ImprovementManager>();
            if (improvementManager == null)
            {
                Debug.LogError("ImprovementManager non trouv� dans la sc�ne !");
            }
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
            ShowInventoryFullMessage();
            return;
        }

        if (items.ContainsKey(itemSO))
        {
            items[itemSO]++;
        }
        else
        {
            items[itemSO] = 1;
        }

        Debug.Log($"Objet ajout� � l'inventaire : {itemSO.itemName}");

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
            inventoryFullCanvas.SetActive(true);
            Debug.Log("Canvas activ� : " + inventoryFullCanvas.activeSelf);

            Invoke(nameof(HideInventoryFullMessage), 4f);
        }
        else
        {
            Debug.LogError("Le Canvas pour 'Inventaire plein' n'est pas assign� dans l'inspecteur !");
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

    public void DiscardItem(ItemSO itemSO)
    {
        if (items.ContainsKey(itemSO))
        {
            items[itemSO]--;
            if (items[itemSO] <= 0)
            {
                items.Remove(itemSO);
            }

            Debug.Log($"Objet {itemSO.itemName} jet�.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("L'objet n'est pas pr�sent dans l'inventaire.");
        }
    }

    public ItemSO GetFirstItem()
    {
        foreach (var item in items.Keys)
        {
            return item;
        }
        return null;
    }

    public void PickupCarton(Carton carton)
    {
        if (carton != null)
        {
            if (IsInventoryFull())
            {
                Debug.Log("Inventaire plein, impossible de ramasser le carton.");
                return;
            }

            AddItem(carton.cartonItemSO);

            carton.gameObject.SetActive(false);
            Debug.Log($"Carton {carton.gameObject.name} ajout� � l'inventaire.");
        }
        else
        {
            Debug.LogError("Tentative de ramasser un carton null.");
        }
    }

    public Carton GetCartonFromInventory(string itemName)
    {
        foreach (var carton in cartons)
        {
            if (carton.cartonItemSO != null && carton.cartonItemSO.itemName == itemName)
            {
                return carton;
            }
        }
        return null;
    }

    public void AddCarton(Carton carton)
    {
        if (carton != null)
        {
            if (carton.cartonItemSO != null)
            {
                AddItem(carton.cartonItemSO);
                Debug.Log($"Carton {carton.gameObject.name} ajout� en tant qu'item {carton.cartonItemSO.itemName} dans l'inventaire.");

                cartons.Add(carton);
            }
            else
            {
                Debug.LogError($"Le carton {carton.gameObject.name} n'a pas de ItemSO associ�.");
            }
        }
        else
        {
            Debug.LogError("Le carton est nul.");
        }
    }

    public ItemSO FindItemByName(string itemName)
    {
        foreach (var item in items.Keys)
        {
            Debug.Log($"Item dans l'inventaire : {item.itemName}");
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        Debug.Log($"Aucun item avec le nom '{itemName}' trouv� dans l'inventaire.");
        return null;
    }
}
