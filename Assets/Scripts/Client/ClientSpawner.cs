using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSpawner : MonoBehaviour
{
    public List<GameObject> clientPrefabs; // Liste des prefabs de clients
    public List<Vector3> spawnPositions; // Liste des positions o� les clients peuvent appara�tre
    public int numberOfClients = 3; // Nombre de clients � g�n�rer

    private List<Vector3> usedPositions = new List<Vector3>(); // Liste des positions d�j� utilis�es

    void Start()
    {
        // Appel de la fonction pour g�n�rer des clients � des positions uniques
        SpawnClients();
    }

    void SpawnClients()
    {
        if (spawnPositions.Count == 0)
        {
            Debug.LogWarning("Aucune position de spawn d�finie.");
            return;
        }

        for (int i = 0; i < numberOfClients; i++)
        {
            // Trouver une position disponible qui n'a pas �t� utilis�e
            Vector3 spawnPosition = GetUniqueSpawnPosition();

            if (spawnPosition == Vector3.zero)
            {
                Debug.LogWarning("Pas assez de positions disponibles pour spawn.");
                break;
            }

            // Choisir un prefab de client al�atoire dans la liste des prefabs
            GameObject clientPrefab = clientPrefabs[Random.Range(0, clientPrefabs.Count)];

            // Instancier le client � la position unique
            Instantiate(clientPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Fonction pour obtenir une position unique
    Vector3 GetUniqueSpawnPosition()
    {
        List<Vector3> availablePositions = new List<Vector3>(spawnPositions);

        // Retirer les positions d�j� utilis�es
        foreach (Vector3 usedPosition in usedPositions)
        {
            availablePositions.Remove(usedPosition);
        }

        if (availablePositions.Count > 0)
        {
            // Choisir une position al�atoire parmi celles disponibles
            Vector3 uniquePosition = availablePositions[Random.Range(0, availablePositions.Count)];

            // Ajouter la position utilis�e � la liste des positions utilis�es
            usedPositions.Add(uniquePosition);

            return uniquePosition;
        }
        else
        {
            // Si aucune position disponible, retourner Vector3.zero
            return Vector3.zero;
        }
    }
}
