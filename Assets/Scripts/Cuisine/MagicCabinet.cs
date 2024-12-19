using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

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
    public int maxItemsPerShelf = 6; // Maximum d'objets par étagère

    public List<Ingredient> possibleIngredients; // Liste des objets Ingredient possibles

    private Ingredient currentIngredient;
    private List<Shelf> generatedShelves; // Liste des étagères générées

    public GameObject magicCabinetCanvas; // Canvas spécifique au Magic Cabinet
    public IngredientHandSelectionHandler ingredientHandSelectionHandler; // Référence au script IngredientHandSelectionHandler


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
        // Initialisation des boutons de main
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);

        closeCanvasButton.onClick.AddListener(CloseCanvas);
    }

    void GenerateShelves()
    {
        generatedShelves = new List<Shelf>();

        int numberOfShelves = Random.Range(minShelves, maxShelves + 1);
        Debug.Log($"Number of shelves: {numberOfShelves}"); // Débogage

        for (int i = 0; i < numberOfShelves; i++)
        {
            Shelf shelf = new Shelf
            {
                shelfName = $"Shelf {i + 1}"
            };

            int numberOfItems = Random.Range(minItemsPerShelf, maxItemsPerShelf + 1);
            Debug.Log($"Shelf {i + 1} will contain {numberOfItems} items"); // Débogage

            for (int j = 0; j < numberOfItems; j++)
            {
                // Choisir un Ingredient aléatoire depuis possibleIngredients
                Ingredient ingredient = possibleIngredients[Random.Range(0, possibleIngredients.Count)];
                Debug.Log($"Added ingredient: {ingredient.ingredientName}"); // Débogage

                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }

    void CloseCanvas()
    {
        // Ferme le Magic Cabinet et réactive la caméra principale
        magicCabinetCanvas.SetActive(false);
        Time.timeScale = 1;
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }

        // Désactivation des boutons de main lors de la fermeture du cabinet
        leftHandButton.SetActive(false);
        rightHandButton.SetActive(false);
    }

    void CreateShelfButtons()
    {
        // Nettoyer les anciens boutons
        foreach (Transform child in shelfButtonsParent)
        {
            Destroy(child.gameObject);
        }

        // Créer des boutons pour chaque étagère
        foreach (var shelf in generatedShelves)
        {
            // Crée un bouton pour chaque étagère à partir du prefab
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);

            // Définir le nom de l'étagère sur le bouton
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;

            // Ajouter un listener pour afficher les éléments de l'étagère
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));

            // Assurez-vous que le bouton est activé
            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true; // Réactive le bouton
                buttonComponent.enabled = true; // Assure que le bouton est activé
            }

            // Assurez-vous que l'image du bouton est activée
            Image imageComponent = button.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.enabled = true; // Réactive l'image du bouton si nécessaire
            }

            // Vérifier si un CanvasGroup est présent dans un parent
            CanvasGroup canvasGroup = button.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            // Activer l'objet entier du bouton dans la hiérarchie
            button.SetActive(true);
        }
    }

    void ShowShelfItems(Shelf shelf)
    {
        // Nettoie les anciens objets
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        if (shelf.ingredients.Count == 0)
        {
            Debug.LogWarning($"No ingredients in shelf {shelf.shelfName}"); // Débogage
        }

        foreach (var ingredient in shelf.ingredients)
        {
            // Crée un nouveau GameObject pour l'élément
            GameObject itemUI = new GameObject("IngredientImage");

            // Assure-toi que l'élément a un RectTransform pour se positionner correctement dans le GridLayout
            RectTransform rectTransform = itemUI.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);  // Définir une taille pour l'élément, ajustez selon vos besoins
            itemUI.transform.SetParent(gridLayout.transform, false); // Ne pas forcer la mise à l'échelle du parent

            // Ajoute un composant Image
            Image imageComponent = itemUI.AddComponent<Image>();
            imageComponent.sprite = ingredient.ingredientSprite; // Utilise le sprite de l'Ingredient

            // Assurez-vous que l'image est visible
            imageComponent.color = new Color(1, 1, 1, 1);  // Vérifier que l'alpha est correct

            // Ajoute un EventTrigger pour détecter les clics
            EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

            EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient, itemUI));  // Passer itemUI ici

            eventTrigger.triggers.Add(entryPointerClick);

            // Ajoutez un EventTrigger pour afficher la description quand la souris survole l'élément
            EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryPointerEnter.callback.AddListener((data) => OnPointerEnter(ingredient));
            eventTrigger.triggers.Add(entryPointerEnter);

            // Ajoutez un EventTrigger pour effacer la description quand la souris quitte l'élément
            EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entryPointerExit.callback.AddListener((data) => OnPointerExit());
            eventTrigger.triggers.Add(entryPointerExit);

            Debug.Log($"Created ingredient UI for {ingredient.ingredientName}"); // Débogage
        }
    }

    void OnPointerEnter(Ingredient ingredient)
    {
        // Afficher la description de l'ingrédient dans le texte
        itemDescriptionText.text = ingredient.description;
    }

    void OnPointerExit()
    {
        // Effacer la description lorsqu'on quitte l'élément
        itemDescriptionText.text = "";
    }
    void OnIngredientClick(Ingredient ingredient, GameObject ingredientUI)
    {
        // Vérifiez si l'ingrédient est null
        if (ingredient == null)
        {
            Debug.LogError("Ingredient is null in OnIngredientClick");
            return;
        }

        // Vérifiez si ingredientHandSelectionHandler est null
        if (ingredientHandSelectionHandler == null)
        {
            Debug.LogError("ingredientHandSelectionHandler is not assigned in MagicCabinet");
            return;
        }

        // Si tout est correct, afficher un message de débogage
        Debug.Log($"Ingredient clicked: {ingredient.ingredientName}");

        // Mettez à jour l'ingrédient actuel
        currentIngredient = ingredient;

        // Afficher les boutons de main
        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);
        Debug.Log("Left and Right Hand buttons activated.");

        // Mettre à jour le handler avec l'ingrédient sélectionné
        ingredientHandSelectionHandler.SetCurrentIngredient(ingredient, ingredientUI);
        Debug.Log("Ingredient assigned to IngredientHandSelectionHandler.");
    }



    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            // Assigner l'ingrédient à la main sélectionnée
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // Désactiver les boutons de main après la sélection
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Retirer l'ingrédient de l'étagère et du canvas
            RemoveIngredientFromShelf(currentIngredient);
            currentIngredient = null; // Réinitialisation de l'ingrédient
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

    public void OpenMagicCabinetCanvas()
    {
        magicCabinetCanvas.SetActive(true); // Active le Magic Cabinet Canvas
        leftHandButton.SetActive(false); // Désactive les boutons de main à l'ouverture
        rightHandButton.SetActive(false);

        Debug.Log("Magic Cabinet Canvas is now active!");

        // Génération des étagères et ingrédients aléatoires
        GenerateShelves();
        CreateShelfButtons();
    }


    public void CloseMagicCabinetCanvas()
    {
        if (magicCabinetCanvas != null)
        {
            magicCabinetCanvas.SetActive(false); // Désactive le Magic Cabinet Canvas
        }
    }



}