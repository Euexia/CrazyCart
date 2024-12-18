
using UnityEditor.PackageManager;
using UnityEngine;

public class ClientInteraction : MonoBehaviour
{
    public Client client;
    public InventoryManager playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool hasItem = client.CheckIfPlayerHasItem();  // Appelez la m?thode sans param?tre
            if (hasItem)
            {
                Debug.Log("Le joueur a satisfait la demande du client !");
                // Logique pour confirmer la commande (ex: r?compense, message, etc.)
            }
            else
            {
                Debug.Log("Le joueur n'a pas encore l'article demand?.");
            }
        }
    }
}