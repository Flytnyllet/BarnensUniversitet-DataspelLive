using System;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
	private int slåMedSPade = Animator.StringToHash("Slå");
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

	public void SlåMedSpade(bool slå)
	{
		animator.SetBool(slåMedSPade, slå);
	}
}