using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    [Header("Bar Settings")]
    [Tooltip("Image qui représente le remplissage de la barre.")]
    public Image fillImage; // Représente la progression visuelle.

    [Tooltip("Image de fond de la barre.")]
    public Image background; // Optionnel : image de fond.



    [Tooltip("Valeur actuelle de la progression (entre 0 et 100).")]
    [Range(0, 100)]
    public float progress = 100f; // Valeur actuelle de progression.

    [Tooltip("Couleur de la barre en état normal.")]
    public Color normalColor = Color.green; // Couleur par défaut.

    [Tooltip("Couleur de la barre lorsqu'elle est en dessous du seuil d'alerte.")]
    public Color alertColor = Color.red; // Couleur d'alerte.

    [Tooltip("Seuil de déclenchement de la couleur d'alerte.")]
    [Range(0, 100)]
    public float alertThreshold = 20f; // Seuil d'alerte.

    private void Update()
    {
        // Mettre à jour la barre à chaque frame (utile si `progress` change dynamiquement).
        UpdateBar(progress);
    }

    /// <summary>
    /// Met à jour l'affichage de la barre de progression.
    /// </summary>
    /// <param name="value">Valeur entre 0 et 100 à afficher sur la barre.</param>
    public void UpdateBar(float value)
    {
        // Clamp la valeur pour s'assurer qu'elle reste entre 0 et 100.
        progress = Mathf.Clamp(value, 0f, 100f);

        // Mettre à jour l'image de remplissage si elle est définie.
        if (fillImage != null)
        {
            fillImage.fillAmount = progress / 100f;

            // Changer la couleur en fonction du seuil d'alerte.
            fillImage.color = progress <= alertThreshold ? alertColor : normalColor;
        }

        // Mettre à jour le texte du pourcentage, si défini.

    }

    /// <summary>
    /// Réinitialise la barre à 100%.
    /// </summary>
    public void ResetBar()
    {
        UpdateBar(100f); // Remettre à 100% et mettre à jour.
    }

    /// <summary>
    /// Diminue progressivement la barre de progression.
    /// </summary>
    /// <param name="amount">Quantité à diminuer.</param>
    public void DecreaseBar(float amount)
    {
        UpdateBar(progress - amount); // Réduit la progression de la quantité spécifiée.
    }
}
