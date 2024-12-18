using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Kuisine/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    [TextArea]
    public string description;
    public Sprite ingredientSprite; 
}
