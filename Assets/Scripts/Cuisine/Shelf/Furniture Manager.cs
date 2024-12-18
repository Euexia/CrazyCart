using UnityEngine;
using System.Collections.Generic;

public class FurnitureManager : MonoBehaviour
{
    public Furniture furnitureData; 

    private GameObject furnitureGameObject; 

    public void InitializeFurniture(GameObject targetObject)
    {
        if (furnitureData == null)
        {
            Debug.LogError("Furniture data is not assigned!");
            return;
        }

        furnitureGameObject = targetObject;

        furnitureGameObject.name = furnitureData.furnitureName;

 

        Debug.Log($"Furniture '{furnitureData.furnitureName}' initialized on {targetObject.name}.");
    }

  
}
