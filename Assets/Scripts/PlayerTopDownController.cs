using System;
using UnityEngine;

public class PlayerTopDownController : MonoBehaviour
{
	InputManager inputManager;
	AnimatorManager animatorManager;
	[SerializeField] UIManager uiManager;
	[SerializeField] HealthBar healthBar;

	// public variables
	public int currentCourage;
	public int maxCourage;
	public bool holdingShovel;

	private Vector3 moveDirection;
	private Transform cameraObject;
	private Rigidbody rb;

	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float rotationSpeed = 360f;

	[SerializeField] private Collider hitBox;

	private void Awake()
	{
		inputManager = GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
		animatorManager = GetComponent<AnimatorManager>();
		cameraObject = Camera.main.transform;
		currentCourage = maxCourage;
		if (healthBar)
			healthBar.SetMaxCourage(maxCourage);
		if (hitBox)
			hitBox.enabled = false;
	}
	
	public void TakeDamage(int damage)
	{
		if (!healthBar) return;

		currentCourage -= damage;
		healthBar.SetCourage(currentCourage);
		if (currentCourage <= 0)
			uiManager.isGameOver = true;
	}

	public void HandleAllMovement()
	{
		HandleMovement();
		HandleRotation();
	}

	private void HandleMovement()
	{
		if (inputManager.movementInput.magnitude < 0.001f)
			return;

		Vector3 cameraForwardDirection = cameraObject.forward;
		cameraForwardDirection.y = 0;
		cameraForwardDirection.Normalize();

		Vector3 cameraRightDirection = cameraObject.right;
		cameraRightDirection.y = 0;
		cameraRightDirection.Normalize();

		moveDirection = (cameraForwardDirection * inputManager.verticalInput) +
		                (cameraRightDirection * inputManager.horizontalInput);

		transform.position += moveDirection * (moveSpeed * Time.deltaTime);
	}

	private void HandleRotation()
	{
		var targetDirection = cameraObject.forward * inputManager.verticalInput;
		targetDirection += cameraObject.right * inputManager.horizontalInput;
		targetDirection.Normalize();
		targetDirection.y = 0;

		if (targetDirection == Vector3.zero)
			targetDirection = transform.forward;

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
		Quaternion playerRotation =
			Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

		transform.rotation = playerRotation;
	}

	private void EnableHitBox()
	{
		hitBox.enabled = true;
	}

	private void DisableHitBox()
	{
		hitBox.enabled = false;
	}

	private void AnimationFinished(string animationBool)
	{
		Animator animator = GetComponent<Animator>();
		animator.SetBool(animationBool, false);
	}
}