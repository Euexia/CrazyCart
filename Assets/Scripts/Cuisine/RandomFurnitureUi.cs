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

    // Liste d'ingr�dients disponibles pour les meubles random
    public List<Ingredient> allIngredients;

    private Ingredient currentIngredient;
    private List<Furniture.Shelf> generatedShelves;

    // Liste pour stocker les boutons d'�tag�re cr��s
    private List<GameObject> shelfButtons = new List<GameObject>();

    // R�f�rence au Canvas
    public Canvas randomFurnitureCanvas;

    void Start()
    {
        // Initialisation des �l�ments
        generatedShelves = new List<Furniture.Shelf>();

        // G�n�rer les �tag�res et ingr�dients al�atoires
        GenerateRandomShelves();

        // Cr�er les boutons pour chaque �tag�re
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
        int shelfCount = Random.Range(1, 7);  // Nombre d'�tag�res al�atoire entre 1 et 6
        for (int i = 0; i < shelfCount; i++)
        {
            Furniture.Shelf shelf = new Furniture.Shelf();
            shelf.shelfName = "Shelf " + (i + 1);
            shelf.ingredients = new List<Ingredient>();

            // Nombre d'ingr�dients al�atoire entre 1 et 6
            int ingredientCount = Random.Range(1, 7);

            // R�cup�rer les ingr�dients, autorisant les r�p�titions
            List<Ingredient> randomIngredients = GetRandomIngredients(ingredientCount);

            foreach (var ingredient in randomIngredients)
            {
                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }

    // Fonction pour r�cup�rer des ingr�dients al�atoires (avec r�p�titions possibles)
    List<Ingredient> GetRandomIngredients(int count)
    {
        List<Ingredient> randomIngredients = new List<Ingredient>();

        // On ne modifie plus la liste temporaire d'ingr�dients
        List<Ingredient> tempIngredients = new List<Ingredient>(allIngredients);

        for (int i = 0; i < count; i++)
        {
            if (tempIngredients.Count > 0)
            {
                // Choisir un ingr�dient au hasard sans l'enlever de la liste
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

            // Ajouter le bouton � la liste des boutons
            shelfButtons.Add(button);
        }
    }

    void ShowShelfItems(Furniture.Shelf shelf)
    {
        // Nettoyer les �l�ments pr�c�demment affich�s dans la grille
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        // Ajouter les ingr�dients de l'�tag�re � la grille
        foreach (var ingredient in shelf.ingredients)
        {
            if (ingredient != null)
            {
                GameObject itemUI = new GameObject("IngredientImage");
                itemUI.transform.SetParent(gridLayout.transform);

                Image imageComponent = itemUI.AddComponent<Image>();
                imageComponent.sprite = ingredient.ingredientSprite;

                // Ajouter un EventTrigger pour g�rer les clics
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
            // Assigner l'ingr�dient � une main
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // Cacher les boutons de s�lection de main
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Supprimer visuellement l'ingr�dient de l'interface
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

        // Retirer l'ingr�dient visuellement de l'interface
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

        Debug.LogWarning("Impossible de trouver l'ingr�dient � supprimer dans l'interface.");
    }

    // Modifier la fonction pour fermer uniquement le Canvas
    void CloseCanvas()
    {
        // D�sactiver le Canvas
        if (randomFurnitureCanvas != null)
        {
            randomFurnitureCanvas.gameObject.SetActive(false); // D�sactive seulement le Canvas
        }

        // D�sactiver tous les boutons d'�tag�re
        foreach (var button in shelfButtons)
        {
            Destroy(button);  // D�truire chaque bouton d'�tag�re
        }
        shelfButtons.Clear();  // Vider la liste des boutons

        Time.timeScale = 1;

        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }
    }
}
