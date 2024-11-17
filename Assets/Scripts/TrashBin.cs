using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public InventoryManager inventoryManager; // Référence à l'inventaire
    private bool isPlayerNearby = false; // Indique si le joueur est proche de la poubelle

    // Détecter quand le joueur entre dans la zone d'interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("Appuyez sur 'E' pour jeter un objet.");
        }
    }

    // Détecter quand le joueur sort de la zone d'interaction
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("Vous êtes trop loin de la poubelle.");
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            DiscardItem();
        }
    }

    private void DiscardItem()
    {
        if (inventoryManager != null)
        {
            // Par défaut, on jette le premier objet trouvé
            var itemToDiscard = inventoryManager.GetFirstItem();

            if (itemToDiscard != null)
            {
                inventoryManager.DiscardItem(itemToDiscard);
                Debug.Log($"Vous avez jeté : {itemToDiscard.itemName}");
            }
            else
            {
                Debug.Log("Votre inventaire est vide !");
            }
        }
        else
        {
            Debug.LogError("InventoryManager non assigné !");
        }
    }
}
