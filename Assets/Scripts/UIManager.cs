using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject improvementMenuPanel;
    public TMP_Text improvementPointsText;
    public ImprovementManager improvementManager;

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

        if (!isActive)
        {
            UpdateImprovementPointsText();
        }
    }

    public void UpdateImprovementPointsText()
    {
        if (improvementManager != null && improvementPointsText != null)
        {
            improvementPointsText.text = "Points d'amélioration : " + improvementManager.improvementPoints;
        }
    }

    public void ShowImprovementMenu()
    {
        improvementMenuPanel.SetActive(true);
    }
}
