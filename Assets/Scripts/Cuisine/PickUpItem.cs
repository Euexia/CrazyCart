using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    public Image leftHandUI;
    public Image rightHandUI;
    public GameObject worldSpaceCanvas;
    public Button leftHandButton;
    public Button rightHandButton;

    private GameObject currentObject = null;
    private GameObject leftHandObject = null;
    private GameObject rightHandObject = null;

    private int pickUpLayer;
    private int containerLayer;

    public Transform player;
    public float detectionRadius = 3f;
    public float maxAngle = 45f;
    public Vector3 canvasOffset = new Vector3(0f, 1f, 0f);

    private string currentAction = "";

    private Ingredient currentIngredient = null;

    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");
        containerLayer = LayerMask.NameToLayer("Container");

        worldSpaceCanvas.SetActive(false);
    }

    void Update()
    {
        DetectObjectOrContainerInProximity();
    }

    private void DetectObjectOrContainerInProximity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, (1 << pickUpLayer) | (1 << containerLayer));

        GameObject closestObject = null;
        float closestDistance = detectionRadius;

        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToObject = (hitCollider.transform.position - player.position).normalized;
            float angle = Vector3.Angle(player.forward, directionToObject);

            if (angle <= maxAngle)
            {
                float distance = Vector3.Distance(player.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hitCollider.gameObject;
                }
            }
        }

        if (closestObject != null)
        {
            if (closestObject.layer == pickUpLayer)
            {
                if (currentObject != closestObject)
                {
                    currentObject = closestObject;
                    currentAction = "pick";
                    DisplayCanvas(currentObject);
                }
            }
            else if (closestObject.layer == containerLayer)
            {
                currentObject = closestObject;
                currentAction = "drop";
                DisplayCanvas(currentObject);
            }
        }
        else
        {
            HideCanvas();
        }
    }

    private void DisplayCanvas(GameObject obj)
    {
        worldSpaceCanvas.transform.position = obj.transform.position + canvasOffset;
        worldSpaceCanvas.SetActive(true);
    }

    private void HideCanvas()
    {
        currentObject = null;
        worldSpaceCanvas.SetActive(false);
    }

    public void OnLeftHandButtonClick()
    {
        HandleHandSelection("left");
    }

    public void OnRightHandButtonClick()
    {
        HandleHandSelection("right");
    }

    public void HandleHandSelection(string hand)
    {
        Debug.Log("Sélection de la main: " + hand);

        if (currentAction == "pick")  // Si l'action est de ramasser un objet
        {
            if (hand == "left" && leftHandObject == currentObject)
            {
                Debug.LogWarning("L'objet est déjà dans la main gauche.");
                return;
            }
            else if (hand == "right" && rightHandObject == currentObject)
            {
                Debug.LogWarning("L'objet est déjà dans la main droite.");
                return;
            }

            AssignIngredientToHand(currentObject.GetComponent<IngredientHolder>().ingredientData, hand);
        }
        else if (currentAction == "drop")  // Si l'action est de déposer un objet
        {
            RemoveFromHand(hand);
        }
        else
        {
            Debug.LogWarning("Action inconnue : " + currentAction);
        }
    }

    private void PlaceInHand(string hand)
    {
        if (currentObject != null)
        {
            IngredientHolder ingredientHolder = currentObject.GetComponent<IngredientHolder>();
            if (ingredientHolder != null)
            {
                Ingredient ingredient = ingredientHolder.ingredientData;

                if (hand == "left" && leftHandUI.sprite == null)  // Si la main gauche est vide
                {
                    Debug.Log("Placer l'objet dans la main gauche : " + ingredient.ingredientName);
                    leftHandUI.sprite = ingredient.ingredientSprite;  // Afficher l'icône dans l'UI
                    leftHandObject = currentObject;  // Référence à l'objet dans la main gauche
                    currentObject.SetActive(false);  // Désactive l'objet dans le monde
                    Debug.Log("Objet désactivé dans le monde : " + currentObject.name);
                }
                else if (hand == "right" && rightHandUI.sprite == null)  // Si la main droite est vide
                {
                    Debug.Log("Placer l'objet dans la main droite : " + ingredient.ingredientName);
                    rightHandUI.sprite = ingredient.ingredientSprite;  // Afficher l'icône dans l'UI
                    rightHandObject = currentObject;  // Référence à l'objet dans la main droite
                    currentObject.SetActive(false);  // Désactive l'objet dans le monde
                    Debug.Log("Objet désactivé dans le monde : " + currentObject.name);
                }

                // Réinitialisation de l'objet courant et du canevas
                currentObject = null;
                worldSpaceCanvas.SetActive(false);
            }
        }
    }

    private void RemoveFromHand(string hand)
    {
        if (hand == "left" && leftHandObject != null)
        {
            Debug.Log("Retirer l'objet de la main gauche.");
            PlaceObjectInScene(leftHandObject);
            leftHandUI.sprite = null;
            leftHandObject = null;
        }
        else if (hand == "right" && rightHandObject != null)
        {
            Debug.Log("Retirer l'objet de la main droite.");
            PlaceObjectInScene(rightHandObject);
            rightHandUI.sprite = null;
            rightHandObject = null;
        }
        else
        {
            Debug.LogWarning("Aucun objet à retirer dans cette main.");
        }
    }

    private void PlaceObjectInScene(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Placer l'objet dans le monde à la position : " + player.position + " + " + Vector3.up);
            obj.transform.position = player.position + Vector3.up; // Place un peu au-dessus
            obj.SetActive(true);  // Réactive l'objet dans le monde
        }
        else
        {
            Debug.LogWarning("L'objet à placer dans le monde est null.");
        }
    }

    public void AssignIngredientToHand(Ingredient ingredient, string hand)
    {
        if (hand == "left")
        {
            if (rightHandObject == currentObject)
            {
                Debug.Log("L'objet est dans la main droite, on le retire.");
                RemoveFromHand("right");  // Retire l'objet de la main droite
            }

            if (leftHandUI.sprite == null)
            {
                Debug.Log($"Assigning {ingredient.ingredientName} to left hand.");
                leftHandUI.sprite = ingredient.ingredientSprite;
                leftHandObject = currentObject;
            }
            else
            {
                Debug.LogWarning("La main gauche est déjà occupée.");
            }
        }
        else if (hand == "right")
        {
            if (leftHandObject == currentObject)
            {
                Debug.Log("L'objet est dans la main gauche, on le retire.");
                RemoveFromHand("left");  // Retire l'objet de la main gauche
            }

            if (rightHandUI.sprite == null)
            {
                Debug.Log($"Assigning {ingredient.ingredientName} to right hand.");
                rightHandUI.sprite = ingredient.ingredientSprite;
                rightHandObject = currentObject;
            }
            else
            {
                Debug.LogWarning("La main droite est déjà occupée.");
            }
        }
    }
}
