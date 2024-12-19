using UnityEngine;

public class IngredientHandSelectionHandler : MonoBehaviour
{
    public PickUpObject pickUpObjectScript; // Référence au script qui gère la sélection d'objet
    public GameObject leftHandButton; // Bouton pour la main gauche
    public GameObject rightHandButton; // Bouton pour la main droite
    private GameObject currentIngredientUI; // UI de l'ingrédient sélectionné
    private Ingredient currentIngredient; // Ingrédient sélectionné

    // Méthode pour définir l'ingrédient sélectionné et son UI
    public void SetCurrentIngredient(Ingredient ingredient, GameObject ingredientUI)
    {
        currentIngredient = ingredient;
        currentIngredientUI = ingredientUI;
    }

    // Méthode appelée lors du clic sur le bouton de la main gauche
    public void OnLeftHandButtonClick()
    {
        if (currentIngredient != null)
        {
            // Attribuer l'ingrédient à la main gauche
            pickUpObjectScript.HandleHandSelection("left");

            // Détruire l'UI de l'ingrédient une fois pris
            Destroy(currentIngredientUI);

            // Cacher les boutons des mains après la sélection
            HideHandButtons();
        }
        else
        {
            Debug.LogWarning("Aucun ingrédient sélectionné pour la main gauche.");
        }
    }

    public void OnRightHandButtonClick()
    {
        if (currentIngredient != null)
        {
            // Attribuer l'ingrédient à la main droite
            pickUpObjectScript.HandleHandSelection("right");

            // Détruire l'UI de l'ingrédient une fois pris
            Destroy(currentIngredientUI);

            // Cacher les boutons des mains après la sélection
            HideHandButtons();
        }
        else
        {
            Debug.LogWarning("Aucun ingrédient sélectionné pour la main droite.");
        }
    }

    private void HideHandButtons()
    {
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
    }

}
