using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> clientPrefabs;
    public int currentLevel = 1;
    public float spawnInterval = 2f;
    public float levelDuration = 5f;

    public AudioClip backgroundMusic;
    private AudioSource audioSource;

    private List<GameObject> activeClients = new List<GameObject>();
    private bool levelRunning = false;

    private UIManager uiManager;
    public bool ClientLostByImpatience { get; private set; }

    public GameObject statsCanvas;
    public TextMeshProUGUI statsText;
    private bool statsDisplayed = false;

    public int satisfiedClients = 0;
    public int globalTotalClients = 0; 

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        audioSource = gameObject.AddComponent<AudioSource>();

        ConfigureAudio();
        StartLevel();
    }

    private void ConfigureAudio()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }

    public void StartLevel()
    {
        ClientLostByImpatience = false;
        levelRunning = true;
        StartCoroutine(SpawnClientsAtLevelStart());
    }

    public void SetClientLostByImpatience(bool value)
    {
        ClientLostByImpatience = value;
    }

    public void ClientSatisfied()
    {
        satisfiedClients++;
        Debug.Log($"Client satisfait. Clients satisfaits : {satisfiedClients} sur {globalTotalClients} au total.");
        ShowSatisfactionCanvas();
    }

    private void ShowSatisfactionCanvas()
    {
        if (currentLevel % 5 == 0)
        {
            string stats = $"Total Clients: {globalTotalClients}\nSatisfied Clients: {satisfiedClients}";
        }
    }

    private IEnumerator SpawnClientsAtLevelStart()
    {
        int numberOfClientsToSpawn = currentLevel;
        Debug.Log($"Niveau {currentLevel}: Nombre de clients à spawn = {numberOfClientsToSpawn}");

        for (int i = 0; i < numberOfClientsToSpawn; i++)
        {
            if (spawnPoints.Count > 0 && clientPrefabs.Count > 0)
            {
                SpawnClient();
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        StartCoroutine(ManageClients());
    }

    private IEnumerator ManageClients()
    {
        while (levelRunning)
        {
            if (activeClients.Count == 0)
            {
                EndLevel();
            }

            yield return null;
        }
    }

    private void SpawnClient()
    {
        if (spawnPoints.Count == 0 || clientPrefabs.Count == 0)
            return;

        int randomIndex = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
        GameObject newClient = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
        activeClients.Add(newClient);

        globalTotalClients++;
        Debug.Log($"Client spawné. Total global clients : {globalTotalClients}");

        Client clientScript = newClient.GetComponent<Client>();
        if (clientScript != null)
        {
            clientScript.OnClientCompleted -= () => RemoveClient(newClient);
            clientScript.OnDespawn -= () => RemoveClient(newClient);

            clientScript.OnClientCompleted += () => RemoveClient(newClient);
            clientScript.OnDespawn += () => RemoveClient(newClient);
        }
    }

    private void RemoveClient(GameObject client)
    {
        if (activeClients.Contains(client))
        {
            activeClients.Remove(client);
            Destroy(client);
        }

        Client clientScript = client.GetComponent<Client>();
        if (clientScript != null && clientScript.IsSatisfied())
        {
            satisfiedClients++;
        }

        if (activeClients.Count == 0 && levelRunning)
        {
            EndLevel();
        }
    }

    public void EndLevel()
    {
        levelRunning = false;

        if (!ClientLostByImpatience)
        {
            if (ImprovementManager.Instance != null)
            {
                ImprovementManager.Instance.AddImprovementPoints(1);
            }
        }

        PauseGame();

        if (uiManager != null)
        {
            uiManager.ShowImprovementMenu();
        }

        if (currentLevel >= 3 && !statsDisplayed)
        {
            ShowStats();
        }
        else
        {
            StartCoroutine(WaitBeforeNextLevel(5f));
        }
    }

    private void ShowStats()
    {
        statsDisplayed = true;

        if (statsCanvas != null)
        {
            statsCanvas.SetActive(true);
        }

        if (statsText != null)
        {
            statsText.text = $"Vous avez réussi à aider {satisfiedClients} personne(s) sur {globalTotalClients} clients au total.";
        }
    }

    private IEnumerator WaitBeforeNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentLevel++;
        satisfiedClients = 0; 
        statsDisplayed = false;
        StartLevel();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void RetourMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
