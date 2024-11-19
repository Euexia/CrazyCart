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

    private List<Vector3> usedPositions = new List<Vector3>();
    private int currentRound = 1;

    void Start()
    {
        StartCoroutine(SpawnClients());
    }

    IEnumerator SpawnClients()
    {
        if (spawnPositions.Count == 0 || destinations.Count == 0 || clientPrefabs.Count == 0)
        {
            Debug.LogWarning("Aucune position de spawn, destination ou prefab de client définie !");
            yield break;
        }

        int numberOfClientsToSpawn = baseNumberOfClients + (currentRound - 1);

        Debug.Log("Round " + currentRound + ": Nombre de clients à spawn = " + numberOfClientsToSpawn);

        for (int i = 0; i < numberOfClientsToSpawn; i++)
        {
            Vector3 spawnPosition = GetUniqueSpawnPosition();

            if (spawnPosition == Vector3.zero)
            {
                Debug.LogWarning("Aucune position de spawn unique disponible !");
                yield break;
            }

            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
            GameObject spawnedClient = Instantiate(clientPrefab, spawnPosition, Quaternion.identity);

            Client clientScript = spawnedClient.GetComponent<Client>();
            if (clientScript != null)
            {
                clientScript.destinations = new List<Transform>(destinations);
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        currentRound++;
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
