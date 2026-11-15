using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    
    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = .5f;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;
    [SerializeField] private bool readyForNextWave = false;
    [Header("Events")]
    public static UnityEvent onEnemyDestroyed = new UnityEvent();
    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawning = false;
    public static EnemySpawner main;
    

    private void Awake()
    {
        main = this;
        onEnemyDestroyed.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        
    }
  
    // Update is called once per frame
    void Update()
    {
        if (isSpawning == false)
        {
            return;

        }
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            enemiesAlive++;
            timeSinceLastSpawn = 0;
        }
        if (enemiesAlive <= 0 && enemiesLeftToSpawn <=0)
        {
            EndWave();
            LevelManager.main.EndRound();
        }
    }
    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, (Mathf.Clamp(currentWave, 0, enemyPrefabs.Length)))];
        Instantiate(prefabToSpawn, LevelManager.main.startPoint.position, Quaternion.identity);
    }
    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }
    public void StartNewWave()
    {
        StartCoroutine(StartWave());
    }
    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
        enemiesLeftToSpawn = EnemiesPerWave();
    }
    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        LevelManager.main.EndRound();
    }
    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
    }
}
