using UnityEngine;
using UnityEngine.UI;

public class RecipeHover : MonoBehaviour
{
    public Button recipeButton;

    private void OnMouseEnter()
    {
        recipeButton.GetComponent<Image>().color = Color.yellow;  
    }

    private void OnMouseExit()
    {
        recipeButton.GetComponent<Image>().color = Color.white; 
    }
}
