using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class FurnitureUI : MonoBehaviour
{
    public Furniture furnitureData;
    public GameObject shelfButtonPrefab;
    public Transform shelfButtonsParent;
    public GridLayoutGroup gridLayout;
    public TMP_Text itemDescriptionText;
    public Button closeCanvasButton;
    public Camera mainCamera;
    public PickUpObject pickUpObjectScript;

    private Ingredient currentIngredient;
    public GameObject leftHandButton;
    public GameObject rightHandButton;

    void Start()
    {
        CreateShelfButtons();
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
        closeCanvasButton.onClick.AddListener(CloseCanvas);

        leftHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("left"));
        rightHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("right"));
    }

    void CreateShelfButtons()
    {
        foreach (var shelf in furnitureData.shelves)
        {
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));
        }
    }

    void ShowShelfItems(Furniture.Shelf shelf)
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in shelf.ingredients)
        {
            if (ingredient != null)
            {
                GameObject itemUI = new GameObject("IngredientImage");
                itemUI.transform.SetParent(gridLayout.transform);

                Image imageComponent = itemUI.AddComponent<Image>();
                imageComponent.sprite = ingredient.ingredientSprite;

                EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

                EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient));

                eventTrigger.triggers.Add(entryPointerClick);
            }
        }
    }

    void OnIngredientClick(Ingredient ingredient)
    {
        currentIngredient = ingredient;

        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);

        Debug.Log($"Ingr�dient s�lectionn� : {ingredient.name}");
    }


    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            // Assigne l'ingr�dient � une main
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // Cache les boutons de s�lection de la main
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Supprime visuellement l'ingr�dient du canvas
            RemoveIngredientFromShelf(currentIngredient);
            currentIngredient = null; // R�initialise l'ingr�dient courant
        }
    }
    void RemoveIngredientFromShelf(Ingredient ingredient)
    {
        // Supprime l'ingr�dient de la liste de donn�es
        foreach (var shelf in furnitureData.shelves)
        {
            if (shelf.ingredients.Contains(ingredient))
            {
                shelf.ingredients.Remove(ingredient);
                break;
            }
        }

        // Supprime visuellement l'objet du canvas
        foreach (Transform child in gridLayout.transform)
        {
            Image imageComponent = child.GetComponent<Image>();

            if (imageComponent != null && imageComponent.sprite == ingredient.ingredientSprite)
            {
                Destroy(child.gameObject);
                Debug.Log($"L'ingr�dient {ingredient.name} a �t� retir� de l'interface.");
                return;
            }
        }

        Debug.LogWarning("Impossible de trouver l'ingr�dient � supprimer dans le canvas.");
    }


    void CloseCanvas()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
    }
}
