using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBook : MonoBehaviour
{
    public TMP_Text leftPageTitle;
    public TMP_Text leftPageIngredients;
    public TMP_Text leftPageDescription;

    public TMP_Text rightPageTitle;
    public TMP_Text rightPageIngredients;
    public TMP_Text rightPageDescription;

    public Button leftPageButton;
    public Button rightPageButton;
    public Button nextPageButton;
    public Button previousPageButton;

    public GameObject popupPanel;
    public TMP_Text popupTitleText;
    public TMP_Text popupIngredientsText;
    public TMP_Text popupDescriptionText;
    public Button closePopupButton;

    private int currentLeftRecipeIndex = 0;
    private int currentRightRecipeIndex = 1;
    private Book book;

    void Start()
    {
        book = FindObjectOfType<Book>();
        UpdatePages();

        leftPageButton.onClick.AddListener(() => ShowPopup(currentLeftRecipeIndex));
        rightPageButton.onClick.AddListener(() => ShowPopup(currentRightRecipeIndex));
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        closePopupButton.onClick.AddListener(ClosePopup);

        popupPanel.SetActive(false);
    }

    void UpdatePages()
    {
        if (book != null)
        {
            // Affiche la recette pour la page gauche
            Recipe leftRecipe = book.GetRecipe(currentLeftRecipeIndex);
            if (leftRecipe != null)
            {
                leftRecipe.DisplayRecipe(leftPageTitle, leftPageIngredients, leftPageDescription);
            }
            else
            {
                leftPageTitle.text = "";
                leftPageIngredients.text = "";
                leftPageDescription.text = "";
            }

            // Affiche la recette pour la page droite
            Recipe rightRecipe = book.GetRecipe(currentRightRecipeIndex);
            if (rightRecipe != null)
            {
                rightRecipe.DisplayRecipe(rightPageTitle, rightPageIngredients, rightPageDescription);
            }
            else
            {
                rightPageTitle.text = "";
                rightPageIngredients.text = "";
                rightPageDescription.text = "";
            }
        }
    }

    void ShowPopup(int recipeIndex)
    {
        if (book != null)
        {
            Recipe recipe = book.GetRecipe(recipeIndex);
            if (recipe != null)
            {
                popupTitleText.text = recipe.title;
                popupIngredientsText.text = string.Join("\n", recipe.ingredients);
                popupDescriptionText.text = recipe.description;
                popupPanel.SetActive(true);
            }
        }
    }

    void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    void NextPage()
    {
        currentLeftRecipeIndex = Mathf.Min(currentLeftRecipeIndex + 2, book.recipes.Count - 2);
        currentRightRecipeIndex = Mathf.Min(currentRightRecipeIndex + 2, book.recipes.Count - 1);
        UpdatePages();
    }

    void PreviousPage()
    {
        currentLeftRecipeIndex = Mathf.Max(currentLeftRecipeIndex - 2, 0);
        currentRightRecipeIndex = Mathf.Max(currentRightRecipeIndex - 2, 1);
        UpdatePages();
    }
}
