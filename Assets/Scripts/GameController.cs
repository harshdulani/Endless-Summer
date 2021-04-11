using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController game;
    public static int score;

    public bool hasGameStarted;
    
    [Header("Spawning")] 
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints;
    
    public int enemiesOnScreen, maxEnemiesOnScreen;
    public float enemySpawnInterval;
    
    public bool shouldSpawn;

    private WaitForSeconds _spawnWait;

    private void Awake()
    {
        if (!game)
            game = this;
        else
            Destroy(this);
    }
    
    private void Start()
    {
        _spawnWait = new WaitForSeconds(enemySpawnInterval);
    }

    public void StartGameplay()
    {
        shouldSpawn = true;
        hasGameStarted = true;
        StartCoroutine(SpawnEnemies());
        
        foreach (var light in FindObjectsOfType<DayNightCycle>())
        {
            light.StartAnimations();
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (shouldSpawn)
        {
            if (enemiesOnScreen < maxEnemiesOnScreen)
                SpawnEnemy();
            
            yield return _spawnWait;
        }
    }

    private void SpawnEnemy()
    {
        var index = Random.Range(0, spawnPoints.Count);
        var x = Instantiate(enemyPrefab,  spawnPoints[index].position, Quaternion.identity);
        x.GetComponent<EnemyStats>().spawnPointIndex = index;

        enemiesOnScreen++;
    }
}
