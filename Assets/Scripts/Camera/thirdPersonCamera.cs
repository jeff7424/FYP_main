using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour {

	public Transform target;
	public bool inverseYMouse;
	public float rotationSpeed;
	public float zoomSpeed;
	public float zoomDistance;
	public float minZoomDistance;
	public float maxZoomDistance;
	public float minPitch;
	public float maxPitch;

	private Vector3 offset;
	private float angleX;
	private float angleY;

	void Start ()
	{
		// initialize the offset position
		offset = target.transform.position - transform.position;
		inverseYMouse = false;
		angleX = 0.0f;
		angleY = 0.0f;
	}
	
	void LateUpdate () 
	{
		float horizontalAngle = Input.GetAxis ("Mouse X") * rotationSpeed;
		float verticalAngle = Input.GetAxis ("Mouse Y") * rotationSpeed;

		if (angleX < 360.0f) {
			angleX += horizontalAngle;
		} else {
			angleX = 0.0f;
		}
		if (inverseYMouse == false) 
		{
			angleY += verticalAngle;
		} else 
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

		// Fixed the camera to look at the target (player)
		transform.LookAt (target.position);
	}
}
