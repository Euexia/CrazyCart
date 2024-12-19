using UnityEngine;

public class MagicCabinetInteraction : MonoBehaviour
{
    public LayerMask magicCabinetLayer; // Layer des Magic Cabinets
    public MonoBehaviour cameraMovementScript; // Script de mouvement de la caméra (optionnel)
    public PlayerController playerController; // Script du joueur (optionnel)

    void Start()
    {
        // Désactiver tous les Canvas des MagicCabinets au démarrage
        MagicCabinet[] cabinets = FindObjectsOfType<MagicCabinet>();
        foreach (MagicCabinet cabinet in cabinets)
        {
            if (cabinet != null && cabinet.gameObject.activeSelf)
            {
                cabinet.CloseMagicCabinetCanvas(); // Désactiver le canvas au démarrage
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clique gauche
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast pour détecter un Magic Cabinet
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, magicCabinetLayer))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);  // Affiche le nom du collider touché

            MagicCabinet magicCabinet = hit.collider.GetComponent<MagicCabinet>();
            if (magicCabinet != null)
            {
                OpenMagicCabinet(magicCabinet);
            }
        }
    }


    void OpenMagicCabinet(MagicCabinet cabinet)
    {
        if (cabinet != null)
        {
            cabinet.OpenMagicCabinetCanvas(); // Active le canvas spécifique à MagicCabinet
        }

        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = false; // Désactive le mouvement de la caméra
        }

        if (playerController != null)
        {
            playerController.LockCamera(true); // Bloque la rotation de la caméra
        }

        Time.timeScale = 0; // Met le jeu en pause
    }

    public void CloseMagicCabinet(MagicCabinet cabinet)
    {
        if (cabinet != null)
        {
            cabinet.CloseMagicCabinetCanvas(); // Désactive le canvas spécifique à MagicCabinet
        }

        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true; // Réactive le mouvement de la caméra
        }

        if (playerController != null)
        {
            playerController.LockCamera(false); // Réactive la rotation de la caméra
        }

        Time.timeScale = 1; // Reprend le jeu
    }
}
