using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    public Image leftHandUI;
    public Image rightHandUI;
    public GameObject worldSpaceCanvas;  // Canvas pour interaction contextuelle
    public GameObject handsFullCanvas;  // Canvas pour signaler que les mains sont pleines
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
        handsFullCanvas.SetActive(false);  // Assurez-vous que le canvas est d�sactiv� au d�marrage
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
        Debug.Log("S�lection de la main: " + hand);

        if (currentAction == "pick")  // Si l'action est de ramasser un objet
        {
            // V�rifie si les deux mains sont d�j� occup�es
            if (leftHandObject != null && rightHandObject != null)
            {
                Debug.LogWarning("Les deux mains sont pleines !");
                ShowHandsFullCanvas();
                return;
            }

            AssignIngredientToHand(currentObject.GetComponent<IngredientHolder>().ingredientData, hand);
        }
        else if (currentAction == "drop")  // Si l'action est de d�poser un objet
        {
            RemoveFromHand(hand);
        }
        else
        {
            Debug.LogWarning("Action inconnue : " + currentAction);
        }
    }

    private void ShowHandsFullCanvas()
    {
        handsFullCanvas.SetActive(true);  // Active le canvas temporairement
        Invoke("HideHandsFullCanvas", 2f);  // Le d�sactive apr�s 2 secondes
    }

    private void HideHandsFullCanvas()
    {
        handsFullCanvas.SetActive(false);
    }

    public void AssignIngredientToHand(Ingredient ingredient, string hand)
    {
        if (hand == "left")
        {
            if (leftHandUI.sprite == null)  // V�rifie si la main gauche est vide
            {
                Debug.Log($"Assigning {ingredient.ingredientName} to left hand.");
                leftHandUI.sprite = ingredient.ingredientSprite;
                leftHandObject = currentObject;

                HandleObjectDeactivation(currentObject); // G�re la d�sactivation de l'objet
            }
            else
            {
                Debug.LogWarning("La main gauche est d�j� occup�e.");
            }
        }
        else if (hand == "right")
        {
            if (rightHandUI.sprite == null)  // V�rifie si la main droite est vide
            {
                Debug.Log($"Assigning {ingredient.ingredientName} to right hand.");
                rightHandUI.sprite = ingredient.ingredientSprite;
                rightHandObject = currentObject;

                HandleObjectDeactivation(currentObject); // G�re la d�sactivation de l'objet
            }
            else
            {
                Debug.LogWarning("La main droite est d�j� occup�e.");
            }
        }
    }
    private void HandleObjectDeactivation(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("L'objet � d�sactiver est null.");
            return;
        }

        // V�rifie si l'objet fait partie d'une UI (Canvas)
        if (obj.GetComponent<RectTransform>() != null) // Appartient � une interface utilisateur
        {
            Debug.Log("L'objet fait partie d'un Canvas UI, d�sactivation des �l�ments graphiques.");

            // D�sactive uniquement si c'est un objet r�cup�rable, et pas le canvas "main remplie"
            if (obj.CompareTag("PickUpItemUI")) // Taguez les objets r�cup�rables dans l'UI
            {
                if (obj.TryGetComponent<Graphic>(out Graphic graphic))
                {
                    graphic.enabled = false; // D�sactive uniquement le rendu de cet �l�ment
                }
                else
                {
                    Debug.LogWarning("L'objet UI n'a pas de composant graphique.");
                }
            }
            else
            {
                Debug.Log("Cet objet UI n'est pas un �l�ment r�cup�rable, aucune action prise.");
            }
        }
        else // Objet de la sc�ne 3D
        {
            Debug.Log("L'objet fait partie de la sc�ne, d�sactivation avec SetActive.");
            obj.SetActive(false); // D�sactive l'objet complet
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
            Debug.LogWarning("Aucun objet � retirer dans cette main.");
        }
    }

    private void PlaceObjectInScene(GameObject obj)
    {
        if (obj != null)
        {
            // V�rifie si l'objet courant est un conteneur
            if (currentObject != null && currentObject.layer == containerLayer)
            {
                Debug.Log("Placer l'objet dynamiquement au-dessus du conteneur d�tect�.");

                // R�cup�re les colliders du conteneur et de l'objet
                if (currentObject.TryGetComponent<Collider>(out Collider containerCollider))
                {
                    if (obj.TryGetComponent<Collider>(out Collider objectCollider))
                    {
                        // Calcule la position du sommet du conteneur
                        float containerTopY = containerCollider.bounds.max.y; // Bord sup�rieur du conteneur
                        float objectHeight = objectCollider.bounds.extents.y * 2; // Hauteur totale de l'objet
                        float objectOffsetY = objectCollider.bounds.extents.y; // D�calage vertical pour que l'objet repose correctement

                        // Place l'objet au sommet du conteneur
                        obj.transform.position = new Vector3(
                            containerCollider.bounds.center.x,   // Aligne l'objet horizontalement
                            containerTopY + objectOffsetY + 0.05f, // Ajuste la hauteur
                            containerCollider.bounds.center.z    // Aligne l'objet horizontalement
                        );
                    }
                    else
                    {
                        Debug.LogWarning("L'objet � placer n'a pas de collider. Placement par d�faut.");
                        obj.transform.position = containerCollider.bounds.center + Vector3.up * (containerCollider.bounds.extents.y + 0.1f);
                    }
                }
                else
                {
                    Debug.LogWarning("Le conteneur n'a pas de collider, utilisation de sa position.");
                    obj.transform.position = currentObject.transform.position + Vector3.up * 0.1f;
                }
            }
            else
            {
                Debug.Log("Aucun conteneur d�tect�, placer l'objet pr�s du joueur.");

                // Tente de positionner l'objet l�g�rement au-dessus du joueur
                if (player.TryGetComponent<Collider>(out Collider playerCollider))
                {
                    if (obj.TryGetComponent<Collider>(out Collider objectCollider))
                    {
                        float playerTopY = playerCollider.bounds.max.y; // Bord sup�rieur du joueur
                        float objectHeight = objectCollider.bounds.extents.y * 2; // Hauteur totale de l'objet
                        float objectOffsetY = objectCollider.bounds.extents.y; // D�calage vertical

                        // Place l'objet au sommet du joueur
                        obj.transform.position = new Vector3(
                            playerCollider.bounds.center.x,
                            playerTopY + objectOffsetY + 0.1f,
                            playerCollider.bounds.center.z
                        );
                    }
                    else
                    {
                        obj.transform.position = playerCollider.bounds.center + Vector3.up * (playerCollider.bounds.extents.y + 0.1f);
                    }
                }
                else
                {
                    // Si le joueur n'a pas de collider, utilise une valeur par d�faut
                    obj.transform.position = player.position + Vector3.up * 0.1f;
                }
            }

            // R�active l'objet dans le monde
            obj.SetActive(true);
        }
        else
        {
            Debug.LogWarning("L'objet � placer dans le monde est null.");
        }
    }




}
