using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public LayerMask pickUpLayer;       
    public Color highlightColor = Color.yellow; 
    private Color originalColor;      
    private Renderer currentRenderer; 
    private GameObject highlightedObject; 

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, pickUpLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (highlightedObject != hitObject) 
            {
                RemoveHighlight();
                ApplyHighlight(hitObject);
            }
        }
        else
        {
            RemoveHighlight(); 
        }
    }

    private void ApplyHighlight(GameObject obj)
    {
        highlightedObject = obj;

        currentRenderer = obj.GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            originalColor = currentRenderer.material.color;

            currentRenderer.material.color = highlightColor;
        }
    }

    private void RemoveHighlight()
    {
        if (currentRenderer != null)
        {
            currentRenderer.material.color = originalColor;
        }

        highlightedObject = null;
        currentRenderer = null;
    }
}
