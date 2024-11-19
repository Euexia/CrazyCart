using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<GameObject> clientPrefabs;
    public int currentLevel = 1;
    public float spawnInterval = 2f; // Intervalle entre les spawns
    public float levelDuration = 5f;

    private List<GameObject> activeClients = new List<GameObject>();
    private bool levelRunning = false;

    private UIManager uiManager; // Référence au UIManager

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();  // Trouver le UIManager
        StartLevel();
    }

    public void StartLevel()
    {
        levelRunning = true;
        StartCoroutine(SpawnClientsAtLevelStart());  // Spawn les clients dès le début du niveau
    }

    private IEnumerator SpawnClientsAtLevelStart()
    {
        int numberOfClientsToSpawn = currentLevel; // On spawn autant de clients que le niveau
        Debug.Log($"Niveau {currentLevel}: Nombre de clients à spawn = {numberOfClientsToSpawn}");

        for (int i = 0; i < numberOfClientsToSpawn; i++)
        {
            if (spawnPoints.Count > 0 && clientPrefabs.Count > 0)
            {
                SpawnClient();
            }
            yield return new WaitForSeconds(spawnInterval);
        }

        // Une fois tous les clients spawnés, on démarre le suivi
        StartCoroutine(ManageClients());
    }

    private IEnumerator ManageClients()
    {
        while (levelRunning)
        {
            // Vérifier si des clients ont terminé leur tâche et les enlever
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
            // Abonnez-vous à l'événement pour gérer le despawn
            clientScript.OnClientCompleted += () => RemoveClient(newClient);
        }
    }

    private void RemoveClient(GameObject client)
    {
        if (activeClients.Contains(client))
        {
            activeClients.Remove(client);
            Destroy(client);
        }

        // Vérifiez si le niveau doit se terminer
        if (activeClients.Count == 0)
        {
            EndLevel();
        }
    }

    public void EndLevel()
    {
        levelRunning = false;

        if (ImprovementManager.Instance != null)
        {
            ImprovementManager.Instance.AddImprovementPoints(1); // Ajouter des points d'amélioration
        }

        // Mettre le jeu en pause et afficher le menu d'amélioration
        PauseGame();

        // Affiche le menu d'amélioration
        if (uiManager != null)
        {
            uiManager.ShowImprovementMenu();
        }

        // Délai avant de passer au niveau suivant
        StartCoroutine(WaitBeforeNextLevel(5f)); // Donne 5 secondes pour montrer le menu
    }

    private IEnumerator WaitBeforeNextLevel(float delay)
    {
        yield return new WaitForSeconds(delay); // Attendre avant de passer au niveau suivant

        currentLevel++; // Incrémenter le niveau
        StartLevel(); // Démarrer le prochain niveau
    }

    // Fonction pour mettre le jeu en pause
    private void PauseGame()
    {
        Time.timeScale = 0f;  // Met le jeu en pause
    }

    // Fonction pour reprendre le jeu
    public void ResumeGame()
    {
        Time.timeScale = 1f;  // Reprend le jeu
    }
}
