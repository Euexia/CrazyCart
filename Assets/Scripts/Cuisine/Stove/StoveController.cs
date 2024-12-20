using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoveController : MonoBehaviour
{
    public GameObject worldSpaceCanvas;
    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureText;
    public ParticleSystem temperatureParticles;
    public float activationDistance = 3f;
    public Transform player;
    private Stove stove;
    private bool isCanvasActive = false;
    private Collider stoveCollider;

    void Start()
    {
        worldSpaceCanvas.SetActive(false);
        stove = GetComponent<Stove>();
        stoveCollider = stove.GetComponent<Collider>();


        if (temperatureSlider != null && stove != null)
        {
            temperatureSlider.minValue = stove.minTemperature;
            temperatureSlider.maxValue = stove.maxTemperature;
            temperatureSlider.value = 0; // Initialisez la température à 0
            temperatureSlider.onValueChanged.AddListener(UpdateTemperature);
        }

        if (stove != null)
        {
            stove.temperature = 0; // Assurez-vous que la température de départ est 0
        }

        if (temperatureParticles != null)
        {
            var mainModule = temperatureParticles.main;
            mainModule.startSize = 0.5f;
            mainModule.startColor = Color.gray;
        }
       

        // Mettez à jour la température et l'état des particules dès le départ
        UpdateTemperature(0);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= activationDistance && !isCanvasActive)
        {
            ActivateCanvas();
        }
        else if (distance > activationDistance && isCanvasActive)
        {
            DeactivateCanvas();
        }

        // Call the DetectContainerOnStove method to check if a container is placed
        DetectContainerOnStove();
    }


    private void ActivateCanvas()
    {
        worldSpaceCanvas.SetActive(true);
        isCanvasActive = true;
    }

    private void DeactivateCanvas()
    {
        worldSpaceCanvas.SetActive(false);
        isCanvasActive = false;
    }

    void UpdateTemperature(float value)
    {
        if (stove != null)
        {
            stove.temperature = value;
            if (temperatureText != null)
            {
                temperatureText.text = $"Température : {value:F1}°C";
            }

            UpdateParticleFeedback(value);
        }
    }

    void UpdateParticleFeedback(float temperature)
    {
        if (temperatureParticles != null)
        {
            var mainModule = temperatureParticles.main;
            var emissionModule = temperatureParticles.emission;

            if (temperature <= 0)
            {
                emissionModule.rateOverTime = 0f;
            }
            else
            {
                emissionModule.rateOverTime = Mathf.Lerp(0f, 50f, Mathf.InverseLerp(stove.minTemperature, stove.maxTemperature, temperature));
            }

            Color particleColor = Color.Lerp(Color.gray, Color.red, Mathf.InverseLerp(stove.minTemperature, stove.maxTemperature, temperature));
            mainModule.startColor = particleColor;
        }
    }

    public void PlaceContainerOnStove(GameObject container)
    {
        if (container != null)
        {
            // Placer l'objet au-dessus du poêle
            container.transform.position = transform.position + Vector3.up * 0.5f; // Ajustez la position
            container.transform.SetParent(transform); // Associez le conteneur au poêle
        }
    }


    private void DetectContainerOnStove()
    {
        // Détecte tous les objets dans la zone de la stove
        Collider[] colliders = Physics.OverlapBox(stoveCollider.bounds.center, stoveCollider.bounds.extents);

        foreach (var collider in colliders)
        {
            // Vérifie si l'objet est un conteneur et s'il n'est pas un enfant du poêle
            if (collider.CompareTag("Container") && collider.transform.parent != stove.transform)
            {
                Debug.Log("Un conteneur est sur le poêle : " + collider.gameObject.name);
                // Logique de placement de conteneur ou autre traitement
            }
        }
    }

}
