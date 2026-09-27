using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	private PlayerControls playerControls;
	public float verticalInput;
	public float horizontalInput;

	public Vector2 movementInput;

	private void OnEnable()
	{
		if (playerControls == null)
		{
			playerControls = new PlayerControls();
			
			playerControls.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
		}
		
		playerControls.Enable();
	}

	private void OnDisable()
	{
		playerControls.Disable();
	}

	public void HandleAllInputs()
	{
		HandleMovementInput();
	}
	
	private void HandleMovementInput()
	{
		verticalInput = movementInput.y;
		horizontalInput = movementInput.x;
	}
}