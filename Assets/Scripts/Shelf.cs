using UnityEngine;

public class Shelf : MonoBehaviour
{
    private Carton storedCarton;

    public void TryToFillWithCarton(Carton carton)
    {
        if (carton != null)
        {
            if (storedCarton == null) 
            {
                storedCarton = carton;
                storedCarton.gameObject.SetActive(true); 
                storedCarton.transform.SetParent(transform); 
                storedCarton.transform.localPosition = Vector3.zero; 
                Debug.Log($"Carton {carton.gameObject.name} ajouté à l'étagère.");
            }
            else
            {
                Debug.Log("L'étagère est déjà pleine.");
            }
        }
    }

}
