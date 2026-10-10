using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MassEnemySpawner : MonoBehaviour
{
    PickUpManager pickUpManager;
    
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform target;

    [Header("Spawn area (ring around the target)")]
    [SerializeField] private int enemyCount = 30;
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float maxDistance = 25f;

    private void Awake()
    {
        pickUpManager = FindObjectOfType<PickUpManager>();
    }

    private void OnEnable()
    {
        pickUpManager.OnItemDelivered += SpawnAll;
    }

    private void OnDisable()
    {
        pickUpManager.OnItemDelivered -= SpawnAll;
    }

    public void SpawnAll()
    {
        int spawned = 0;
        int attemptsLeft = enemyCount * 20;

        while (spawned < enemyCount && attemptsLeft > 0)
        {
            attemptsLeft--;

            if (!TryGetSpawnPosition(out Vector3 position)) continue;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
            enemy.GetComponent<EnemyMovement>().target = target;
            spawned++;
        }
    }

    private bool TryGetSpawnPosition(out Vector3 position)
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPoint = target.position + new Vector3(direction.x, 0f, direction.y) * distance;

        if (NavMesh.SamplePosition(spawnPoint, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            Vector3 offset = hit.position - target.position;
            offset.y = 0f;

            if (offset.magnitude >= minDistance)
            {
                position = hit.position;
                return true;
            }
        }

        position = default;
        return false;
    }
}