using System;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
	private int attackAnim = Animator.StringToHash("Attack");
	private Animator animator;
	private int horizontal;
	private int vertical;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		horizontal = Animator.StringToHash("Horizontal");
		vertical = Animator.StringToHash("Vertical");
	}

	public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
	{
		animator.SetFloat(horizontal, horizontalMovement, 0.1f, Time.deltaTime);
		animator.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime);
	}

	public void AttackAnimation(bool attack)
	{
		animator.SetBool(attackAnim, attack);
	}
}