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

        Debug.Log($"Objet ajouté à l'inventaire : {itemSO.itemName}");

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
        Debug.Log("Mise à jour de l'inventaire UI...");

        // Affichage des items actuels pour le débogage
        foreach (var item in items)
        {
            Debug.Log($"Item: {item.Key.itemName}, Quantité: {item.Value}");
        }

        inventoryUI.UpdateUI(items); // Mise à jour réelle de l'UI
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
            cartons.Add(carton); // Ajoute le carton à la liste
            Debug.Log($"Carton {carton.gameObject.name} ajouté à l'inventaire.");
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
                Debug.Log($"Carton {carton.gameObject.name} ajouté en tant qu'item {carton.cartonItemSO.itemName} dans l'inventaire.");

                cartons.Add(carton);
            }
            else
            {
                Debug.LogError($"Le carton {carton.gameObject.name} n'a pas de ItemSO associé.");
            }
        }
        else
        {
            Debug.LogError("Le carton est nul.");
        }
    }

    public void RemoveCarton(Carton carton)
    {
        if (carton == null)
        {
            Debug.LogError("Tentative de supprimer un carton null.");
            return;
        }

        if (cartons.Contains(carton))
        {
            cartons.Remove(carton);
            Debug.Log($"Carton {carton.gameObject.name} supprimé de l'inventaire.");

            if (carton.cartonItemSO != null)
            {
                RemoveItem(carton.cartonItemSO); // Supprime l'item associé
            }

            carton.gameObject.SetActive(true); // Réactive l'objet dans la scène
        }
        else
        {
            Debug.LogWarning($"Le carton {carton.gameObject.name} n'est pas présent dans l'inventaire.");
        }

        UpdateInventoryUI(); // Mise à jour de l'interface utilisateur
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
        Debug.Log($"Aucun item avec le nom '{itemName}' trouvé dans l'inventaire.");
        return null;
    }
}
