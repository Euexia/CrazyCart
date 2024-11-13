using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // N'oubliez pas d'ajouter ce namespace

public class InventoryUI : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public Transform inventoryPanel;

    public void UpdateUI(Dictionary<ItemSO, int> items)
    {
        // Nettoie le contenu actuel du panel
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // Remplit le panel avec des objets mis à jour
        foreach (var itemEntry in items)
        {
            if (itemEntry.Key == null || itemEntry.Value <= 0)
            {
                Debug.LogWarning("L'élément ou la quantité est invalide pour l'objet : " + itemEntry.Key);
                continue;  // Ignore cet item s'il est invalide
            }

            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);

            if (slot != null)
            {
                Transform itemIconTransform = slot.transform.Find("ItemIcon");
                Transform quantityTextTransform = slot.transform.Find("QuantityText");

                if (itemIconTransform != null && quantityTextTransform != null)
                {
                    Image iconImage = itemIconTransform.GetComponent<Image>();
                    TextMeshProUGUI quantityText = quantityTextTransform.GetComponent<TextMeshProUGUI>();  // Utiliser TextMeshProUGUI

                    if (iconImage != null && quantityText != null)
                    {
                        iconImage.sprite = itemEntry.Key.icon;
                        quantityText.text = itemEntry.Value.ToString(); // Affiche la quantité
                    }
                    else
                    {
                        Debug.LogError("Composants Image ou TextMeshProUGUI manquants sur l'élément dans le prefab.");
                    }
                }
                else
                {
                    Debug.LogError("L'objet 'ItemIcon' ou 'QuantityText' est introuvable dans le prefab.");
                }
            }
            else
            {
                Debug.LogError("Échec de l'instanciation du prefab d'élément.");
            }
        }
    }
}