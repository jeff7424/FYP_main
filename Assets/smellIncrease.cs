using UnityEngine;
using System.Collections;

public class smellIncrease : MonoBehaviour {

    public float value;
    GameObject player;
    ringOfSmell script;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("player");
        script = player.GetComponentInChildren<ringOfSmell>();
	}

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag ("player") || other.gameObject.CompareTag ("smell"))
        {
            script.increaseSmell(value);
        }
    }

    void OnTriggerExit()
    {
        script.radius = Mathf.Floor(script.radius);
    }
}
