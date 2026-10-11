using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float distance = 6f;
	[SerializeField] private float height = 1.5f;
	[SerializeField] private float sensitivity = 0.15f;
	[SerializeField] private float minPitch = -30f;
	[SerializeField] private float maxPitch = 75f;
	[SerializeField] private float collisionRadius = 0.25f;
	[SerializeField] private float minimumDistance = 0.5f;
	[SerializeField] private LayerMask collisionLayers = ~0;

	private float yaw;
	private float pitch;

	private void Awake()
	{
		if (target == null)
		{
			PlayerController player = FindAnyObjectByType<PlayerController>();
			target = player != null ? player.transform : null;
		}

		if (target == null)
		{
			Debug.LogWarning("CameraOrbit requires a target Transform or a PlayerController in the scene.", this);
			return;
		}

		Vector3 offset = transform.position - (target.position + Vector3.up * height);
		distance = Mathf.Max(0.01f, offset.magnitude);
		yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
		pitch = Mathf.Clamp(Mathf.Asin(offset.y / distance) * Mathf.Rad2Deg, minPitch, maxPitch);
	}

	private void OnEnable()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	private void Update()
	{
		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void LateUpdate()
	{
		if (target == null)
		{
			return;
		}

		if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
		{
			Vector2 mouseDelta = Mouse.current.delta.ReadValue();
			yaw += mouseDelta.x * sensitivity;
			pitch = Mathf.Clamp(pitch - mouseDelta.y * sensitivity, minPitch, maxPitch);
		}

		Vector3 targetPosition = target.position + Vector3.up * height;
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
		Vector3 desiredOffset = rotation * (Vector3.back * distance);
		Vector3 cameraPosition = targetPosition + desiredOffset;
		Vector3 direction = desiredOffset.normalized;
		RaycastHit[] hits = Physics.SphereCastAll(
			targetPosition,
			collisionRadius,
			direction,
			desiredOffset.magnitude,
			collisionLayers,
			QueryTriggerInteraction.Ignore);

		float availableDistance = desiredOffset.magnitude;
		bool collisionDetected = false;
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform == target || hit.transform.IsChildOf(target))
			{
				continue;
			}

			collisionDetected = true;
			availableDistance = Mathf.Min(availableDistance, hit.distance - collisionRadius);
		}

		availableDistance = collisionDetected
			? Mathf.Max(0.01f, availableDistance)
			: Mathf.Max(minimumDistance, availableDistance);
		transform.position = targetPosition + direction * availableDistance;
		transform.LookAt(targetPosition);
	}
}
