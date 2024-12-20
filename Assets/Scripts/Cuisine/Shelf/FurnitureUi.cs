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
        // Désactive ou détruit le prefab inutile déjà dans le parent
        if (shelfButtonPrefab.transform.parent == shelfButtonsParent)
        {
            shelfButtonPrefab.SetActive(false); // Cache le prefab s'il est actif
        }

        // Supprime les anciens boutons d'étagère s'ils existent
        foreach (Transform child in shelfButtonsParent)
        {
            if (child.gameObject != shelfButtonPrefab) // Ignore le prefab modèle
            {
                Destroy(child.gameObject);
            }
        }

        // Crée les nouveaux boutons dynamiquement
        foreach (var shelf in furnitureData.shelves)
        {
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);
            button.SetActive(true); // Active le bouton après instanciation
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));
        }
    }


    void ShowShelfItems(Furniture.Shelf shelf)
    {
        // Supprime les anciens éléments de la grille
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        // Crée une nouvelle image pour chaque ingrédient
        foreach (var ingredient in shelf.ingredients)
        {
            if (ingredient != null)
            {
                GameObject itemUI = new GameObject("IngredientImage");
                itemUI.transform.SetParent(gridLayout.transform);

                // Ajoute une image à l'objet
                Image imageComponent = itemUI.AddComponent<Image>();
                imageComponent.sprite = ingredient.ingredientSprite;

                // Ajoute un EventTrigger pour interagir avec l'élément
                EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

                // Événement PointerClick (clic)
                EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient));
                eventTrigger.triggers.Add(entryPointerClick);

                // Événement PointerEnter (passage de la souris)
                EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entryPointerEnter.callback.AddListener((data) => OnPointerEnter(ingredient));
                eventTrigger.triggers.Add(entryPointerEnter);

                // Événement PointerExit (retrait de la souris)
                EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                entryPointerExit.callback.AddListener((data) => OnPointerExit());
                eventTrigger.triggers.Add(entryPointerExit);
            }
        }
    }


    void OnIngredientClick(Ingredient ingredient)
    {
        currentIngredient = ingredient;

        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);

        Debug.Log($"Ingrédient sélectionné : {ingredient.name}");
    }


    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            // Assigne l'ingrédient à une main
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // Cache les boutons de sélection de la main
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Supprime visuellement l'ingrédient du canvas
            RemoveIngredientFromShelf(currentIngredient);
            currentIngredient = null; // Réinitialise l'ingrédient courant
        }
    }
    void RemoveIngredientFromShelf(Ingredient ingredient)
    {
        // Supprime l'ingrédient de la liste de données
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
                Debug.Log($"L'ingrédient {ingredient.name} a été retiré de l'interface.");
                return;
            }
        }

        Debug.LogWarning("Impossible de trouver l'ingrédient à supprimer dans le canvas.");
    }


    void CloseCanvas()
    {
        this.gameObject.SetActive(false);  // Cache l'UI sans la désactiver
        Time.timeScale = 1;  // Assurez-vous que le temps est rétabli à la normale

        // Assurez-vous que la caméra reste activée
        if (mainCamera != null && !mainCamera.gameObject.activeSelf)
        {
            mainCamera.gameObject.SetActive(true);
        }
    }

    void OnPointerEnter(Ingredient ingredient)
    {
        if (ingredient != null && itemDescriptionText != null)
        {
            itemDescriptionText.text = ingredient.description;
            Debug.Log($"Description affichée : {ingredient.description}");
        }
    }

    void OnPointerExit()
    {
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "";
            Debug.Log("Description effacée.");
        }
    }

}
