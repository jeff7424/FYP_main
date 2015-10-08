using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	private int id;
	private bool isStart;
	private float m_timer;
	private float m_reserveTime;

	// Use this for initialization
	void Start () {
		m_reserveTime = 12.0f;
		m_timer = m_reserveTime;

		isStart = false;
		id = Animator.StringToHash ("Start");
		GetComponent<Animator> ().SetBool (id, false);
	}
	
	// Update is called once per frame
	void Update () {
		m_timer -= Time.deltaTime;
		if (m_timer <= 0) {
			GameObject.Find("Char_Cat").GetComponent<TemporaryMovement>().reduceTrapPlacedNumber();
			Destroy (gameObject);
		}
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
