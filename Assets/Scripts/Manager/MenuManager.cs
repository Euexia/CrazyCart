using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPopup;
    public GameObject creditsPopup;

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
        }
    }

    public void OpenCredits()
    {
        if (creditsPopup != null)
        {
            creditsPopup.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        if (creditsPopup != null)
        {
            creditsPopup.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
