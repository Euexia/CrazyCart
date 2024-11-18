using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public InventoryManager inventoryManager;
    private bool isPlayerNearby = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
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
            var itemToDiscard = inventoryManager.GetFirstItem();

            if (itemToDiscard != null)
            {
                inventoryManager.DiscardItem(itemToDiscard);
            }
        }
    }
}
