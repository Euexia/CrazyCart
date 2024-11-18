using static UnityEditor.Progress;
using UnityEngine;

[System.Serializable]
public class Carton : MonoBehaviour
{
    public ItemSO cartonItemSO; 
    public ItemData containedItem;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    public Vector3 OriginalPosition => originalPosition;
 
    void Start()
    {
        // Enregistrer la position et rotation d'origine
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void ResetToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        gameObject.SetActive(true); // Réactiver le carton si désactivé
    }
}


