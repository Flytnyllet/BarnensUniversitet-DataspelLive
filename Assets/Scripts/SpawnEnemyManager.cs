using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField] private GameObject[] enemyPrefabs;
	[SerializeField] private Transform[] spawnPoints;
	[SerializeField] private PickUpManager pickUpManager;
	[SerializeField] private Transform target;

	private int nextEnemy;

	private void OnEnable()
	{
		pickUpManager.OnItemDelivered += SpawnNextEnemy;
	}

	private void Update()
	{
		
	}

	private void OnDisable()
	{
		pickUpManager.OnItemDelivered -= SpawnNextEnemy;
	}

	private void SpawnNextEnemy()
	{
		if (nextEnemy >= enemyPrefabs.Length) return; // all enemies have spawned

		Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
		GameObject enemy = Instantiate(enemyPrefabs[nextEnemy], point.position, point.rotation);
		enemy.GetComponent<EnemyMovement>().target = target;

		nextEnemy++; // this enemy can't spawn again
	}
}