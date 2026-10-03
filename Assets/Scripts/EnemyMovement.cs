using System;
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
	
	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		animatorManager = GetComponent<AnimatorManager>();
	}

	private void Update()
	{
		agent.SetDestination(target.position);
		if (animatorManager)
			HandleAnimator();
	}

	private void HandleAnimator()
	{
		if (stunned)
		{
			agent.speed = 0;
			return;
		}
		
		vertical = agent.speed;
		moveAmount = Mathf.Clamp01(Mathf.Abs(vertical));
		animatorManager.UpdateAnimatorValues(0, moveAmount);
	}
	
	
}