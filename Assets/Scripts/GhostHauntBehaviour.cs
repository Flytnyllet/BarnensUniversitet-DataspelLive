using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class GhostHaunt : MonoBehaviour
{
	[Tooltip("How close the ghost must be to snatch. Keep it larger than EnemyMovement's stop distance.")]
	[SerializeField] private float snatchRange = 1.4f;
	[SerializeField] private float snatchCooldown = 3f;
	[SerializeField] private float fleeDuration = 5f;

	private PlayerTopDownController player;
	private EnemyMovement movement;
	private PickUpManager playerPickup;
	private float nextSnatchTime;

	private void Awake()
	{
		player = FindObjectOfType<PlayerTopDownController>();
		playerPickup = player.GetComponent<PickUpManager>();
		movement = GetComponent<EnemyMovement>();
	}

	private void Update()
	{
		if (movement.stunned || movement.IsFleeing || !movement.target) return;
		if (Time.time < nextSnatchTime) return;
		if (!playerPickup) return;
		if (!playerPickup.HasItem) return;

		Vector3 toTarget = movement.target.position - transform.position;
		toTarget.y = 0f;
		if (toTarget.magnitude > snatchRange) return;

		playerPickup.StealItem();
		nextSnatchTime = Time.time + snatchCooldown;
		movement.Flee(fleeDuration);
	}
}