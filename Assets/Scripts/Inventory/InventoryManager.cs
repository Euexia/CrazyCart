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

    void Start()
    {
        improvementManager = FindObjectOfType<ImprovementManager>();

     
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
            inventoryFullCanvas.SetActive(true); 
            Debug.Log("Canvas activé : " + inventoryFullCanvas.activeSelf);

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
            ShowInventoryFullMessage(); 
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

            Debug.Log($"Objet {itemSO.itemName} jeté.");
            UpdateInventoryUI();
        }
        else
        {
            Debug.Log("L'objet n'est pas présent dans l'inventaire.");
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


   /* public void PickupCarton(Carton carton)
    {
        if (carton != null)
        {
            cartons.Add(carton);  // Ajouter le carton à la liste

            if (carton.containedItem != null && carton.containedItem.itemSO != null)
            {
                AddItem(carton.containedItem.itemSO);  // Ajouter l'ItemSO contenu dans le carton
                Debug.Log($"Carton contenant {carton.containedItem.itemSO.itemName} pris.");
            }
            else
            {
                Debug.LogError("Le carton ne contient aucun item valide.");
            }
        }
    }





    public void AddCarton(Carton carton)
    {
        if (carton != null)
        {
            AddItem(carton.itemSO);
            Debug.Log($"Carton {carton.gameObject.name} ajouté à l'inventaire.");

            cartons.Add(carton);
        }
        else
        {
            Debug.LogError("Le carton est nul.");
        }
    }*/

}
