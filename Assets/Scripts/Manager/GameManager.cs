using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> clientPrefabs;
    public int currentLevel = 1;
    public int maxClientsPerLevel = 1;
    public float levelDuration = 30f;
    private List<GameObject> activeClients = new List<GameObject>();
    private List<Transform> availableSpawnPoints = new List<Transform>();
    private bool levelRunning = false;

    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        levelRunning = true;
        availableSpawnPoints = new List<Transform>(spawnPoints);
        maxClientsPerLevel = Mathf.Min(currentLevel, 6);
        StartCoroutine(SpawnClients());
    }

    private IEnumerator SpawnClients()
    {
        for (int i = 0; i < maxClientsPerLevel; i++)
        {
            if (availableSpawnPoints.Count == 0 || clientPrefabs.Count == 0)
                yield break;

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Transform spawnPoint = availableSpawnPoints[randomIndex];
            availableSpawnPoints.RemoveAt(randomIndex);

            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
            GameObject newClient = Instantiate(clientPrefab, spawnPoint.position, Quaternion.identity);
            activeClients.Add(newClient);

            Client clientScript = newClient.GetComponent<Client>();
            if (clientScript != null)
            {
                clientScript.OnClientCompleted += () => RemoveClient(newClient, spawnPoint);
            }

            yield return null;
        }
    }

    private void RemoveClient(GameObject client, Transform spawnPoint)
    {
        activeClients.Remove(client);
        availableSpawnPoints.Add(spawnPoint);

        if (activeClients.Count == 0 && levelRunning)
        {
            EndLevel();
        }
    }

    public void EndLevel()
    {
        levelRunning = false;

        if (ImprovementManager.Instance != null)
        {
            ImprovementManager.Instance.AddImprovementPoints(1);
        }

        FindObjectOfType<UIManager>().ShowImprovementMenu();
        StartCoroutine(WaitBeforeNextLevel(20f));
    }

    private IEnumerator WaitBeforeNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentLevel++;
        StartLevel();
    }
}
