/*using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public GameObject worldSpaceCanvas;
    public GameObject handsFullCanvas;
    public GameObject takeOrDropCanvas;
    public GameObject handSelectionCanvas;
    public Transform player;
    public float detectionRadius = 3f;
    public float maxAngle = 45f;

    private string currentAction = "";
    private GameObject currentObject = null;
    private int pickUpLayer;
    private int containerLayer;

    private PickUpManager pickUpManager;
    private UIManager uiManager;

    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");
        containerLayer = LayerMask.NameToLayer("Container");

        pickUpManager = GetComponent<PickUpManager>();
        uiManager = GetComponent<UIManager>();

        worldSpaceCanvas.SetActive(false);
        handsFullCanvas.SetActive(false);
    }

    void Update()
    {
        pickUpManager.DetectObjectOrContainerInProximity(player.position, detectionRadius, maxAngle, pickUpLayer, containerLayer, currentAction);
    }

    public void OnLeftHandButtonClick() => uiManager.HandleHandSelection("left");
    public void OnRightHandButtonClick() => uiManager.HandleHandSelection("right");

    public void OnTakeButtonClick() => pickUpManager.OnTakeButtonClick();
    public void OnDropButtonClick() => pickUpManager.OnDropButtonClick();
}
*/