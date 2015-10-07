
﻿using UnityEngine;
using System.Collections;

public class rtsCam : MonoBehaviour {

	public GameObject target = null;
	public bool orbitY = false;

	void Start()
	{
	}

	// Update is called once per frame
	void Update () 
	{
		float mousePosX = Input.mousePosition.x;
		float mousePosY = Input.mousePosition.y;

		int scrollDistance = 100;
		float scrollSpeed = 2 * Camera.main.orthographicSize + 2;

		float scrollAmount = scrollSpeed * Time.deltaTime;

		const float orthographicSizeMin = 5f;
		const float orthographicSizeMax = 256f;

		/////////////////////////////////////////////////////////////////////////////////////
										     //MOUSE//
		/////////////////////////////////////////////////////////////////////////////////////

		// Mouse Left
		if ((mousePosX < scrollDistance) && (transform.position.x > -240))
		{ 
			transform.Translate (-scrollAmount,0,0, Space.World); 
		} 
		//	Mouse Right
		if ((mousePosX >= Screen.width - scrollDistance) && (transform.position.x < 240))
		{ 
			transform.Translate (scrollAmount,0,0, Space.World);  
		}
		// Mouse Down
		if ((mousePosY < scrollDistance) && (transform.position.z > -240))
		{ 
			transform.Translate (0,0,-scrollAmount, Space.World); 
		} 
		// Mouse Up
		if ((mousePosY >= Screen.height - scrollDistance) && (transform.position.z < 240))
		{ 
			transform.Translate (0,0,scrollAmount, Space.World); ; 
		}

		/////////////////////////////////////////////////////////////////////////////////////
											//SCROLLING//
		//////////////////////////////////////////////////////////////////////////////////// 
	
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax );
	

	}
}
