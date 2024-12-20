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

    private GameObject currentDraggedObject;


    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");
        containerLayer = LayerMask.NameToLayer("Container");

        worldSpaceCanvas.SetActive(false);
        handsFullCanvas.SetActive(false);

        leftHandDefaultColor = leftHandUI.color;
        rightHandDefaultColor = rightHandUI.color;
        if (stove == null)
        {
            stove = GameObject.FindWithTag("Stove");
            if (stove == null)
            {
                Debug.LogError("Aucun objet avec le tag 'Stove' trouvé !");
            }
        }
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
        if (isDragging && selectedObject != null && stove != null)
        {
            if (Vector3.Distance(selectedObject.transform.position, stove.transform.position) < 0.5f)
            {
                PlaceObjectOnStove(selectedObject, stove);
                StopDragObject(); // Arrêter le déplacement
            }
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
        if (obj == null) return;

        Vector3 newPosition;

        if (currentObject != null && currentObject.layer == containerLayer)
        {
            // Si un conteneur est détecté, placez l'objet dans ou au-dessus du conteneur
            if (currentObject.TryGetComponent<Collider>(out Collider containerCollider))
            {
                float containerTopY = containerCollider.bounds.max.y;
                newPosition = new Vector3(
                    containerCollider.bounds.center.x,
                    containerTopY + 0.1f, // Position légèrement au-dessus du conteneur
                    containerCollider.bounds.center.z
                );

                // Devenir enfant du conteneur pour conserver la hiérarchie
                obj.transform.SetParent(currentObject.transform);
            }
            else
            {
                Debug.LogWarning("Le conteneur n'a pas de collider, placement par défaut.");
                newPosition = currentObject.transform.position + Vector3.up * 0.1f;
            }
        }
        else
        {
            // Si aucun conteneur n'est détecté, placez l'objet près du joueur
            newPosition = player.position + player.forward * 0.05f + Vector3.up * 0.2f;
            obj.transform.SetParent(null); // Retirer l'objet de toute hiérarchie
        }

        // Positionner l'objet
        obj.transform.position = newPosition;
        obj.SetActive(true); // Réactiver l'objet après placement

        Debug.Log($"{obj.name} placé dans la scène.");
    }










    private void StartDragObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << containerLayer))
        {
            selectedObject = hit.collider.gameObject;
            currentDraggedObject = selectedObject;
            isDragging = true;

            // Calculer l'offset
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Vector3.Distance(Camera.main.transform.position, selectedObject.transform.position);
            offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(mousePosition);

            Debug.Log($"Objet sélectionné pour drag : {selectedObject.name}");
        }
        else
        {
            Debug.LogWarning("Aucun objet sélectionnable détecté !");
        }
    }




    private void StopDragObject()
    {
        if (currentDraggedObject == null)
        {
            Debug.LogWarning("Aucun objet en cours de déplacement.");
            return;
        }

        // Vérifier si l'objet est suffisamment proche du poêle
        if (stove != null && Vector3.Distance(currentDraggedObject.transform.position, stove.transform.position) < 0.5f)
        {
            PlaceObjectOnStove(currentDraggedObject, stove);
        }
        else
        {
            Debug.Log("Objet relâché en dehors du poêle.");
        }

        currentDraggedObject = null; // Réinitialisez après placement
        isDragging = false; // Réinitialiser l'état de drag
    }

    


        GameObject GetStove()
    {
        // Exemples d'approches :
        // Utilisez un raycast ou un déclencheur pour détecter le poêle
        return GameObject.FindWithTag("Stove"); // Ou une autre méthode de récupération.
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Container"))
        {
            Debug.Log($"Un conteneur a touché : {other.name}");

            // Si l'objet en collision avec le conteneur est un objet que nous pouvons mettre dedans
            if (currentObject != null && currentObject.layer == pickUpLayer)
            {
                PlaceObjectInContainer(currentObject, other.gameObject);
            }
        }
        else
        {
            Debug.Log($"Collision ignorée avec : {other.name}");
        }
    }


    private void PlaceObjectInContainer(GameObject obj, GameObject container)
    {
        if (obj == null || container == null)
        {
            Debug.LogError("Objet ou conteneur manquant !");
            return;
        }

        // Utiliser la méthode générique avec un offset spécifique pour le conteneur
        PlaceObject(obj, container, 0.1f); // Offset de 0.1 pour le placement au-dessus du conteneur
    }




    public void PlaceObjectOnStove(GameObject obj, GameObject stove)
    {
        if (obj == null || stove == null)
        {
            Debug.LogError("Objet ou poêle manquant !");
            return;
        }

        // Utiliser la méthode générique avec un offset spécifique pour le poêle
        PlaceObject(obj, stove, 0.1f); // Offset de 0.3 pour le placement au-dessus du poêle
    }


    private void PlaceObject(GameObject obj, GameObject parent, float yOffset = 0.1f)
    {
        if (obj == null || parent == null)
        {
            Debug.LogError("Objet ou parent manquant !");
            return;
        }

        // Vérifier si le parent a un collider pour ajuster la position
        Collider parentCollider = parent.GetComponent<Collider>();
        if (parentCollider != null)
        {
            // Calculer la position pour être juste au-dessus du parent
            Vector3 parentTopPosition = parentCollider.bounds.max;
            Vector3 newPosition = new Vector3(
                parentCollider.bounds.center.x,
                parentTopPosition.y + yOffset,
                parentCollider.bounds.center.z
            );

            obj.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Le parent n'a pas de collider, position par défaut appliquée.");
            obj.transform.position = parent.transform.position + Vector3.up * yOffset;
        }

        // Définir le parent comme transform.parent de l'objet
        obj.transform.SetParent(parent.transform);
        Debug.Log($"{obj.name} placé sur {parent.name}.");
    }


}



