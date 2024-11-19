using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform inventoryPanel;

    public void UpdateUI(Dictionary<ItemSO, int> items)
    {
        // Détruit tous les enfants existants dans le panel
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // Ajoute un slot pour chaque item dans l'inventaire
        foreach (var itemEntry in items)
        {
            if (itemEntry.Key == null || itemEntry.Value <= 0)
            {
                continue;
            }

            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);

            if (slot != null)
            {
                // Recherche des composants dans le prefab
                Transform itemIconTransform = slot.transform.Find("ItemIcon");
                Transform quantityTextTransform = slot.transform.Find("QuantityText");

                if (itemIconTransform != null && quantityTextTransform != null)
                {
                    Image iconImage = itemIconTransform.GetComponent<Image>();
                    TextMeshProUGUI quantityText = quantityTextTransform.GetComponent<TextMeshProUGUI>();

                    if (iconImage != null && quantityText != null)
                    {
                        // Mise à jour de l'icône et de la quantité
                        iconImage.sprite = itemEntry.Key.icon;
                        quantityText.text = itemEntry.Value.ToString();
                    }
                }
            }
        }
    }
}
