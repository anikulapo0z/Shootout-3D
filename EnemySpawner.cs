using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f; // The interval between each spawn
    public int enemiesKilledCount = 0;
    //public GameObject enemySpawner;

    private float timer = 0f;

    //trigger the coroutine if the player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger"); // Added Debug.Log
            StartCoroutine(SpawnEnemies());
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    IEnumerator SpawnEnemies()
    {
        timer = 0f;
        while (timer <= 30f && enemiesKilledCount < 15)
        {
            Debug.Log("Spawning Enemy"); // Added Debug.Log
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);

            if (enemiesKilledCount == 15)
            {
                Debug.Log("Enemy Kill Count reached 15"); // Added Debug.Log
                StopCoroutine(SpawnEnemies());
            }

            else if (timer >= 30f)
            {
                Debug.Log("Timer reached 30 seconds"); // Added Debug.Log
                StopCoroutine(SpawnEnemies());
            }
        }
    }

    void SpawnEnemy()
    {
        Debug.Log("Spawning Enemy"); // Added Debug.Log
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    }

    public void EnemyKilled()
    {
        enemiesKilledCount++;
        Debug.Log("Enemy Killed. Total: " + enemiesKilledCount); // Added Debug.Log
    }
}
