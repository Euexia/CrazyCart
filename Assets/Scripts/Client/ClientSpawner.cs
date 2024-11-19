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
        // Lancer l'apparition des clients au début du jeu
        StartCoroutine(SpawnClients());
    }

    IEnumerator SpawnClients()
    {
        // Eviter de lancer plusieurs fois l'apparition des clients
        if (isSpawning) yield break;
        isSpawning = true;

        if (spawnPositions.Count == 0 || destinations.Count == 0 || clientPrefabs.Count == 0)
        {
            Debug.LogWarning("Aucune position de spawn, destination ou prefab de client définie !");
            isSpawning = false;
            yield break;
        }

        // Calculer le nombre de clients à faire spawn
        int numberOfClientsToSpawn = baseNumberOfClients + (currentRound - 1);

        Debug.Log("Round " + currentRound + ": Nombre de clients à spawn = " + numberOfClientsToSpawn);

        for (int i = 0; i < numberOfClientsToSpawn; i++)
        {
            Vector3 spawnPosition = GetUniqueSpawnPosition();
            if (spawnPosition == Vector3.zero)
            {
                Debug.LogWarning("Aucune position de spawn unique disponible !");
                break;
            }

            // Créer un client à la position de spawn
            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];
            GameObject spawnedClient = Instantiate(clientPrefab, spawnPosition, Quaternion.identity);

            // Associer les destinations et la gestion de la disparition
            Client clientScript = spawnedClient.GetComponent<Client>();
            if (clientScript != null)
            {
                clientScript.destinations = new List<Transform>(destinations);
                clientScript.OnDespawn += () => HandleClientDespawn(spawnedClient, spawnPosition);
                activeClients.Add(spawnedClient);
            }

            // Attendre un certain délai avant de spawn le prochain client
            yield return new WaitForSeconds(spawnDelay);
        }

        currentRound++;
        isSpawning = false;
    }

    private void HandleClientDespawn(GameObject client, Vector3 position)
    {
        Debug.Log("Client despawn détecté : " + client.name);

        if (usedPositions.Contains(position))
        {
            usedPositions.Remove(position);
        }

        activeClients.Remove(client); // Retirer de la liste des clients actifs
        Destroy(client);

        // Vérifier si tous les clients ont disparu pour respawner si nécessaire
        if (!isSpawning) StartCoroutine(SpawnClients());
    }

    Vector3 GetUniqueSpawnPosition()
    {
        // Trouver une position unique pour chaque client
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
