using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe/Recipe")]
public class Recipe : ScriptableObject
{
    public string title;
    public string[] ingredients;
    public string description;

   public void DisplayRecipe(TMP_Text titleText, TMP_Text ingredientsText, TMP_Text descriptionText)
    {
        // Vérifiez d'abord si les références sont nulles
        if (titleText == null || ingredientsText == null || descriptionText == null)
        {
            Debug.LogError("One or more TMP_Text references are not assigned.");
            return;
        }

        // Ensuite, assignez les valeurs
        titleText.text = title;
        ingredientsText.text = string.Join("\n", ingredients);
        descriptionText.text = description;
    }

}
