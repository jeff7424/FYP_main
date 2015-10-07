using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	private int id;
	private bool isStart;

	// Use this for initialization
	void Start () {
		isStart = false;
		id = Animator.StringToHash ("Start");
		GetComponent<Animator> ().SetBool (id, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//void OnTriggerEnter(Collider coll)
	void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.tag == "Enemy" && !isStart) 
		{
			GetComponent<Animator> ().SetBool (id, true);
			isStart = true;
			coll.gameObject.GetComponent<enemyPathfinding>().stateManager(9);
		}
	}
}
