/*using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    private GameObject currentObject;
    private string currentAction = "";
    private UIManager uiManager;

    void Start()
    {
        uiManager = GetComponent<UIManager>();
    }

    public void DetectObjectOrContainerInProximity(Vector3 playerPosition, float detectionRadius, float maxAngle, int pickUpLayer, int containerLayer, string currentAction)
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, detectionRadius, (1 << pickUpLayer) | (1 << containerLayer));
        GameObject closestObject = null;
        float closestDistance = detectionRadius;

        foreach (var hitCollider in hitColliders)
        {
            Vector3 directionToObject = (hitCollider.transform.position - playerPosition).normalized;
            float angle = Vector3.Angle(playerPosition, directionToObject);

            if (angle <= maxAngle)
            {
                float distance = Vector3.Distance(playerPosition, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = hitCollider.gameObject;
                }
            }
        }

        if (closestObject != null)
        {
            currentObject = closestObject;
            currentAction = closestObject.layer == pickUpLayer ? "pick" : "drop";
            uiManager.DisplayCanvas(currentObject, currentAction);
        }
        else
        {
            uiManager.HideAllCanvases();
        }
    }

    public void OnTakeButtonClick()
    {
        if (currentObject != null && currentObject.layer == containerLayer)
        {
            pendingAction = "take";
            ShowHandSelectionCanvas();
        }
    }

    public void OnDropButtonClick()
    {
        if (currentObject != null && currentObject.layer == containerLayer)
        {
            pendingAction = "drop";
            ShowHandSelectionCanvas();
        }
    }
}
*/