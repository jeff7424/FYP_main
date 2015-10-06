using UnityEngine;
using System.Collections;

public class CageOpen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int id = Animator.StringToHash ("Open");
		GetComponent<Animator>().SetBool(id, false);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerStay(Collider coll)
	{
		if (coll.tag == "Player" && Input.GetButtonDown ("Interact"))
		{
			Open ();
		}
	}

	public void Open()
	{
		int id = Animator.StringToHash ("Open");
		GetComponent<Animator>().SetBool(id, true);
	}
}
