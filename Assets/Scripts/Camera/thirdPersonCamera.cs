using UnityEngine;
using System.Collections;

public class thirdPersonCamera : MonoBehaviour {

	public Transform target;
	public float rotationSpeed;

	private Vector3 offset;

	void Start ()
	{
		// initialize the offset position
		offset = target.transform.position - transform.position;
	}
	
	void LateUpdate () 
	{
		float horizontalAngle = Input.GetAxis ("Mouse X") * rotationSpeed;
		float verticalAngle = Input.GetAxis ("Mouse Y") * rotationSpeed;

		// Rotate cat
		//target.Rotate (0, horizontalAngle, 0);

		// Calculate the rotation
		Quaternion rotation = Quaternion.Euler (0, target.transform.eulerAngles.y, 0);

		// Update the position of the camera
		// transform.position = target.transform.position - (rotation * offset);

		// Fixed the camera to look at the target (player)
		transform.LookAt (target);
	}
}
