using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAttack : MonoBehaviour
{
	[SerializeField] private int damage = 10;
	[SerializeField] private float attackRange = 1.5f;
	[SerializeField] private float attackCooldown = 1.5f;

	private float rangeLeeway = 0.5f;
	private float failSafeTimeout = 3f;

	private EnemyMovement movement;
	private AnimatorManager animatorManager;
	private PlayerTopDownController player;

	private bool attacking;
	public bool IsAttacking => attacking;
	private bool hitDelivered;
	private float attackStartTime;
	private float nextAttackTime;

	private void Start()
	{
		player = movement.target.GetComponent<PlayerTopDownController>();
	}

	private void Awake()
	{
		movement = GetComponent<EnemyMovement>();
		animatorManager = GetComponent<AnimatorManager>();
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

	// Animation events
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