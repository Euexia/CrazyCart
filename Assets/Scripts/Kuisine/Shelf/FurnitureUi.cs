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
    public GameObject leftHandButton;
    public GameObject rightHandButton;
    public PickUpObject pickUpObjectScript;

    private Ingredient currentIngredient;
    private GameObject currentIngredientUI;
    public Button closeCanvasButton;

    public Camera mainCamera;

    void Start()
    {
        CreateShelfButtons();
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);

        closeCanvasButton.onClick.AddListener(CloseCanvas);
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

        if (shelf.ingredients == null || shelf.ingredients.Count == 0)
        {
            // Handle the case where there are no ingredients in the shelf
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

                EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entryPointerEnter.callback.AddListener((data) => OnPointerEnter(ingredient, itemUI));

                EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                entryPointerExit.callback.AddListener((data) => OnPointerExit(itemUI));

                EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient, itemUI));

                eventTrigger.triggers.Add(entryPointerEnter);
                eventTrigger.triggers.Add(entryPointerExit);
                eventTrigger.triggers.Add(entryPointerClick);
            }
            else
            {
                // Handle the case where the ingredient is null
            }
        }
    }

    void ShowItemDescription(Ingredient ingredient)
    {
        itemDescriptionText.text = ingredient.description;
    }

    void HideItemDescription()
    {
        itemDescriptionText.text = "";
    }

    void OnIngredientClick(Ingredient ingredient, GameObject ingredientUI)
    {
        if (currentIngredientUI != null)
        {
            Image previousImageComponent = currentIngredientUI.GetComponent<Image>();
            if (previousImageComponent != null)
            {
                previousImageComponent.color = Color.white;
            }
        }

        currentIngredient = ingredient;
        currentIngredientUI = ingredientUI;

        Image imageComponent = ingredientUI.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.color = new Color(0f, 1f, 0f, 1f);
        }

        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);
    }

    public void OnLeftHandButtonClick()
    {
        if (currentIngredient != null)
        {
            pickUpObjectScript.HandleHandSelection("left");

            Destroy(currentIngredientUI);
            HideHandButtons();
        }
    }

    public void OnRightHandButtonClick()
    {
        if (currentIngredient != null)
        {
            pickUpObjectScript.HandleHandSelection("right");

            Destroy(currentIngredientUI);
            HideHandButtons();
        }
    }

    void HideHandButtons()
    {
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
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

    void OnPointerEnter(Ingredient ingredient, GameObject itemUI)
    {
        if (currentIngredient != ingredient)
        {
            Image imageComponent = itemUI.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.color = new Color(1f, 1f, 0f, 1f);
            }

            ShowItemDescription(ingredient);
        }
    }

    void OnPointerExit(GameObject itemUI)
    {
        if (currentIngredientUI != itemUI)
        {
            Image imageComponent = itemUI.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.color = Color.white;
            }
        }
    }
}
