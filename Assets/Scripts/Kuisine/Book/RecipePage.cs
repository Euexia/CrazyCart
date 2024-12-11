using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipePage : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text ingredientsText;
    public TMP_Text descriptionText;
    public Button nextPageButton;
    public Button previousPageButton;

    private int currentRecipeIndex = 0;
    private Book book;

    void Start()
    {
        book = FindObjectOfType<Book>();
        ShowRecipe(currentRecipeIndex);

        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
    }

    void ShowRecipe(int index)
    {
        if (book != null)
        {
            Recipe recipe = book.GetRecipe(index);
            if (recipe != null)
            {
                recipe.DisplayRecipe(titleText, ingredientsText, descriptionText);
            }
        }
    }

    void NextPage()
    {
        currentRecipeIndex = Mathf.Min(currentRecipeIndex + 1, book.recipes.Count - 1);
        ShowRecipe(currentRecipeIndex);
    }

    void PreviousPage()
    {
        currentRecipeIndex = Mathf.Max(currentRecipeIndex - 1, 0);
        ShowRecipe(currentRecipeIndex);
    }
}
