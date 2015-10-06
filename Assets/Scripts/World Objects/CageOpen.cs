using UnityEngine;
using System.Collections;

public class CageOpen : MonoBehaviour {

	private bool isOpen;
	private int id;

	// Use this for initialization
	void Start () {
		id = Animator.StringToHash ("Open");
		GetComponent<Animator>().SetBool(id, false);
		isOpen = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerStay(Collider coll)
	{
		if (coll.tag == "Player" && Input.GetButtonDown ("Interact")
		     && !isOpen)
		{
			Open ();
			coll.GetComponent<TemporaryMovement>().numberOfRescue++;
		}
	}

	public void Open()
	{
		GetComponent<Animator>().SetBool(id, true);
		isOpen = true;
	}
}
