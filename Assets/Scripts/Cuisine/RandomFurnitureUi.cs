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
    private List<Furniture.Shelf> generatedShelves = new List<Furniture.Shelf>();

    // Liste pour stocker les boutons d'étagère créés
    private List<GameObject> shelfButtons = new List<GameObject>();

    // Référence au Canvas
    public Canvas randomFurnitureCanvas;

    public void OpenRandomFurnitureCanvas()
    {
        randomFurnitureCanvas.gameObject.SetActive(true); // Affiche le Canvas
        ResetAndGenerateShelves(); // Appelle la méthode de réinitialisation et génération aléatoire
    }

    void Start()
    {
        // Vérifications des assignations dans l'inspecteur
        CheckInspectorReferences();

        // Désactiver le prefab des boutons d'étagère après usage
        if (shelfButtonPrefab != null)
        {
            shelfButtonPrefab.SetActive(false);
        }

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
        int shelfCount = Random.Range(1, 6); // Nombre d'étagères aléatoire entre 1 et 6
        for (int i = 0; i < shelfCount; i++)
        {
            Furniture.Shelf shelf = new Furniture.Shelf
            {
                shelfName = "Shelf " + (i + 1),
                ingredients = new List<Ingredient>()
            };

            // Nombre d'ingrédients aléatoire entre 1 et 6
            int ingredientCount = Random.Range(1, 6);

            // Récupérer les ingrédients, autorisant les répétitions
            List<Ingredient> randomIngredients = GetRandomIngredients(ingredientCount);

            foreach (var ingredient in randomIngredients)
            {
                shelf.ingredients.Add(ingredient);
            }

            generatedShelves.Add(shelf);
        }
    }

    List<Ingredient> GetRandomIngredients(int count)
    {
        List<Ingredient> randomIngredients = new List<Ingredient>();
        List<Ingredient> tempIngredients = new List<Ingredient>(allIngredients);

        for (int i = 0; i < count; i++)
        {
            if (tempIngredients.Count > 0)
            {
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

                // Ajouter un EventTrigger pour gérer les interactions
                EventTrigger eventTrigger = itemUI.AddComponent<EventTrigger>();

                // Événement PointerClick (clic)
                EventTrigger.Entry entryPointerClick = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entryPointerClick.callback.AddListener((data) => OnIngredientClick(ingredient));
                eventTrigger.triggers.Add(entryPointerClick);

                // Événement PointerEnter (survol)
                EventTrigger.Entry entryPointerEnter = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                entryPointerEnter.callback.AddListener((data) => OnPointerEnter(ingredient));
                eventTrigger.triggers.Add(entryPointerEnter);

                // Événement PointerExit (quitte le survol)
                EventTrigger.Entry entryPointerExit = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                entryPointerExit.callback.AddListener((data) => OnPointerExit());
                eventTrigger.triggers.Add(entryPointerExit);
            }
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
            pickUpObjectScript.AssignIngredientToHand(currentIngredient, hand);

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
                Debug.Log($"L'ingrédient {ingredient.name} a été retiré de l'interface.");
                return;
            }
        }

        Debug.LogWarning("Impossible de trouver l'ingrédient à supprimer dans l'interface.");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable appelé : Vérification de l'état du Canvas...");

        if (randomFurnitureCanvas == null)
        {
            Debug.LogError("Le Canvas randomFurnitureCanvas n'est pas assigné.");
            return;
        }

        // Vérifiez si le Canvas est activé avant d'appeler ResetAndGenerateShelves
        if (randomFurnitureCanvas.gameObject.activeInHierarchy)
        {
            ResetAndGenerateShelves(); // Appelle la méthode pour réinitialiser et générer les étagères aléatoires
        }
        else
        {
            Debug.LogWarning("Le Canvas n'est pas actif. Assurez-vous qu'il est bien activé avant de générer les étagères.");
        }

        Debug.Log("Vérification terminée.");
    }



    public void ResetAndGenerateShelves()
    {
        Debug.Log("Réinitialisation des étagères et génération aléatoire...");

        // Réinitialisation des étagères et des boutons
        ResetShelves();

        // Génération de nouvelles étagères de manière aléatoire
        GenerateRandomShelves();

        // Création des boutons d'étagère à partir des nouvelles étagères générées
        CreateShelfButtons();

        Debug.Log("Nouvelles étagères générées à l'ouverture du Canvas.");
    }


    void ResetShelves()
    {
        // Vérifier si les listes sont nulles
        if (generatedShelves != null)
        {
            generatedShelves.Clear();
        }

        // Désactiver les anciens boutons d'étagère avant de régénérer
        foreach (var button in shelfButtons)
        {
            Destroy(button);
        }
        shelfButtons.Clear();
    }

    // Vérification des références dans l'inspecteur
    void CheckInspectorReferences()
    {
        if (shelfButtonPrefab == null)
        {
            Debug.LogError("Le prefab de bouton d'étagère n'est pas assigné.");
        }
        if (shelfButtonsParent == null)
        {
            Debug.LogError("Le parent des boutons d'étagère n'est pas assigné.");
        }
        if (gridLayout == null)
        {
            Debug.LogError("Le GridLayout n'est pas assigné.");
        }
        if (itemDescriptionText == null)
        {
            Debug.LogError("Le TMP_Text de description d'élément n'est pas assigné.");
        }
    }

    void CloseCanvas()
    {
        randomFurnitureCanvas.gameObject.SetActive(false);
    }
}
