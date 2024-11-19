using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject improvementMenuPanel;
    public TMP_Text improvementPointsText;
    public ImprovementManager improvementManager;

    private bool isGamePaused = false; // Variable pour suivre l'état de la pause du jeu

    void Start()
    {
        if (improvementMenuPanel == null || improvementPointsText == null || improvementManager == null)
        {
            return;
        }

        improvementMenuPanel.SetActive(false);
        UpdateImprovementPointsText();
    }

    void Update()
    {
        // Vérifie si l'utilisateur appuie sur la touche Tab pour ouvrir/fermer le menu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleImprovementMenu();
        }
    }

    public void ToggleImprovementMenu()
    {
        bool isActive = improvementMenuPanel.activeSelf;
        improvementMenuPanel.SetActive(!isActive);

        if (isActive)
        {
            // Si le menu est fermé, on reprend le jeu
            ResumeGame();
        }
        else
        {
            // Si le menu est ouvert, on met le jeu en pause
            PauseGame();
        }

        UpdateImprovementPointsText();  // Mettre à jour les points à chaque fois que l'on ouvre le menu
    }

    public void ShowImprovementMenu()
    {
        improvementMenuPanel.SetActive(true);
        PauseGame();  // Met le jeu en pause lorsque le menu est affiché
    }

    // Fonction pour mettre le jeu en pause
    private void PauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0f;  // Met le jeu en pause
            isGamePaused = true;
        }
    }

    // Fonction pour reprendre le jeu
    private void ResumeGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;  // Reprend le jeu
            isGamePaused = false;
        }
    }

    public void UpdateImprovementPointsText()
    {
        if (improvementManager != null && improvementPointsText != null)
        {
            improvementPointsText.text = "Points d'amélioration : " + improvementManager.improvementPoints;
        }
    }
}
