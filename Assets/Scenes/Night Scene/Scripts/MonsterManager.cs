using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : MonoBehaviour
{
    [Header("Monster Setup")]
    [SerializeField] private GameObject skullMonsterPrefab;
    [SerializeField] private float minSpawnRadius = 10f;

    [Header("Debug Spawning")]
    [SerializeField] private bool enableDebugSpawns = true;
    [SerializeField] private float spawnInterval = 5f;
    private float spawnTimer = 0f;

    private void Update()
    {
        if (enableDebugSpawns)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnMonster();
                spawnTimer = 0f;
            }
        }
    }

    public void SpawnMonster()
    {
        if(skullMonsterPrefab == null)
        {
            Debug.LogWarning($"Skull Monster Prefab is not assigned in MonsterManager!");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPoint();

        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            Instantiate(skullMonsterPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Instantiate(skullMonsterPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        var campfireTransform = GameManager.instance.campfire.transform;
        float maxRadius = GameManager.instance.playAreaRadius;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Sqrt(Random.Range(minSpawnRadius * minSpawnRadius, maxRadius * maxRadius));

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return campfireTransform.position + offset;
    }
}
