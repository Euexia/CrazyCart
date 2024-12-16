using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject settingsPopup;
    public GameObject creditsPopup;

    public InputRebinding rebindPrefab; // Prefab pour une action spécifique
    public Transform rebindContainer;
    private CrazyCart controls;

    private void Awake()
    {
        controls = new CrazyCart();
    }
    private void OnEnable()
    {
        foreach (var action in controls.asset.actionMaps[0].actions)
        {
            CreateRebindUI(action);
        }
    }
    private void CreateRebindUI(InputAction action)
    {
        var rebindUI = Instantiate(rebindPrefab, rebindContainer);
        rebindUI.bindingDisplayNameText.text = action.GetBindingDisplayString(0);
        rebindUI.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            rebindUI.StartRebinding(action, 0);
        });
    }
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
