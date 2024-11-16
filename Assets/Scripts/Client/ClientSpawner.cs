using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : MonoBehaviour
{
    public List<GameObject> clientPrefabs;
    public List<Vector3> spawnPositions;
    public int numberOfClients = 3;

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        SpawnClients();
    }

    void SpawnClients()
    {
        if (spawnPositions.Count == 0)
        {
            return;
        }

        for (int i = 0; i < numberOfClients; i++)
        {
            Vector3 spawnPosition = GetUniqueSpawnPosition();

            if (spawnPosition == Vector3.zero)
            {
                break;
            }

            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
            Instantiate(clientPrefab, spawnPosition, Quaternion.identity);
        }
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