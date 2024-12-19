using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class RandomFurnitureUI : MonoBehaviour
{
    public GameObject shelfButtonPrefab;
    public Transform shelfButtonsParent;
    public GridLayoutGroup gridLayout;
    public TMP_Text itemDescriptionText;
    public Button closeCanvasButton;
    public Camera mainCamera;
    public PickUpObject pickUpObjectScript;

    public GameObject leftHandButton;
    public GameObject rightHandButton;

    // Liste d'ingrédients disponibles pour les meubles random
    public List<Ingredient> allIngredients;

    private Ingredient currentIngredient;
    private List<Furniture.Shelf> generatedShelves;

    // Liste pour stocker les boutons d'étagère créés
    private List<GameObject> shelfButtons = new List<GameObject>();

    // Référence au Canvas
    public Canvas randomFurnitureCanvas;

    void Start()
    {
        // Initialisation des éléments
        generatedShelves = new List<Furniture.Shelf>();

        // Générer les étagères et ingrédients aléatoires
        GenerateRandomShelves();

        // Créer les boutons pour chaque étagère
        CreateShelfButtons();

        // Configurer les boutons de main
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);

        // Assigner l'action pour le bouton de fermeture du Canvas
        closeCanvasButton.onClick.AddListener(CloseCanvas);

        leftHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("left"));
        rightHandButton.GetComponent<Button>().onClick.AddListener(() => HandleHandSelection("right"));
    }

    void GenerateRandomShelves()
    {
        int shelfCount = Random.Range(1, 7);  // Nombre d'étagères aléatoire entre 1 et 6
        for (int i = 0; i < shelfCount; i++)
        {
            Furniture.Shelf shelf = new Furniture.Shelf();
            shelf.shelfName = "Shelf " + (i + 1);
            shelf.ingredients = new List<Ingredient>();

            // Nombre d'ingrédients aléatoire entre 1 et 6
            int ingredientCount = Random.Range(1, 7);

            // Récupérer les ingrédients, autorisant les répétitions
            List<Ingredient> randomIngredients = GetRandomIngredients(ingredientCount);

            foreach (var ingredient in randomIngredients)
            {
                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }

    // Fonction pour récupérer des ingrédients aléatoires (avec répétitions possibles)
    List<Ingredient> GetRandomIngredients(int count)
    {
        List<Ingredient> randomIngredients = new List<Ingredient>();

        // On ne modifie plus la liste temporaire d'ingrédients
        List<Ingredient> tempIngredients = new List<Ingredient>(allIngredients);

        for (int i = 0; i < count; i++)
        {
            if (tempIngredients.Count > 0)
            {
                // Choisir un ingrédient au hasard sans l'enlever de la liste
                int randomIndex = Random.Range(0, tempIngredients.Count);
                randomIngredients.Add(tempIngredients[randomIndex]);
            }
        }

        return randomIngredients;
    }

    void CreateShelfButtons()
    {
        foreach (var shelf in generatedShelves)
        {
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));

            // Ajouter le bouton à la liste des boutons
            shelfButtons.Add(button);
        }
    }

    void ShowShelfItems(Furniture.Shelf shelf)
    {
        // Nettoyer les éléments précédemment affichés dans la grille
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        // Ajouter les ingrédients de l'étagère à la grille
        foreach (var ingredient in shelf.ingredients)
        {
            if (ingredient != null)
            {
                GameObject itemUI = new GameObject("IngredientImage");
                itemUI.transform.SetParent(gridLayout.transform);

                Image imageComponent = itemUI.AddComponent<Image>();
                imageComponent.sprite = ingredient.ingredientSprite;

                // Ajouter un EventTrigger pour gérer les clics
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

        Debug.Log($"Ingrédient sélectionné : {ingredient.name}");
    }

    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            // Assigner l'ingrédient à une main
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // Cacher les boutons de sélection de main
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Supprimer visuellement l'ingrédient de l'interface
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

        // Retirer l'ingrédient visuellement de l'interface
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

        Debug.LogWarning("Impossible de trouver l'ingrédient à supprimer dans l'interface.");
    }

    // Modifier la fonction pour fermer uniquement le Canvas
    void CloseCanvas()
    {
        // Désactiver le Canvas
        if (randomFurnitureCanvas != null)
        {
            randomFurnitureCanvas.gameObject.SetActive(false); // Désactive seulement le Canvas
        }

        // Désactiver tous les boutons d'étagère
        foreach (var button in shelfButtons)
        {
            Destroy(button);  // Détruire chaque bouton d'étagère
        }
        shelfButtons.Clear();  // Vider la liste des boutons

        Time.timeScale = 1;

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
    }
}
