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

    void Start()
    {
        worldSpaceCanvas.SetActive(false);
        stove = GetComponent<Stove>();

        if (temperatureSlider != null && stove != null)
        {
            temperatureSlider.minValue = stove.minTemperature;
            temperatureSlider.maxValue = stove.maxTemperature;
            temperatureSlider.value = stove.temperature;
            temperatureSlider.onValueChanged.AddListener(UpdateTemperature);
        }

        if (temperatureParticles != null)
        {
            var mainModule = temperatureParticles.main;
            mainModule.startSize = 0.5f;  
            mainModule.startColor = Color.gray;  
        }
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

            // Pas de particules à 0°C
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
}
