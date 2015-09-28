using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour {

	public Transform target;
	public float rotationSpeed;
	public float zoomSpeed;
	public float zoomDistance;
	public float minZoomDistance;
	public float maxZoomDistance;

	private Vector3 offset;
	private float angle;

	void Start ()
	{
		// initialize the offset position
		offset = target.transform.position - transform.position;
		angle = 0.0f;
	}
	
	void LateUpdate () 
	{
		float horizontalAngle = Input.GetAxis ("Mouse X") * rotationSpeed;
		float verticalAngle = Input.GetAxis ("Mouse Y") * rotationSpeed;

		if (angle < 360.0f) {
			angle += horizontalAngle;
		} else {
			angle = 0.0f;
		}

		zoomDistance -= Input.GetAxis ("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * Mathf.Abs (zoomDistance);
		zoomDistance = Mathf.Clamp (zoomDistance, minZoomDistance, maxZoomDistance);

		// Rotate cat
		//target.Rotate (0, horizontalAngle, 0);

		// Calculate the rotation
		Quaternion rotation = Quaternion.Euler (0, angle, 0);

		// Update the position of the camera
		transform.position = target.transform.position - (rotation * offset * zoomDistance);
		//transform.position = target.transform.position - offset;

		// Fixed the camera to look at the target (player)
		transform.LookAt (target.position);
	}
}
