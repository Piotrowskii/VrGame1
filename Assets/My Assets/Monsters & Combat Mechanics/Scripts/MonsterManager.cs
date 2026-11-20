using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Skull
}

[System.Serializable]
public class SpawnCircleDescription
{
    public float spawnInterval = 0.5f;

    public List<(EnemyType type, int amount)> enemies = new();
}

public class WaveDescription
{
    public List<SpawnCircleDescription> circles = new(); 
}

public class MonsterManager : MonoBehaviour
{
    [Header("Monster Setup")]
    [SerializeField] private GameObject skullMonsterPrefab;
    [SerializeField] private float minSpawnRadius = 20f;

    [Header("Spawn circle")]
    [SerializeField] private GameObject spawnCriclePrefab;
    [SerializeField] private float spawnWarningDuration = 3f;
    [SerializeField] private float navMeshSampleDistance = 2f;

    private HashSet<Monster> currentCircleMonsters = new HashSet<Monster>();
    private bool isWaveRunning;

    private WaveDescription exampleWave = new WaveDescription()
    {
        circles = new List<SpawnCircleDescription>()
        {
            new SpawnCircleDescription()
            {
                spawnInterval = 1.5f,
                enemies = new List<(EnemyType, int)>
                {
                    (EnemyType.Skull, 10)
                }
            },
            new SpawnCircleDescription()
            {
                spawnInterval = 1f,
                enemies = new List<(EnemyType, int)>
                {
                    (EnemyType.Skull, 20),
                }
            },
            new SpawnCircleDescription()
            {
                spawnInterval = 0.5f,
                enemies = new List<(EnemyType, int)>
                {
                    (EnemyType.Skull, 30)
                }
            },
            new SpawnCircleDescription()
            {
                spawnInterval = 1f,
                enemies = new List<(EnemyType, int)>
                {
                    (EnemyType.Skull, 20),
                }
            },
            new SpawnCircleDescription()
            {
                spawnInterval = 1.5f,
                enemies = new List<(EnemyType, int)>
                {
                    (EnemyType.Skull, 10)
                }
            },
        }
    };

    private void Start()
    {
        StartWave(exampleWave);
    }

    private void StartWave(WaveDescription wave)
    {
        if (isWaveRunning)
        {
            Debug.LogWarning("A wave is already running");
            return;
        }

        if (wave == null || wave.circles == null || wave.circles.Count == 0)
        {
            Debug.LogWarning("Wave has no spawn circles!");
            return;
        }

        StartCoroutine(SpawnWaveRoutine(wave));
    }

    private IEnumerator SpawnWaveRoutine(WaveDescription wave)
    {
        isWaveRunning = true;

        foreach(var circleDescription in wave.circles)
        {
            var circle = CreateSpawnCircle();

            if (circle == null) continue;

            circle.gameObject.SetActive(true);

            yield return new WaitForSeconds(spawnWarningDuration);

            yield return StartCoroutine(SpawnEnemiesInCircle(circle, circleDescription));

            yield return StartCoroutine(WaitForCircleEnemiesToDie());

            Destroy(circle.gameObject);

            currentCircleMonsters.Clear();
        }

        isWaveRunning = false;
    }

    private SpawnCircle CreateSpawnCircle()
    {
        if(spawnCriclePrefab == null)
        {
            Debug.LogWarning("Spawn circle prefab is missing");
            return null;
        }

        var position = GetRandomSpawnPoint();

        var circleObject = Instantiate(spawnCriclePrefab, position, Quaternion.identity);

        var spawnCircleComponent = circleObject.GetComponent<SpawnCircle>();

        if(spawnCircleComponent == null)
        {
            Debug.LogWarning("Spawn circle doesn't have spawnCircle component");
            return null;
        }

        return circleObject.GetComponent<SpawnCircle>();
    }

    private IEnumerator SpawnEnemiesInCircle(SpawnCircle circle, SpawnCircleDescription description)
    {
        currentCircleMonsters.Clear();

        foreach(var (enemyType, amount) in description.enemies)
        {
            for(int i = 0; i < amount; i++)
            {
                if(circle == null) yield break;

                var position = circle.GetRandomSpawnPoint();

                SpawnMonsterAt(position, enemyType);

                if(description.spawnInterval > 0f)
                {
                    yield return new WaitForSeconds(description.spawnInterval);
                }
            }
        }
    }

    private void SpawnMonsterAt(Vector3 position, EnemyType enemyType)
    {
        GameObject prefab = GetEnemyPrefab(enemyType);

        if(prefab == null)
        {
            Debug.LogWarning($"Missing prefab for {enemyType}");
            return;
        }

        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
        {
            Debug.LogWarning("Couldn't find a valid NavMesh spawn position");
            return;
        }

        var towerDirection = (GameManager.Instance.Tower.transform.position - hit.position).normalized;
        towerDirection.y = 0;
        var monsterRotation = Quaternion.LookRotation(towerDirection);

        var monsterObject = Instantiate(prefab, hit.position, monsterRotation);

        var monster = monsterObject.GetComponent<Monster>();

        if (monster == null)
        {
            Debug.LogWarning("Enemy prefab is missing Monster component");
            Destroy(monsterObject);
            return;
        }

        currentCircleMonsters.Add(monster);

        monster.OnDeath.AddListener(() => HandleMonsterDeath(monster));
    }

    private GameObject GetEnemyPrefab(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Skull:
                return skullMonsterPrefab;
            default:
                return null;
        }
    }

    private void HandleMonsterDeath(Monster monster)
    {
        currentCircleMonsters.Remove(monster);
    }

    private IEnumerator WaitForCircleEnemiesToDie()
    {
        while (currentCircleMonsters.Count > 0)
        {
            yield return null;
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        var towerTransform = GameManager.Instance.Tower.transform;
        float maxRadius = GameManager.Instance.PlayAreaRadius;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(minSpawnRadius * minSpawnRadius, maxRadius * maxRadius));

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return towerTransform.position + offset;
    }
}
