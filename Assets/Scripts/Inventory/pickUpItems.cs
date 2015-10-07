using UnityEngine;
using System.Collections;

public class pickUpItems : MonoBehaviour {

	void OnTriggerEnter (Collider pickUpObject)
	{
		if (pickUpObject.CompareTag ("player"))
		{
 			inventory.inventoryArray[0]++;

			Destroy (this.gameObject);
		}
	}
}
