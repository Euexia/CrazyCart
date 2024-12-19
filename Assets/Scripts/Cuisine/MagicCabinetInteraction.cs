using UnityEngine;

public class MagicCabinetInteraction : MonoBehaviour
{
    public LayerMask magicCabinetLayer; // Layer des Magic Cabinets
    public MonoBehaviour cameraMovementScript; // Script de mouvement de la cam�ra (optionnel)
    public PlayerController playerController; // Script du joueur (optionnel)

    void Start()
    {
        // D�sactiver tous les Canvas des MagicCabinets au d�marrage
        MagicCabinet[] cabinets = FindObjectsOfType<MagicCabinet>();
        foreach (MagicCabinet cabinet in cabinets)
        {
            if (cabinet != null && cabinet.gameObject.activeSelf)
            {
                cabinet.CloseMagicCabinetCanvas(); // D�sactiver le canvas au d�marrage
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

        // Raycast pour d�tecter un Magic Cabinet
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, magicCabinetLayer))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);  // Affiche le nom du collider touch�

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
            cabinet.OpenMagicCabinetCanvas(); // Active le canvas sp�cifique � MagicCabinet
        }

        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = false; // D�sactive le mouvement de la cam�ra
        }

        if (playerController != null)
        {
            playerController.LockCamera(true); // Bloque la rotation de la cam�ra
        }

        Time.timeScale = 0; // Met le jeu en pause
    }

    public void CloseMagicCabinet(MagicCabinet cabinet)
    {
        if (cabinet != null)
        {
            cabinet.CloseMagicCabinetCanvas(); // D�sactive le canvas sp�cifique � MagicCabinet
        }

        if (cameraMovementScript != null)
        {
            cameraMovementScript.enabled = true; // R�active le mouvement de la cam�ra
        }

        if (playerController != null)
        {
            playerController.LockCamera(false); // R�active la rotation de la cam�ra
        }

        Time.timeScale = 1; // Reprend le jeu
    }
}
