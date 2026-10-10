using UnityEngine;

public class StunEnemy : MonoBehaviour
{
	[SerializeField] private float knockbackForce = 10f;
	[SerializeField] private float stunDuration = 1.5f;
	[SerializeField] private AudioSource audioSource;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Enemy")) return;

		var enemy = other.GetComponent<EnemyMovement>();
		if (enemy == null) return;

		enemy.Stun(stunDuration);
		enemy.ApplyKnockback(other.transform.position - transform.position, knockbackForce);
		audioSource.Play();
	}
}