using UnityEngine;
using System.Collections;

public class FishBucket : MonoBehaviour {
	   
	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.CompareTag ("Player"))
		{
			coll.GetComponent<TemporaryMovement>().numberOfFishes++;

			Destroy(this.gameObject);
		}
	}
}
