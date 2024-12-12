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

    private string currentAction = ""; // "pick" or "drop"

    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");
        containerLayer = LayerMask.NameToLayer("Container");

        worldSpaceCanvas.SetActive(false);

        leftHandButton.onClick.AddListener(() => HandleHandSelection("left"));
        rightHandButton.onClick.AddListener(() => HandleHandSelection("right"));
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

    private void HandleHandSelection(string hand)
    {
        if (currentAction == "pick")
        {
            PlaceInHand(hand);
        }
        else if (currentAction == "drop")
        {
            RemoveFromHand(hand);
        }
    }

    private void PlaceInHand(string hand)
    {
        if (currentObject != null)
        {
            ItemData itemData = currentObject.GetComponent<ItemData>();
            if (itemData != null)
            {
                if (hand == "left" && leftHandUI.sprite == null)
                {
                    leftHandUI.sprite = itemData.itemSO.icon;
                    leftHandObject = currentObject;
                }
                else if (hand == "right" && rightHandUI.sprite == null)
                {
                    rightHandUI.sprite = itemData.itemSO.icon;
                    rightHandObject = currentObject;
                }

                currentObject.SetActive(false); // Supprime l'objet de la scène
                currentObject = null;
                worldSpaceCanvas.SetActive(false);
            }
        }
    }

    private void RemoveFromHand(string hand)
    {
        if (hand == "left" && leftHandUI.sprite != null)
        {
            PlaceObjectInScene(leftHandObject);
            leftHandUI.sprite = null;
            leftHandObject = null;
        }
        else if (hand == "right" && rightHandUI.sprite != null)
        {
            PlaceObjectInScene(rightHandObject);
            rightHandUI.sprite = null;
            rightHandObject = null;
        }

        worldSpaceCanvas.SetActive(false);
    }

    private void PlaceObjectInScene(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.position = currentObject.transform.position + Vector3.up; // Positionner légèrement au-dessus
            obj.SetActive(true); // Réactive l'objet
        }
    }
}
