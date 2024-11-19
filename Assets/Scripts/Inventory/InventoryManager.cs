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
                Debug.LogError("ImprovementManager non trouvé dans la scène !");
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
            inventoryFullCanvas.SetActive(true);
            Invoke(nameof(HideInventoryFullMessage), 4f);
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

            UpdateInventoryUI();
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
                return;
            }

            AddItem(carton.cartonItemSO);

            carton.gameObject.SetActive(false);
            cartons.Add(carton);
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

                cartons.Add(carton);
            }
        }
    }

    public void RemoveCarton(Carton carton)
    {
        if (carton == null)
        {
            return;
        }

        if (cartons.Contains(carton))
        {
            cartons.Remove(carton);

            if (carton.cartonItemSO != null)
            {
                RemoveItem(carton.cartonItemSO);
            }

            carton.gameObject.SetActive(true);
        }

        UpdateInventoryUI();
    }

    public ItemSO FindItemByName(string itemName)
    {
        foreach (var item in items.Keys)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }
}
