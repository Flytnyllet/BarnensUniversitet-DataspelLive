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

    [Header("Chasing")]
    [SerializeField] private float stopDistance = 1.0f;
    [SerializeField] private float resumeBuffer = 0.2f;
    [SerializeField] private float turnSpeed = 360f;

    [Header("Fleeing")]
    [SerializeField] private float fleeDistance = 10f;
    [SerializeField] private float fleeSpeedMultiplier = 2f;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackDamping = 6f;

    private float defaultSpeed;
    private float stunTimer;
    private float fleeTimer;
    private Vector3 knockbackVelocity;
    private bool chasing = true;

    public bool IsFleeing => fleeTimer > 0f;
    private float NormalSpeed => IsFleeing ? defaultSpeed * fleeSpeedMultiplier : defaultSpeed;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorManager = GetComponent<AnimatorManager>();
        enemyAttack = GetComponent<EnemyAttack>();
        defaultSpeed = agent.speed;
    }

    private void Update()
    {
        HandleStun();
        HandleFlee();
        HandleKnockback();
        HandleChase();

        if (animatorManager)
            HandleAnimator();
    }

    public void Stun(float duration)
    {
        stunned = true;
        stunTimer = duration;
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
 
        if (!stunned)
            agent.speed = NormalSpeed;
 
        agent.isStopped = false;
        agent.SetDestination(FindFleePoint());
    }
 
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
            if (!stunned)
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

    private void HandleStun()
    {
        if (!stunned) return;

        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
            animatorManager.StunAnimation(false);
            stunned = false;
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

        vertical = agent.velocity.magnitude;
        moveAmount = Mathf.Clamp01(vertical / defaultSpeed);
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
}