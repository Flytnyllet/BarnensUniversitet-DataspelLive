using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAttack : MonoBehaviour
{
	[SerializeField] private int damage = 10;
	[SerializeField] private float attackRange = 1.5f;
	[SerializeField] private float attackCooldown = 1.5f;

	[SerializeField] private float rangeLeeway = 0.5f;
	[SerializeField] private float failSafeTimeout = 3f;

	private EnemyMovement movement;
	private AnimatorManager animatorManager;
	[SerializeField] private PlayerTopDownController player;

	private bool attacking;
	private bool hitDelivered;
	private float attackStartTime;
	private float nextAttackTime;

	private void Awake()
	{
		movement = GetComponent<EnemyMovement>();
		animatorManager = GetComponent<AnimatorManager>();

		if (TryGetComponent(out NavMeshAgent agent))
			agent.stoppingDistance = attackRange * 0.8f;
	}

	private void Update()
	{
		if (movement.stunned)
		{
			if (attacking) EndAttack();
			return;
		}

		if (attacking)
		{
			if (Time.time - attackStartTime > failSafeTimeout)
				EndAttack();
			return;
		}

		if (!movement.target) return;
		if (!player) return;

		if (Time.time >= nextAttackTime && DistanceToTarget() <= attackRange)
			StartAttack();
	}

	private void StartAttack()
	{
		attacking = true;
		hitDelivered = false;
		attackStartTime = Time.time;
		SetSwingAnimation(true);
	}

	private void EndAttack()
	{
		attacking = false;
		nextAttackTime = Time.time + attackCooldown;
		SetSwingAnimation(false);
	}

	private float DistanceToTarget()
	{
		Vector3 toTarget = movement.target.position - transform.position;
		toTarget.y = 0f;
		return toTarget.magnitude;
	}

	private void SetSwingAnimation(bool swinging)
	{
		if (animatorManager)
			animatorManager.AttackAnimation(swinging);
	}

	public void OnAttackHit()
	{
		if (!attacking || hitDelivered || movement.stunned || player == null) return;

		hitDelivered = true;

		if (DistanceToTarget() <= attackRange + rangeLeeway)
			player.TakeDamage(damage);
	}

	public void OnAttackEnd()
	{
		if (attacking) EndAttack();
	}
}