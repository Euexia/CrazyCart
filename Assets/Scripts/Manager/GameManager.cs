using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> clientPrefabs;
    public int currentLevel = 1;
    public float spawnInterval = 2f;
    public float levelDuration = 5f;

    private List<GameObject> activeClients = new List<GameObject>();
    private bool levelRunning = false;

    private UIManager uiManager;
    public bool ClientLostByImpatience { get; private set; } 

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        StartLevel();
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

        StartCoroutine(WaitBeforeNextLevel(5f));
    }

    private IEnumerator WaitBeforeNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);

        currentLevel++;
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
}
