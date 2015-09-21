using UnityEngine;
using System.Collections;

public class alertArea : MonoBehaviour 
{
    enemyPathfinding script;
    huntingDog scriptHunter;

    void OnTriggerEnter(Collider other)
    {
		// Send the alert area waypoints to enemies
        if (other.GetComponent<Collider>().CompareTag("enemy"))
        {
            script = other.gameObject.GetComponent<enemyPathfinding>();
			script.setAlertArea(this.gameObject); 
		}
        else if(other.GetComponent<Collider>().CompareTag("huntingDog"))
        {
            scriptHunter = other.gameObject.GetComponent<huntingDog>();
            scriptHunter.setAlertArea(this.gameObject);
        }      
    }
}
