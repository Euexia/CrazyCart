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
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemEntry in items)
        {
            if (itemEntry.Key == null || itemEntry.Value <= 0)
            {
                continue;  
            }

            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);

            if (slot != null)
            {
                Transform itemIconTransform = slot.transform.Find("ItemIcon");
                Transform quantityTextTransform = slot.transform.Find("QuantityText");

                if (itemIconTransform != null && quantityTextTransform != null)
                {
                    Image iconImage = itemIconTransform.GetComponent<Image>();
                    TextMeshProUGUI quantityText = quantityTextTransform.GetComponent<TextMeshProUGUI>();

                    if (iconImage != null && quantityText != null)
                    {
                        iconImage.sprite = itemEntry.Key.icon;
                        quantityText.text = itemEntry.Value.ToString();
                    }
                }
            }
        }
    }
}