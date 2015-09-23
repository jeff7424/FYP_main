using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstructionDetector : MonoBehaviour {

	public float rad = 0.5f;
	public Transform playerTransform;
	//private Wall m_LastWall;
	private List<Wall> lastWalls = new List<Wall>();
    Vector3 direction;

	void FixedUpdate()
	{
		// If the list is not empty
		if (lastWalls != null) {
			// Set all the walls to normal
			foreach (Wall wall in lastWalls) {
				wall.SetToNormal ();
			}
			// Remove all walls from the list
			lastWalls.Clear();
		}

        direction = (playerTransform.position - Camera.main.transform.position).normalized;

		RaycastHit[] rayCastHit;
		
		rayCastHit = Physics.RaycastAll(Camera.main.transform.position, direction, (playerTransform.position - Camera.main.transform.position).magnitude);

		for (int i = 0; i < rayCastHit.Length; i++) 
		{
			RaycastHit hit = rayCastHit[i];

			if (hit.transform.CompareTag ("wall"))
			{
				Wall wall = hit.transform.GetComponent<Wall>();
				if (wall)
				{
					wall.SetTransparent();
					lastWalls.Add(wall);
				}
			}
		}
	}
	
	/*void Start ()
	{
		StartCoroutine (DetectPlayerObstructions());
	}
	
	IEnumerator DetectPlayerObstructions()
	{
		while (true) 
		{
			yield return new WaitForSeconds(0.1f);

			Vector3 direction = (playerTransform.position - Camera.main.transform.position).normalized;
			RaycastHit rayCastHit;

			if (Physics.Raycast(Camera.main.transform.position, direction, out rayCastHit, Mathf.Infinity))
			{
				Wall wall = rayCastHit.transform.GetComponent<Wall>();
				if (wall)
				{
					wall.SetTransparent();
					m_LastWall = wall;
				}
				//Working
				else 
				{
					if (m_LastWall)
					{
						m_LastWall.SetToNormal();
						m_LastWall = null;
					}
				//Working
				}
			}
		}
	}*/
}
