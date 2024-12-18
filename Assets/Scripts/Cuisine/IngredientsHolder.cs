using UnityEngine;

public class IngredientHolder : MonoBehaviour
{
    public Ingredient ingredientData; // R�f�rence au ScriptableObject associ�

    public void DisplayInfo()
    {
        if (ingredientData != null)
        {
            Debug.Log($"Nom de l'ingr�dient : {ingredientData.ingredientName}");
            Debug.Log($"Description : {ingredientData.description}");
        }
        else
        {
            Debug.LogWarning("Aucun ingr�dient assign� � cet objet.");
        }
    }
}
