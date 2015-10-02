using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour {

	[Tooltip("Target for the camera to track")]
	public Transform target;
	[Tooltip("Check if you want inverse pitch angle")]
	public bool inverseYMouse;
	[Tooltip("Check if you want smooth transition of camera movements instead of instant")]
	public bool lerpCamera;
	[Tooltip("The height of the camera from the character")]
	[Range(0.0f, 10.0f)]
	public float cameraHeight;
	[Tooltip("The sensitivity of the mouse to move the camera")]
	[Range(1.0f, 10.0f)]
	public float rotationSpeed;
	[Tooltip("The sensitivity of scroll wheel to zoom")]
	[Range(1.0f, 10.0f)]
	public float zoomSpeed;
	[Tooltip("The minimum zoom distance")]
	[Range(0.5f, 1.0f)]
	public float minZoomDistance;
	[Tooltip("The maximum zoom distance")]
	[Range(1.0f, 5.0f)]
	public float maxZoomDistance;
	[Tooltip("The highest the camera pitch can go")]
	[Range(-30.0f, -75.0f)]
	public float minPitch;
	[Tooltip("The lowest the camera pitch can go")]
	[Range(-10.0f, 10.0f)]
	public float maxPitch;
	[Tooltip("The viewing angle range when hiding inside closet")]
	[Range(90.0f, 180.0f)]
	public float hidingAngleRange;
	[Tooltip("Updating speed of the camera position (Only used if Lerp Camera is enabled)")]
	[Range(1.0f, 10.0f)]
	public float damping;
	[Tooltip("Field of view when zooming in (smaller value result in lesser things able to see)")]
	[Range(1.0f, 30.0f)]
	public float zoomInFOV;

	private Vector3 offset;
	private Vector3 heightOffset;
	private Vector3 initialPosition;
	private Vector3 hidingPosition;
	private bool isHiding;
	private float angleX;
	private float angleY;
	private float currentX;
	private float currentY;
	private float horizontalAngle;
	private float verticalAngle;
	private float zoomDistance;
	private float hidingAngle;
	private float defaultFOV;
	private Camera cam;
	private Quaternion rotation;
	private TemporaryMovement characterMovement;

	void Start ()
	{
		cam = GetComponent<Camera> ();
		defaultFOV = cam.fieldOfView;
		inverseYMouse = false;
		angleX = 0.0f;
		angleY = 0.0f;
		heightOffset.Set (0, cameraHeight, 0);
		offset = target.transform.position - transform.position;
		zoomDistance = minZoomDistance;
		initialPosition = transform.position;
		hidingPosition = Vector3.zero;
		characterMovement = target.GetComponent<TemporaryMovement> ();
	}

	void Update()
	{
		if (target) 
		{
			if (isHiding != characterMovement.playerHidden) 
			{
				isHiding = characterMovement.playerHidden;
				if (isHiding == false) 
				{
					angleX = 0.0f;
					angleY = 0.0f;
					transform.position = initialPosition;
				} 
				else 
				{
					transform.position = hidingPosition;
				}
			}
		}
	}

	void LateUpdate () 
	{
		if (target) 
		{
			// If camera is active then only update the angles from mouse / joystick
			if (cam.enabled) 
			{
				horizontalAngle = (Input.GetAxis ("Mouse X") * rotationSpeed);
				verticalAngle = (Input.GetAxis ("Mouse Y") * rotationSpeed);
			}

			// Reset the angle X if it reaches 360, else add the value of mouse movement into angle X
			if (angleX < 360.0f) 
			{
				angleX += horizontalAngle;
			} 
			else 
			{
				angleX -= 360.0f;
			}

			// Check if inverse mouse is enabled
			if (inverseYMouse == false) 
			{
				angleY += verticalAngle;
			} 
			else 
			{
				angleY -= verticalAngle;
			}

			// Limit the maximum and minimum value of the angleY
			angleY = Mathf.Clamp (angleY, minPitch, maxPitch);

			if (isHiding == false) 
			{
				// Zoom in using right click
				if (Input.GetAxis("Fire2") > 0.1f)
				{
					if (zoomDistance > 0.5f)
					{
						zoomDistance = Mathf.Lerp (zoomDistance, 0.5f, Time.deltaTime * zoomSpeed);
					}
					cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, zoomInFOV, Time.deltaTime * zoomSpeed);
				}
				else 
				{
					if (zoomDistance < 1.0f)
					{
						zoomDistance = Mathf.Lerp (zoomDistance, 1.0f, Time.deltaTime * zoomSpeed);
					}
					if (cam.fieldOfView != defaultFOV)
					{
						cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, defaultFOV, Time.deltaTime * zoomSpeed);
					}
					// Zoom using scroll wheel and limit the maximum and minimum distance
					zoomDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (zoomDistance);
					zoomDistance = Mathf.Clamp (zoomDistance, minZoomDistance, maxZoomDistance);
				}

				if (lerpCamera)
				{
					currentX = Mathf.Lerp (currentX, angleX, damping * Time.deltaTime);
					currentY = Mathf.Lerp (currentY, angleY, damping * Time.deltaTime);
					// Calculate the rotation
					rotation = Quaternion.Euler (0, currentX, currentY);
				}
				else
				{
					rotation = Quaternion.Euler (0, angleX, angleY);
				}

				// Update the position of the camera
				transform.position = target.transform.position - (rotation * offset * zoomDistance);

				// If camera height value is changed, set the height value to offset
				if (cameraHeight != heightOffset.y) {
					heightOffset.Set (0, cameraHeight, 0);
				}

				// Fixed the camera to look at the target (player) + height offset
				transform.LookAt (target.position + heightOffset);
			} 
			else 
			{
				angleX = Mathf.Clamp (angleX, hidingAngle - (hidingAngleRange * 0.5f), hidingAngle + (hidingAngleRange * 0.5f));
				rotation = Quaternion.Euler (-angleY, angleX, 0);
				transform.rotation = rotation;
			}
		}
	}

	public void SetHidingPosition(Vector3 newPosition)
	{
		initialPosition = transform.position;
		hidingPosition = newPosition;
	}

	public void ResetHidingPosition()
	{
		hidingPosition = Vector3.zero;
	}

	public void SetHidingRotation(float newRotation)
	{
		hidingAngle = newRotation;
		angleX = hidingAngle;
		rotation = Quaternion.Euler(-angleY, angleX, 0);
		transform.rotation = rotation;
	}

	public void ResetHidingRotation()
	{
		rotation = Quaternion.identity;
	}
}
