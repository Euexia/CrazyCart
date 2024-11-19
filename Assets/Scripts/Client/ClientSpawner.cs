using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : MonoBehaviour
{
    public List<GameObject> clientPrefabs;
    public List<Vector3> spawnPositions;
    public int baseNumberOfClients = 3;
    public List<Transform> destinations;
    public float spawnDelay = 2f;

    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();
    private List<GameObject> activeClients = new List<GameObject>();
    private int currentRound = 1;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnClients());
    }

    IEnumerator SpawnClients()
    {
        if (isSpawning) yield break;
        isSpawning = true;

        if (spawnPositions.Count == 0 || destinations.Count == 0 || clientPrefabs.Count == 0)
        {
            isSpawning = false;
            yield break;
        }

        int numberOfClientsToSpawn = baseNumberOfClients + (currentRound - 1);

        for (int i = 0; i < numberOfClientsToSpawn; i++)
        {
            Vector3 spawnPosition = GetUniqueSpawnPosition();
            if (spawnPosition == Vector3.zero)
            {
                break;
            }

            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
            GameObject spawnedClient = Instantiate(clientPrefab, spawnPosition, Quaternion.identity);

            Client clientScript = spawnedClient.GetComponent<Client>();
            if (clientScript != null)
            {
                clientScript.destinations = new List<Transform>(destinations);
                clientScript.OnDespawn += () => HandleClientDespawn(spawnedClient, spawnPosition);
                activeClients.Add(spawnedClient);
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        currentRound++;
        isSpawning = false;
    }

    private void HandleClientDespawn(GameObject client, Vector3 position)
    {
        if (usedPositions.Contains(position))
        {
            usedPositions.Remove(position);
        }

        activeClients.Remove(client);
        Destroy(client);

        if (!isSpawning) StartCoroutine(SpawnClients());
    }

    Vector3 GetUniqueSpawnPosition()
    {
        List<Vector3> availablePositions = new List<Vector3>(spawnPositions);

        foreach (Vector3 usedPosition in usedPositions)
        {
            availablePositions.Remove(usedPosition);
        }

        if (availablePositions.Count > 0)
        {
            Vector3 uniquePosition = availablePositions[Random.Range(0, availablePositions.Count)];
            usedPositions.Add(uniquePosition);
            return uniquePosition;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
