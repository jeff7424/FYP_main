using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class ladderClimbing : MonoBehaviour
{
	private TemporaryMovement characterMovement;

	public Transform character;
	public bool inside = false;
	public float heightFactor;
		
	void Start()
	{
		characterMovement = character.GetComponent<TemporaryMovement>();
	}
		
	void OnTriggerEnter(Collider player)
	{
		if (player.gameObject.CompareTag ("Player"))
		{
			characterMovement.onLadder = true;
			characterMovement.rb.useGravity = false;
			inside = true;
		}
	}
		
	void OnTriggerExit(Collider ladder)
	{
       // ladder.GetComponent<TemporaryMovement>().onLadder = false;
		if (ladder.gameObject.CompareTag ("Player"))
		{
			characterMovement.onLadder = false;
			characterMovement.rb.useGravity = true;
            inside = false;
		}
	}
		
	void Update()
	{
		if (inside == true && characterMovement.movement.magnitude > 0.01f)
		{
			characterMovement.transform.position += Vector3.up * characterMovement.movementSpeed * Time.deltaTime * characterMovement.movement.magnitude;
		}
        //print("MAGNITUDE: " + characterController.GetComponent<TemporaryMovement>().movement.magnitude);
	}
}
