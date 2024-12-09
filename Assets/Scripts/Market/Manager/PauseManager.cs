using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuCanvas; 
    public GameObject settingsPopup;   
    public GameObject creditsPopup;    
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuCanvas.SetActive(true); 
        }
        else
        {
            Time.timeScale = 1f; 
            pauseMenuCanvas.SetActive(false); 
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false); 
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
