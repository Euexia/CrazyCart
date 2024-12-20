using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public Image leftHandUI;
    public Image rightHandUI;
    private Color leftHandDefaultColor;
    private Color rightHandDefaultColor;

    public GameObject worldSpaceCanvas;
    public GameObject handsFullCanvas;
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

    private bool isDragging = false;  // Pour savoir si l'objet est en train d'être déplacé
    private Vector3 offset;           // Offset pour garder la position relative de l'objet pendant le déplacement
    private Collider stoveCollider;
    private GameObject selectedObject;
    public GameObject stove; // Déclarez la référence à la stove


    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");
        containerLayer = LayerMask.NameToLayer("Container");

        worldSpaceCanvas.SetActive(false);
        handsFullCanvas.SetActive(false);

        leftHandDefaultColor = leftHandUI.color;
        rightHandDefaultColor = rightHandUI.color;
    }

    void Update()
    {
        DetectObjectOrContainerInProximity();

        if (isDragging && selectedObject != null)
        {
            // Déplacer l'objet en fonction de la position de la souris
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;  // Ajuster la distance pour le raycast
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.transform.position = worldPosition - offset;
        }

        // Vérifier le clic gauche pour commencer ou arrêter le déplacement
        if (Input.GetMouseButtonDown(0)) // Clic gauche
        {
            StartDragObject();
        }
        if (Input.GetMouseButtonUp(0)) // Relâcher le clic
        {
            StopDragObject();
        }
    }

    private void DetectObjectOrContainerInProximity()
    {
        // Détecter les objets dans la zone de proximité du joueur
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
        if (currentAction == "pick")
        {
            if (leftHandObject != null && rightHandObject != null)
            {
                ShowHandsFullCanvas();
                return;
            }

            AssignIngredientToHand(currentObject.GetComponent<IngredientHolder>().ingredientData, hand);
        }
        else if (currentAction == "drop")
        {
            RemoveFromHand(hand);
        }
    }

    private void ShowHandsFullCanvas()
    {
        handsFullCanvas.SetActive(true);
        Invoke("HideHandsFullCanvas", 2f);
    }

    private void HideHandsFullCanvas()
    {
        handsFullCanvas.SetActive(false);
    }

    public void AssignIngredientToHand(Ingredient ingredient, string hand)
    {
        if (hand == "left" && leftHandUI.sprite == null)
        {
            leftHandUI.sprite = ingredient.ingredientSprite;
            leftHandUI.color = Color.white;
            leftHandObject = currentObject;
            HandleObjectDeactivation(currentObject);
        }
        else if (hand == "right" && rightHandUI.sprite == null)
        {
            rightHandUI.sprite = ingredient.ingredientSprite;
            rightHandUI.color = Color.white;
            rightHandObject = currentObject;
            HandleObjectDeactivation(currentObject);
        }
        else
        {
            ShowHandsFullCanvas();
        }
    }

    private void HandleObjectDeactivation(GameObject obj)
    {
        if (obj == null) return;

        if (obj.GetComponent<RectTransform>() != null)
        {
            if (obj.CompareTag("PickUpItemUI") && obj.TryGetComponent<Graphic>(out Graphic graphic))
            {
                graphic.enabled = false;
            }
        }
        else
        {
            obj.SetActive(false);
        }
    }

    private void RemoveFromHand(string hand)
    {
        if (hand == "left" && leftHandObject != null)
        {
            PlaceObjectInScene(leftHandObject);
            leftHandUI.sprite = null;
            leftHandUI.color = leftHandDefaultColor; // Remet la couleur par défaut
            leftHandObject = null;
        }
        else if (hand == "right" && rightHandObject != null)
        {
            PlaceObjectInScene(rightHandObject);
            rightHandUI.sprite = null;
            rightHandUI.color = rightHandDefaultColor; // Remet la couleur par défaut
            rightHandObject = null;
        }
    }

    private void PlaceObjectInScene(GameObject obj)
    {
        if (obj != null)
        {
            // Placer l'objet vraiment très près du joueur, juste devant lui et légèrement au-dessus
            Vector3 newPosition = player.position + player.forward * 0.05f + Vector3.up * 0.2f; // Réduire la distance à 0.05f

            if (obj.layer == containerLayer)
            {
                // Si l'objet est dans un container, on le place sur le container
                if (currentObject != null && currentObject.layer == containerLayer)
                {
                    if (currentObject.TryGetComponent<Collider>(out Collider containerCollider))
                    {
                        float containerTopY = containerCollider.bounds.max.y;
                        newPosition = new Vector3(
                            containerCollider.bounds.center.x,
                            containerTopY + 0.05f, // Placer légèrement au-dessus du container
                            containerCollider.bounds.center.z
                        );
                    }
                }
            }

            // Placer l'objet très près du joueur
            obj.transform.position = newPosition;
            obj.SetActive(true); // Assurer que l'objet est activé après placement
        }
    }






    private void StartDragObject()
    {
        // Détecter si un objet est sous le curseur pour le déplacer
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.layer == containerLayer)
            {
                selectedObject = hit.collider.gameObject;
                isDragging = true;

                // Calculer l'offset entre la souris et l'objet sélectionné
                offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    private void StopDragObject()
    {
        // Arrêter le déplacement
        isDragging = false;
        selectedObject = null;

        // Vérifier si l'objet est sur la stove et le placer au-dessus si nécessaire
        if (selectedObject != null && selectedObject.CompareTag("Container")) // Ensure it's a container
        {
            // Placer l'objet au-dessus du poêle
            PlaceObjectOnStove(selectedObject, stove);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        // Vérifiez si l'objet entre en contact avec le poêle
        if (other.CompareTag("Stove")) // Assurez-vous que le poêle est étiqueté avec "Stove"
        {
            // Désactivez le drag de l'objet
            isDragging = false;
            selectedObject = null;

            // Assignez la stove à la variable stove
            stove = other.gameObject;

            // Placez l'objet sur le poêle
            PlaceObjectOnStove(currentObject, stove);
        }
    }



    private void PlaceObjectOnStove(GameObject obj, GameObject stove)
    {
        if (obj != null && stove != null)
        {
            Collider stoveCol = stove.GetComponent<Collider>();
            if (stoveCol != null)
            {
                // Get the position above the stove
                Vector3 stovePosition = stoveCol.bounds.center;
                float stoveTopY = stoveCol.bounds.max.y;
                float objOffsetY = obj.GetComponent<Collider>().bounds.extents.y;

                // Position the object just above the stove
                obj.transform.position = new Vector3(stovePosition.x, stoveTopY + objOffsetY + 0.05f, stovePosition.z);
                obj.transform.SetParent(stove.transform); // Set the stove as the parent of the container
            }
        }
    }



}



