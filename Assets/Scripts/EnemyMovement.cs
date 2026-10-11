using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
	AnimatorManager animatorManager;
	public float vertical;
	float moveAmount;

	public bool stunned;
	public Transform target;
	private NavMeshAgent agent;
	private EnemyAttack enemyAttack;

	[Header("Ghost Mode (leave off for normal enemies)")]
	[SerializeField] private bool isGhost = false;
	[SerializeField] private float ghostSpeed = 3f;
	[SerializeField] private float ghostAcceleration = 8f;
	[Tooltip("If true, the ghost floats toward the player's height. If false, it stays at its starting height.")]
	[SerializeField] private bool ghostFollowsHeight = false;

	[Header("Chasing")] [SerializeField] private float stopDistance = 1.0f;
	[SerializeField] private float resumeBuffer = 0.2f;
	[SerializeField] private float turnSpeed = 360f;

	[Header("Fleeing")] [SerializeField] private float fleeDistance = 10f;
	[SerializeField] private float fleeSpeedMultiplier = 2f;

	[Header("Knockback")] [SerializeField] private float knockbackDamping = 6f;

	private float defaultSpeed;
	private float stunTimer;
	private float fleeTimer;
	private Vector3 knockbackVelocity;
	private bool chasing = true;

	// Ghost-only state
	private Vector3 ghostVelocity;
	private Vector3 ghostDesiredVelocity;
	private float ghostStartY;

	public bool IsFleeing => fleeTimer > 0f;
	private float NormalSpeed => IsFleeing ? defaultSpeed * fleeSpeedMultiplier : defaultSpeed;
	private float GhostMaxSpeed => IsFleeing ? ghostSpeed * fleeSpeedMultiplier : ghostSpeed;

	private void Awake()
	{
		animatorManager = GetComponent<AnimatorManager>();
		enemyAttack = GetComponent<EnemyAttack>();
		agent = GetComponent<NavMeshAgent>();

		if (isGhost)
		{
			ghostStartY = transform.position.y;

			// Disable the agent so it can't snap the ghost back onto the navmesh.
			if (agent) agent.enabled = false;

			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb)
			{
				rb.isKinematic = true;
				rb.useGravity = false;
			}

			// Triggers never block movement but can still detect overlaps.
			foreach (Collider col in GetComponentsInChildren<Collider>())
				col.isTrigger = true;
		}
		else
		{
			defaultSpeed = agent.speed;
		}
	}

	private void Update()
	{
		if (isGhost)
		{
			ghostDesiredVelocity = Vector3.zero;
			HandleStun();
			HandleFlee();
			GhostFlee();
			GhostChase();
			GhostApplyMovement();
			HandleKnockback();
		}
		else
		{
			HandleStun();
			HandleFlee();
			HandleKnockback();
			HandleChase();
		}

		if (animatorManager)
			HandleAnimator();
	}

	public void Stun(float duration)
	{
		stunned = true;
		stunTimer = duration;

		if (isGhost)
			ghostVelocity = Vector3.zero;
		else
			agent.speed = 0;
	}

	public void ApplyKnockback(Vector3 direction, float force)
	{
		direction.y = 0f;
		knockbackVelocity = direction.normalized * force;
	}

	public void Flee(float duration)
	{
		fleeTimer = duration;

		if (isGhost) return; // Ghost flees by floating directly away (see GhostFlee)

		if (!stunned)
			agent.speed = NormalSpeed;

		agent.isStopped = false;
		agent.SetDestination(FindFleePoint());
	}

	// ---------------------------------------------------------------
	// Normal (NavMesh) enemy logic - unchanged
	// ---------------------------------------------------------------

	private Vector3 FindFleePoint()
	{
		Vector3 best = transform.position;
		float bestDistance = -1f;
		NavMeshPath path = new NavMeshPath();

		for (int i = 0; i < 12; i++)
		{
			Vector2 offset = Random.insideUnitCircle * fleeDistance;
			Vector3 candidate = transform.position + new Vector3(offset.x, 0f, offset.y);

			if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas)) continue;
			if (!agent.CalculatePath(hit.position, path) || path.status != NavMeshPathStatus.PathComplete) continue;

			float distance = target ? (hit.position - target.position).sqrMagnitude : 0f;
			if (distance > bestDistance)
			{
				bestDistance = distance;
				best = hit.position;
			}
		}

		return best;
	}

	private void HandleFlee()
	{
		if (!IsFleeing) return;

		fleeTimer -= Time.deltaTime;
		if (fleeTimer <= 0f)
		{
			fleeTimer = 0f;
			if (!isGhost && !stunned)
				agent.speed = defaultSpeed;
		}
	}

	private void HandleChase()
	{
		if (stunned || IsFleeing || !target) return;

		Vector3 toTarget = target.position - transform.position;
		toTarget.y = 0f;
		float distance = toTarget.magnitude;

		if (chasing && distance <= stopDistance)
			chasing = false;
		else if (!chasing && distance > stopDistance + resumeBuffer)
			chasing = true;

		bool attacking = enemyAttack && enemyAttack.IsAttacking;
		bool shouldMove = chasing && !attacking;

		agent.isStopped = !shouldMove;

		if (shouldMove)
		{
			agent.SetDestination(target.position);
		}
		else if (toTarget.sqrMagnitude > 0.001f)
		{
			Quaternion lookRotation = Quaternion.LookRotation(toTarget);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
		}
	}

	// ---------------------------------------------------------------
	// Ghost logic - ignores NavMesh and walls
	// ---------------------------------------------------------------

	private void GhostFlee()
	{
		if (!IsFleeing || stunned || !target) return;

		Vector3 away = transform.position - target.position;
		if (!ghostFollowsHeight) away.y = 0f;
		if (away.sqrMagnitude < 0.001f) away = transform.forward;

		ghostDesiredVelocity = away.normalized * GhostMaxSpeed;
		GhostFace(away);
	}

	private void GhostChase()
	{
		if (stunned || IsFleeing || !target) return;

		Vector3 toTarget = target.position - transform.position;
		if (!ghostFollowsHeight) toTarget.y = 0f;
		float distance = toTarget.magnitude;

		if (chasing && distance <= stopDistance)
			chasing = false;
		else if (!chasing && distance > stopDistance + resumeBuffer)
			chasing = true;

		bool attacking = enemyAttack && enemyAttack.IsAttacking;
		bool shouldMove = chasing && !attacking;

		if (shouldMove)
			ghostDesiredVelocity = toTarget.normalized * GhostMaxSpeed;

		GhostFace(toTarget);
	}

	private void GhostApplyMovement()
	{
		ghostVelocity = Vector3.MoveTowards(ghostVelocity, ghostDesiredVelocity, ghostAcceleration * Time.deltaTime);
		Vector3 newPos = transform.position + ghostVelocity * Time.deltaTime;

		if (!ghostFollowsHeight)
			newPos.y = ghostStartY;

		transform.position = newPos;
	}

	private void GhostFace(Vector3 dir)
	{
		dir.y = 0f;
		if (dir.sqrMagnitude < 0.001f) return;

		Quaternion lookRotation = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
	}

	// ---------------------------------------------------------------
	// Shared
	// ---------------------------------------------------------------

	private void HandleStun()
	{
		if (!stunned) return;

		stunTimer -= Time.deltaTime;
		if (stunTimer <= 0f)
		{
			if (animatorManager)
				animatorManager.StunAnimation(false);
			stunned = false;

			if (!isGhost)
				agent.speed = defaultSpeed;
		}
	}

	private void HandleKnockback()
	{
		if (knockbackVelocity.sqrMagnitude < 0.01f)
		{
			knockbackVelocity = Vector3.zero;
			return;
		}

		if (isGhost)
			transform.position += knockbackVelocity * Time.deltaTime;
		else
			agent.Move(knockbackVelocity * Time.deltaTime);

		knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDamping * Time.deltaTime);
	}

	private void HandleAnimator()
	{
		if (stunned)
		{
			animatorManager.UpdateAnimatorValues(0, 0);
			animatorManager.StunAnimation(true);
			return;
		}

		if (isGhost)
		{
			vertical = ghostVelocity.magnitude;
			moveAmount = Mathf.Clamp01(vertical / ghostSpeed);
		}
		else
		{
			vertical = agent.velocity.magnitude;
			moveAmount = Mathf.Clamp01(vertical / defaultSpeed);
		}

		animatorManager.UpdateAnimatorValues(0, moveAmount);
	}
}