using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public Sprite itemIcon; 
    public int quantity;

    public InventoryItem(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
        quantity = 1;  
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }
}
