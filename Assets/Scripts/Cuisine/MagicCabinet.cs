using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MagicCabinet : MonoBehaviour
{
    public GameObject shelfButtonPrefab; // Bouton représentant une étagère
    public Transform shelfButtonsParent; // Parent des boutons d'étagères
    public GridLayoutGroup gridLayout; // Grille pour afficher les objets
    public TMP_Text itemDescriptionText; // Texte pour décrire les objets
    public Button closeCanvasButton; // Bouton pour fermer le placard
    public Camera mainCamera; // Caméra principale
    public PickUpObject pickUpObjectScript; // Script pour gérer la sélection d'objets

    public GameObject leftHandButton; // Bouton pour sélectionner la main gauche
    public GameObject rightHandButton; // Bouton pour sélectionner la main droite

    public int minShelves = 2; // Nombre minimum d'étagères
    public int maxShelves = 5; // Nombre maximum d'étagères
    public int minItemsPerShelf = 3; // Minimum d'objets par étagère
    public int maxItemsPerShelf = 8; // Maximum d'objets par étagère

    public List<Ingredient> possibleIngredients; // Liste des objets Ingredient possibles

    private Ingredient currentIngredient;
    private List<Shelf> generatedShelves; // Liste des étagères générées



    [System.Serializable]
    public class Shelf
    {
        public string shelfName;
        public List<Ingredient> ingredients = new List<Ingredient>();
    }

    [System.Serializable]
    public class MagicCabinetIngredient  
    {
        public string ingredientName;
        public Sprite ingredientSprite;
    }


    void Start()
    {
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
        closeCanvasButton.onClick.AddListener(CloseCanvas);

        leftHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("left"));
        rightHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("right"));
    }

    public void OpenMagicCabinet()
    {
        GenerateShelves();
        CreateShelfButtons();
        this.gameObject.SetActive(true);
        Time.timeScale = 0;
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
        }
    }

    void GenerateShelves()
    {
        generatedShelves = new List<Shelf>();

        int numberOfShelves = Random.Range(minShelves, maxShelves + 1);
        for (int i = 0; i < numberOfShelves; i++)
        {
            Shelf shelf = new Shelf
            {
                shelfName = $"Shelf {i + 1}"
            };

            int numberOfItems = Random.Range(minItemsPerShelf, maxItemsPerShelf + 1);
            for (int j = 0; j < numberOfItems; j++)
            {
                // Choisir un Ingredient aléatoire depuis possibleIngredients
                Ingredient ingredient = possibleIngredients[Random.Range(0, possibleIngredients.Count)];

                // Ajoutez l'ingrédient à l'étagère
                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }


    void CreateShelfButtons()
    {
        foreach (Transform child in shelfButtonsParent)
        {
            Destroy(child.gameObject); // Nettoie les anciens boutons
        }

        foreach (var shelf in generatedShelves)
        {
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));
        }
    }

    void ShowShelfItems(Shelf shelf)
    {
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject); // Nettoie les anciens objets
        }

        foreach (var ingredient in shelf.ingredients)
        {
            GameObject itemUI = new GameObject("IngredientImage");
            itemUI.transform.SetParent(gridLayout.transform);

            Image imageComponent = itemUI.AddComponent<Image>();
            imageComponent.sprite = ingredient.ingredientSprite; // Utilisez le sprite de l'Ingredient

            EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

            EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient));

            eventTrigger.triggers.Add(entryPointerClick);
        }
    }


    void OnIngredientClick(Ingredient ingredient)
    {
        currentIngredient = ingredient;

        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);

        Debug.Log($"Ingrédient sélectionné : {ingredient.ingredientName}");
    }

    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);  // L'Ingredient est un ScriptableObject

            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            RemoveIngredientFromShelf(currentIngredient);
            currentIngredient = null;
        }
    }

    void RemoveIngredientFromShelf(Ingredient ingredient)
    {
        foreach (var shelf in generatedShelves)
        {
            if (shelf.ingredients.Contains(ingredient))
            {
                shelf.ingredients.Remove(ingredient);
                break;
            }
        }

        foreach (Transform child in gridLayout.transform)
        {
            Image imageComponent = child.GetComponent<Image>();

            if (imageComponent != null && imageComponent.sprite == ingredient.ingredientSprite)
            {
                Destroy(child.gameObject);
                return;
            }
        }
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
