/*using UnityEngine;
using System.Collections;

public class PatienceManager : MonoBehaviour
{
    public float basePatience = 30f;
    private float currentPatience;
    public PatienceBar patienceBar;

    public System.Action OnPatienceDepleted;

    public void Initialize(float bonusPatience)
    {
        currentPatience = basePatience + bonusPatience;
        UpdatePatienceBar();
    }

    public void ReducePatience(float amount)
    {
        currentPatience -= amount;
        UpdatePatienceBar();

        if (currentPatience <= 0)
        {
            OnPatienceDepleted?.Invoke();
        }
    }

    private void UpdatePatienceBar()
    {
        if (patienceBar != null)
        {
            patienceBar.UpdateBar((currentPatience / basePatience) * 100);
        }
    }
}
*/