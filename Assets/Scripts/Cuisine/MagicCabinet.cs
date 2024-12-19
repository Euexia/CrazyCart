using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class MagicCabinet : MonoBehaviour
{
    public GameObject shelfButtonPrefab; // Bouton repr�sentant une �tag�re
    public Transform shelfButtonsParent; // Parent des boutons d'�tag�res
    public GridLayoutGroup gridLayout; // Grille pour afficher les objets
    public TMP_Text itemDescriptionText; // Texte pour d�crire les objets
    public Button closeCanvasButton; // Bouton pour fermer le placard
    public Camera mainCamera; // Cam�ra principale
    public PickUpObject pickUpObjectScript; // Script pour g�rer la s�lection d'objets

    public GameObject leftHandButton; // Bouton pour s�lectionner la main gauche
    public GameObject rightHandButton; // Bouton pour s�lectionner la main droite

    public int minShelves = 2; // Nombre minimum d'�tag�res
    public int maxShelves = 5; // Nombre maximum d'�tag�res
    public int minItemsPerShelf = 3; // Minimum d'objets par �tag�re
    public int maxItemsPerShelf = 6; // Maximum d'objets par �tag�re

    public List<Ingredient> possibleIngredients; // Liste des objets Ingredient possibles

    private Ingredient currentIngredient;
    private List<Shelf> generatedShelves; // Liste des �tag�res g�n�r�es

    public GameObject magicCabinetCanvas; // Canvas sp�cifique au Magic Cabinet
    public IngredientHandSelectionHandler ingredientHandSelectionHandler; // R�f�rence au script IngredientHandSelectionHandler


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
        Debug.Log($"Number of shelves: {numberOfShelves}"); // D�bogage

        for (int i = 0; i < numberOfShelves; i++)
        {
            Shelf shelf = new Shelf
            {
                shelfName = $"Shelf {i + 1}"
            };

            int numberOfItems = Random.Range(minItemsPerShelf, maxItemsPerShelf + 1);
            Debug.Log($"Shelf {i + 1} will contain {numberOfItems} items"); // D�bogage

            for (int j = 0; j < numberOfItems; j++)
            {
                // Choisir un Ingredient al�atoire depuis possibleIngredients
                Ingredient ingredient = possibleIngredients[Random.Range(0, possibleIngredients.Count)];
                Debug.Log($"Added ingredient: {ingredient.ingredientName}"); // D�bogage

                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }

    void CloseCanvas()
    {
        // Ferme le Magic Cabinet et r�active la cam�ra principale
        magicCabinetCanvas.SetActive(false);
        Time.timeScale = 1;
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
        }

        // D�sactivation des boutons de main lors de la fermeture du cabinet
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

        // Cr�er des boutons pour chaque �tag�re
        foreach (var shelf in generatedShelves)
        {
            // Cr�e un bouton pour chaque �tag�re � partir du prefab
            GameObject button = Instantiate(shelfButtonPrefab, shelfButtonsParent);

            // D�finir le nom de l'�tag�re sur le bouton
            button.GetComponentInChildren<TMP_Text>().text = shelf.shelfName;

            // Ajouter un listener pour afficher les �l�ments de l'�tag�re
            button.GetComponent<Button>().onClick.AddListener(() => ShowShelfItems(shelf));

            // Assurez-vous que le bouton est activ�
            Button buttonComponent = button.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true; // R�active le bouton
                buttonComponent.enabled = true; // Assure que le bouton est activ�
            }

            // Assurez-vous que l'image du bouton est activ�e
            Image imageComponent = button.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.enabled = true; // R�active l'image du bouton si n�cessaire
            }

            // V�rifier si un CanvasGroup est pr�sent dans un parent
            CanvasGroup canvasGroup = button.GetComponentInParent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = true;
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }

            // Activer l'objet entier du bouton dans la hi�rarchie
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
            Debug.LogWarning($"No ingredients in shelf {shelf.shelfName}"); // D�bogage
        }

        foreach (var ingredient in shelf.ingredients)
        {
            // Cr�e un nouveau GameObject pour l'�l�ment
            GameObject itemUI = new GameObject("IngredientImage");

            // Assure-toi que l'�l�ment a un RectTransform pour se positionner correctement dans le GridLayout
            RectTransform rectTransform = itemUI.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);  // D�finir une taille pour l'�l�ment, ajustez selon vos besoins
            itemUI.transform.SetParent(gridLayout.transform, false); // Ne pas forcer la mise � l'�chelle du parent

            // Ajoute un composant Image
            Image imageComponent = itemUI.AddComponent<Image>();
            imageComponent.sprite = ingredient.ingredientSprite; // Utilise le sprite de l'Ingredient

            // Assurez-vous que l'image est visible
            imageComponent.color = new Color(1, 1, 1, 1);  // V�rifier que l'alpha est correct

            // Ajoute un EventTrigger pour d�tecter les clics
            EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

            EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient, itemUI));  // Passer itemUI ici

            eventTrigger.triggers.Add(entryPointerClick);

            // Ajoutez un EventTrigger pour afficher la description quand la souris survole l'�l�ment
            EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entryPointerEnter.callback.AddListener((data) => OnPointerEnter(ingredient));
            eventTrigger.triggers.Add(entryPointerEnter);

            // Ajoutez un EventTrigger pour effacer la description quand la souris quitte l'�l�ment
            EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entryPointerExit.callback.AddListener((data) => OnPointerExit());
            eventTrigger.triggers.Add(entryPointerExit);

            Debug.Log($"Created ingredient UI for {ingredient.ingredientName}"); // D�bogage
        }
    }

    void OnPointerEnter(Ingredient ingredient)
    {
        // Afficher la description de l'ingr�dient dans le texte
        itemDescriptionText.text = ingredient.description;
    }

    void OnPointerExit()
    {
        // Effacer la description lorsqu'on quitte l'�l�ment
        itemDescriptionText.text = "";
    }
    void OnIngredientClick(Ingredient ingredient, GameObject ingredientUI)
    {
        // V�rifiez si l'ingr�dient est null
        if (ingredient == null)
        {
            Debug.LogError("Ingredient is null in OnIngredientClick");
            return;
        }

        // V�rifiez si ingredientHandSelectionHandler est null
        if (ingredientHandSelectionHandler == null)
        {
            Debug.LogError("ingredientHandSelectionHandler is not assigned in MagicCabinet");
            return;
        }

        // Si tout est correct, afficher un message de d�bogage
        Debug.Log($"Ingredient clicked: {ingredient.ingredientName}");

        // Mettez � jour l'ingr�dient actuel
        currentIngredient = ingredient;

        // Afficher les boutons de main
        leftHandButton.SetActive(true);
        rightHandButton.SetActive(true);
        Debug.Log("Left and Right Hand buttons activated.");

        // Mettre � jour le handler avec l'ingr�dient s�lectionn�
        ingredientHandSelectionHandler.SetCurrentIngredient(ingredient, ingredientUI);
        Debug.Log("Ingredient assigned to IngredientHandSelectionHandler.");
    }



    void HandleHandSelection(string hand)
    {
        if (currentIngredient != null)
        {
            // Assigner l'ingr�dient � la main s�lectionn�e
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

            // D�sactiver les boutons de main apr�s la s�lection
            leftHandButton.SetActive(false);
            rightHandButton.SetActive(false);

            // Retirer l'ingr�dient de l'�tag�re et du canvas
            RemoveIngredientFromShelf(currentIngredient);
            currentIngredient = null; // R�initialisation de l'ingr�dient
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
        leftHandButton.SetActive(false); // D�sactive les boutons de main � l'ouverture
        rightHandButton.SetActive(false);

        Debug.Log("Magic Cabinet Canvas is now active!");

        // G�n�ration des �tag�res et ingr�dients al�atoires
        GenerateShelves();
        CreateShelfButtons();
    }


    public void CloseMagicCabinetCanvas()
    {
        if (magicCabinetCanvas != null)
        {
            magicCabinetCanvas.SetActive(false); // D�sactive le Magic Cabinet Canvas
        }
    }



}