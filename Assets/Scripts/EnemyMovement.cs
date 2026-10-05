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

    [SerializeField] private float knockbackDamping; 

    private float defaultSpeed;
    private float stunTimer;
    private Vector3 knockbackVelocity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorManager = GetComponent<AnimatorManager>();
        defaultSpeed = agent.speed;
    }

    private void Update()
    {
        HandleStun();
        HandleKnockback();

        if (!stunned)
            agent.SetDestination(target.position);

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

    private void HandleStun()
    {
        if (!stunned) return;

        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0f)
        {
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
            return;
        }

        vertical = agent.speed;
        moveAmount = Mathf.Clamp01(Mathf.Abs(vertical));
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }
}