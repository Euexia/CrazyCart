using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    public Ingredient ingredientData; // Référence au ScriptableObject associé

    public void DisplayInfo()
    {
        if (ingredientData != null)
        {
            Debug.Log($"Nom de l'ingrédient : {ingredientData.ingredientName}");
            Debug.Log($"Description : {ingredientData.description}");
        }
        else
        {
            Debug.LogWarning("Aucun ingrédient assigné à cet objet.");
        }
    }
}
