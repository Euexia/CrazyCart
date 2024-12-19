using UnityEngine;

public class IngredientHandSelectionHandler : MonoBehaviour
{
    public PickUpObject pickUpObjectScript; // R�f�rence au script qui g�re la s�lection d'objet
    public GameObject leftHandButton; // Bouton pour la main gauche
    public GameObject rightHandButton; // Bouton pour la main droite
    private GameObject currentIngredientUI; // UI de l'ingr�dient s�lectionn�
    private Ingredient currentIngredient; // Ingr�dient s�lectionn�

    // M�thode pour d�finir l'ingr�dient s�lectionn� et son UI
    public void SetCurrentIngredient(Ingredient ingredient, GameObject ingredientUI)
    {
        currentIngredient = ingredient;
        currentIngredientUI = ingredientUI;
    }

    // M�thode appel�e lors du clic sur le bouton de la main gauche
    public void OnLeftHandButtonClick()
    {
        if (currentIngredient != null)
        {
            // Attribuer l'ingr�dient � la main gauche
            pickUpObjectScript.HandleHandSelection("left");

            // D�truire l'UI de l'ingr�dient une fois pris
            Destroy(currentIngredientUI);

            // Cacher les boutons des mains apr�s la s�lection
            HideHandButtons();
        }
        else
        {
            Debug.LogWarning("Aucun ingr�dient s�lectionn� pour la main gauche.");
        }
    }

    public void OnRightHandButtonClick()
    {
        if (currentIngredient != null)
        {
            // Attribuer l'ingr�dient � la main droite
            pickUpObjectScript.HandleHandSelection("right");

            // D�truire l'UI de l'ingr�dient une fois pris
            Destroy(currentIngredientUI);

            // Cacher les boutons des mains apr�s la s�lection
            HideHandButtons();
        }
        else
        {
            Debug.LogWarning("Aucun ingr�dient s�lectionn� pour la main droite.");
        }
    }

    private void HideHandButtons()
    {
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
    }

}
