using UnityEngine;

public class TimedEnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject[] enemyPrefabs;
	[SerializeField] private Transform[] spawnPoints;
	[SerializeField] private Transform target;
	[SerializeField] private float spawnInterval;

	private void Start()
	{
		InvokeRepeating(nameof(SpawnRandomEnemy), spawnInterval, spawnInterval);
	}

	private void SpawnRandomEnemy()
	{
		GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
		Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

		GameObject enemy = Instantiate(prefab, point.position, point.rotation);
		enemy.GetComponent<EnemyMovement>().target = target;
	}
}