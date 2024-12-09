using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject improvementMenuPanel;
    public TMP_Text improvementPointsText;
    public ImprovementManager improvementManager;

    private bool isGamePaused = false;

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
            ResumeGame();
        }
        else
        {
            PauseGame();
        }

        UpdateImprovementPointsText();  
    }

    public void ShowImprovementMenu()
    {
        improvementMenuPanel.SetActive(true);
        PauseGame();  
    }

    private void PauseGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0f; 
            isGamePaused = true;
        }
    }

    private void ResumeGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;  
            isGamePaused = false;
        }
    }

    public void UpdateImprovementPointsText()
    {
        if (improvementManager != null && improvementPointsText != null)
        {
            improvementPointsText.text = "Points d'am�lioration : " + improvementManager.improvementPoints;
        }
    }
}
