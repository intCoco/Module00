using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed = 5f;
	[SerializeField] private float jumpForce = 4f;
	[SerializeField] private float groundCheckDistance = 0.6f;
	[SerializeField] private Transform movementCamera;

	private Rigidbody playerRigidbody;
	private Vector2 movementInput;
	private bool jumpRequested;

	private void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody>();
		playerRigidbody.useGravity = true;
	}

	private void Update()
	{
		if (Keyboard.current == null)
		{
			movementInput = Vector2.zero;
			return;
		}

		float horizontal = 0f;
		float vertical = 0f;

		if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
		{
			horizontal -= 1f;
		}

		if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
		{
			horizontal += 1f;
		}

		if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
		{
			vertical -= 1f;
		}

		if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
		{
			vertical += 1f;
		}

		movementInput = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			jumpRequested = true;
		}
	}

	private void FixedUpdate()
	{
		Transform cameraTransform = movementCamera != null ? movementCamera : Camera.main != null ? Camera.main.transform : null;
		Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y);

		if (cameraTransform != null)
		{
			Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
			Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
			movement = cameraRight * movementInput.x + cameraForward * movementInput.y;
		}

		Vector3 velocity = playerRigidbody.linearVelocity;
		velocity.x = movement.x * speed;
		velocity.z = movement.z * speed;
		playerRigidbody.linearVelocity = velocity;

		if (jumpRequested && IsGrounded())
		{
			playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}

		jumpRequested = false;
	}

	private bool IsGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
	}
}

