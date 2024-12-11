using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipePopup : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text titleText;
    public TMP_Text ingredientsText;
    public TMP_Text descriptionText;
    public Button closeButton;

    private Recipe currentRecipe;

    void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
        popupPanel.SetActive(false);
    }

    public void OpenPopup(Recipe recipe)
    {
        currentRecipe = recipe;
        recipe.DisplayRecipe(titleText, ingredientsText, descriptionText);
        popupPanel.SetActive(true);
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
