using UnityEngine;

public class PlayerTopDownController : MonoBehaviour
{
	InputManager inputManager;

	private Vector3 moveDirection;
	private Rigidbody rb;

	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float rotationSpeed = 360f;

	private void Awake()
	{
		inputManager = GetComponent<InputManager>();
		rb = GetComponent<Rigidbody>();
	}

	public void HandleAllMovement()
	{
		HandleMovement();
	}

	private void HandleMovement()
	{
		if (inputManager.movementInput.magnitude < 0.001f)
			return;
		
		Vector3 cameraForwardDirection = Camera.main.transform.forward;
		cameraForwardDirection.y = 0;
		cameraForwardDirection.Normalize();
		
		Vector3 cameraRightDirection = Camera.main.transform.right;
		cameraRightDirection.y = 0;
		cameraRightDirection.Normalize();
		
		moveDirection = (cameraForwardDirection * inputManager.verticalInput) + (cameraRightDirection * inputManager.horizontalInput);
		
		transform.position += moveDirection * (moveSpeed * Time.deltaTime);
	}
}