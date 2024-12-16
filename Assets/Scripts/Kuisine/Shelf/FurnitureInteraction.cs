using UnityEngine;

public class FurnitureInteraction : MonoBehaviour
{
    public LayerMask furnitureLayer;
    public Canvas shelvesCanvas;
    public MonoBehaviour cameraMovementScript;
    public PlayerController playerController;

    void Start()
    {
        if (shelvesCanvas != null)
        {
            shelvesCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, furnitureLayer))
        {
            OpenShelvesCanvas();
        }
    }

    void OpenShelvesCanvas()
    {
        if (shelvesCanvas != null)
        {
            shelvesCanvas.gameObject.SetActive(true);
        }
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = false;
        }
        if (playerController != null)
        {
            playerController.LockCamera(true); // Bloque la rotation de la caméra
        }
        Time.timeScale = 0;
    }

    public void CloseShelvesCanvas()
    {
        if (shelvesCanvas != null)
        {
            shelvesCanvas.gameObject.SetActive(false);
        }
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true;
        }
        if (playerController != null)
        {
            playerController.LockCamera(false); // Réactive la rotation de la caméra
        }
        Time.timeScale = 1;
    }
}
