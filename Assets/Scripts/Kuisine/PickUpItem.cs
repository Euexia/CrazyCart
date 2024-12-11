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
    private int pickUpLayer;

    public Transform player;           
    public float detectionRadius = 3f;  
    public float maxAngle = 45f;        
    public Vector3 canvasOffset = new Vector3(0f, 1f, 0f); 

    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");

        worldSpaceCanvas.SetActive(false);

        leftHandButton.onClick.AddListener(() => PlaceInHand("left"));
        rightHandButton.onClick.AddListener(() => PlaceInHand("right"));
    }

    void Update()
    {
        DetectObjectInProximity();
    }

    private void DetectObjectInProximity()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, 1 << pickUpLayer);

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
            if (currentObject != closestObject)
            {
                currentObject = closestObject;
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
                }
                else if (hand == "right" && rightHandUI.sprite == null)
                {
                    rightHandUI.sprite = itemData.itemSO.icon;
                }

                worldSpaceCanvas.SetActive(false);
            }
        }
    }
}
