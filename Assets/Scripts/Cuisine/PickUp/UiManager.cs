/*using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Image leftHandUI;
    public Image rightHandUI;
    public GameObject worldSpaceCanvas;
    public GameObject handsFullCanvas;
    public GameObject takeOrDropCanvas;
    public GameObject handSelectionCanvas;

    public void DisplayCanvas(GameObject obj, string currentAction)
    {
        if (obj.layer == LayerMask.NameToLayer("Container"))
        {
            takeOrDropCanvas.transform.position = obj.transform.position + new Vector3(0f, 1f, 0f);
            takeOrDropCanvas.SetActive(true);
            worldSpaceCanvas.SetActive(false);
        }
        else if (obj.layer == LayerMask.NameToLayer("PickUpItem"))
        {
            worldSpaceCanvas.transform.position = obj.transform.position + new Vector3(0f, 1f, 0f);
            worldSpaceCanvas.SetActive(true);
            takeOrDropCanvas.SetActive(false);
        }
    }

    public void HideAllCanvases()
    {
        takeOrDropCanvas.SetActive(false);
        handSelectionCanvas.SetActive(false);
        worldSpaceCanvas.SetActive(false);
    }
    public void HandleHandSelection(string hand)
    {
        if (currentAction == "pick")
        {
            if (leftHandObject != null && rightHandObject != null)
            {
                ShowHandsFullCanvas();
                return;
            }

            AssignIngredientToHand(currentObject.GetComponent<IngredientHolder>().ingredientData, hand);
        }
        else if (currentAction == "drop")
        {
            RemoveFromHand(hand);
        }
    }
}
*/