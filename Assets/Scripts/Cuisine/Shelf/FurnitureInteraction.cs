using UnityEngine;

public class FurnitureInteraction : MonoBehaviour
{
    public LayerMask furnitureLayer;
    public Canvas shelvesCanvas; 
    public Canvas randomFurnitureCanvas; 
    public MonoBehaviour cameraMovementScript;
    public PlayerController playerController;

    void Start()
    {
        if (shelvesCanvas != null)
        {
            shelvesCanvas.gameObject.SetActive(false);
        }

        if (randomFurnitureCanvas != null)
        {
            randomFurnitureCanvas.gameObject.SetActive(false); 
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
            if (hit.collider.CompareTag("FurnitureRandom"))
            {
                Debug.Log("Meuble random sélectionné");
                OpenRandomFurnitureCanvas();
            }
            else
            {
                Debug.Log("Meuble standard sélectionné");
                OpenShelvesCanvas();
            }
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
            playerController.LockCamera(true);
        }
        Time.timeScale = 0;
    }

    // Méthode pour gérer l'ouverture d'un meuble random
    void OpenRandomFurnitureCanvas()
    {
        // Logique spécifique à un meuble random
        if (randomFurnitureCanvas != null)
        {
            randomFurnitureCanvas.gameObject.SetActive(true); // Active le canvas pour les meubles random
        }
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = false;
        }
        if (playerController != null)
        {
            playerController.LockCamera(true);
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
            playerController.LockCamera(false);
        }
        Time.timeScale = 1;
    }

    // Méthode pour fermer le canvas du meuble random
    public void CloseRandomFurnitureCanvas()
    {
        if (randomFurnitureCanvas != null)
        {
            randomFurnitureCanvas.gameObject.SetActive(false);
        }
        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true;
        }
        if (playerController != null)
        {
            playerController.LockCamera(false);
        }
        Time.timeScale = 1;
    }
}
