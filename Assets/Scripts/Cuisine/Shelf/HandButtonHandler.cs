using UnityEngine;

public class HandButtonHandler : MonoBehaviour
{
    public PickUpObject pickUpObjectScript;
    public GameObject leftHandButton;
    public GameObject rightHandButton;
    private GameObject currentIngredientUI;
    private Ingredient currentIngredient;

    public void SetCurrentIngredient(Ingredient ingredient, GameObject ingredientUI)
    {
        currentIngredient = ingredient;
        currentIngredientUI = ingredientUI;
    }

    public void OnLeftHandButtonClick()
    {
        if (currentIngredient != null)
        {
            pickUpObjectScript.HandleHandSelection("left");
            Destroy(currentIngredientUI);
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
            pickUpObjectScript.HandleHandSelection("right");
            Destroy(currentIngredientUI);
            HideHandButtons();
        }
        else
        {
            Debug.LogWarning("Aucun ingrédient sélectionné pour la main droite.");
        }
    }

    void HideHandButtons()
    {
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
    }
}
