using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour {

	[Tooltip("Target for the camera to track")]
	public Transform target;
	[Tooltip("Check if you want inverse pitch angle")]
	public bool inverseYMouse;
	[Tooltip("The height of the camera from the character")]
	public float cameraHeight;
	[Tooltip("The sensitivity of the mouse to move the camera")]
	public float rotationSpeed;
	[Tooltip("The sensitivity of scroll wheel to zoom")]
	public float zoomSpeed;
	[Tooltip("The minimum zoom distance")]
	public float minZoomDistance;
	[Tooltip("The maximum zoom distance")]
	public float maxZoomDistance;
	[Tooltip("The highest the camera pitch can go")]
	public float minPitch;
	[Tooltip("The lowest the camera pitch can go")]
	public float maxPitch;

	private Vector3 offset;
	private Vector3 heightOffset;
	private float angleX;
	private float angleY;
	private float horizontalAngle;
	private float verticalAngle;
	private float zoomDistance;
	private TemporaryMovement characterMovement;

	void Start ()
	{
		inverseYMouse = false;
		angleX = 0.0f;
		angleY = 0.0f;
		offset = target.transform.position - transform.position;
		zoomDistance = minZoomDistance;
		characterMovement = target.GetComponent<TemporaryMovement> ();
		heightOffset.Set (0, cameraHeight, 0);
	}
	
	void LateUpdate () 
	{
		// If camera is active then only update the angles from mouse / joystick
		if (GetComponent<Camera> ().enabled) 
		{
			horizontalAngle = Input.GetAxis ("Mouse X") * rotationSpeed;
			verticalAngle = Input.GetAxis ("Mouse Y") * rotationSpeed;
		}

		// Reset the angle X if it reaches 360, else add the value of mouse movement into angle X
		if (angleX < 360.0f) 
		{
			angleX += horizontalAngle;
		} 
		else 
		{
			angleX = 0.0f;
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

		// Zoom using scroll wheel and limit the maximum and minimum distance
		zoomDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (zoomDistance);
		zoomDistance = Mathf.Clamp (zoomDistance, minZoomDistance, maxZoomDistance);

		// Rotate cat
		// target.Rotate (0, horizontalAngle, 0);

		// Calculate the rotation
		Quaternion rotation = Quaternion.Euler (0, angleX, angleY);

		// Update the position of the camera
		transform.position = target.transform.position - (rotation * offset * zoomDistance);

		// If camera height value is changed, set the height value to offset
		if (cameraHeight != heightOffset.y) 
		{
			heightOffset.Set (0, cameraHeight, 0);
		}

		// Fixed the camera to look at the target (player) + height offset
		transform.LookAt (target.position + heightOffset);
	}
}
