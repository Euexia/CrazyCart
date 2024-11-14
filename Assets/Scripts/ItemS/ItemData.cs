using UnityEngine;

public class ItemData : MonoBehaviour
{
    public ItemSO itemSO;

    void Start()
    {
        if (itemSO != null)
        {
            Debug.Log("ItemData initialisé avec l'objet : " + itemSO.itemName);
        }
        else
        {
            Debug.LogError("itemSO est nul ! Assurez-vous qu'un ScriptableObject est assigné.");
        }
    }
}
