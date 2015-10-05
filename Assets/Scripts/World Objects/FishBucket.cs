﻿using UnityEngine;
using System.Collections;

public class FishBucket : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
      
	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.tag == "Player")
		{
			coll.GetComponent<TemporaryMovement>().numberOfFishes++;

			Destroy(this.gameObject);
		}
	}
}
