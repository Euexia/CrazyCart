using UnityEngine;
using UnityEngine.UI;

public class PickUpObject : MonoBehaviour
{
    public Image leftHandUI;
    public Image rightHandUI;
    public GameObject worldSpaceCanvas;  
    public Button leftHandButton;        
    public Button rightHandButton;       
    private bool isInRange = false;
    private GameObject pickedObject = null;
    private bool isPicked = false;
    private int pickUpLayer;

    public Vector3 canvasOffset = new Vector3(1f, 0f, 0f); 

    void Start()
    {
        pickUpLayer = LayerMask.NameToLayer("PickUpItem");

        worldSpaceCanvas.SetActive(false);

        leftHandButton.onClick.AddListener(() => PlaceInHand("left"));
        rightHandButton.onClick.AddListener(() => PlaceInHand("right"));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPicked)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.layer == pickUpLayer)
                {
                    PickUp(hit.collider.gameObject);
                }
            }
        }
    }

    private void PickUp(GameObject obj)
    {
        ItemData itemData = obj.GetComponent<ItemData>();
        if (itemData != null && itemData.itemSO != null)
        {
            pickedObject = obj;

            Vector3 spawnPosition = obj.transform.position + canvasOffset;
            worldSpaceCanvas.transform.position = spawnPosition;

            worldSpaceCanvas.SetActive(true);

        }
    }

    private void PlaceInHand(string hand)
    {
        if (pickedObject != null)
        {
            ItemData itemData = pickedObject.GetComponent<ItemData>();
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

                isPicked = true;

                worldSpaceCanvas.SetActive(false);

                pickedObject.SetActive(false); 
            }
        }
    }
}
