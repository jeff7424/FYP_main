using UnityEngine;
using System.Collections;


public class soundSphere : MonoBehaviour 
{
    enemyPathfinding script;
    Vector3 scalingRate = new Vector3(1.0f, 0.125f, 1.0f);
    public float maxDiameter;
    RaycastHit hit;
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.localScale += scalingRate;
        if (this.transform.localScale.x >= maxDiameter)
            Destroy(this.gameObject);
	}

    void OnTriggerEnter(Collider other)
    {
        //----------------------------------------------------------------------------//
        // if an enemy enters the sound sphere, this code sends a message to the enemy//
        //----------------------------------------------------------------------------//
        if (other.gameObject.CompareTag ("Enemy") == true) 
		{
			script = other.GetComponent<enemyPathfinding> ();
			if (script != null) 
			{
				if (this.transform.parent != other.transform)
				{
					if (script.States != enumStates.chase && script.States != enumStates.distracted && script.States != enumStates.eatBone) 
					{
						if (script.soundSource) 
						{
							if (script.soundSource.CompareTag ("Enemy") == false || script.soundSource.CompareTag ("FatDog") == false) 
							{
								if (transform.parent.gameObject.CompareTag ("Enemy") || transform.parent.gameObject.CompareTag ("FatDog"))
								{
									script.stateManager (6);
									script.soundSource = transform.parent.gameObject;
								}
							}
						} 
						else if (script.soundSource == null) 
						{
							Physics.Linecast (transform.parent.position, other.transform.position, out hit);
							if (transform.parent.gameObject.CompareTag ("Enemy") || transform.parent.gameObject.CompareTag("FatDog") || hit.collider.CompareTag ("Enemy") || hit.collider.CompareTag ("FatDog"))
							{
								script.stateManager (6);
								script.soundSource = transform.parent.gameObject;
							}
						}
						if (transform.parent.gameObject != null) 
						{
							script._soundSource = transform.parent.gameObject.transform.position;
						} 
					}
				}
			}
		}
		else if (other.gameObject.CompareTag ("FatDog") == true)
		{
	        fatDogAi fatDogScript = other.GetComponent<fatDogAi>();
	        if (fatDogScript != null)
	        {
	            if (this.transform.parent != other.transform)
				{
	                if (fatDogScript.States != enumStatesFatDog.chase && fatDogScript.States != enumStatesFatDog.distracted && fatDogScript.States != enumStatesFatDog.eatBone)
	                
	                    if (fatDogScript.soundSource != null)
	                    {
	                        if (fatDogScript.soundSource.CompareTag ("Enemy") == false || fatDogScript.soundSource.CompareTag ("FatDog") == false)
	                        {
	                            if (transform.parent.gameObject.CompareTag ("Enemy") == true || transform.parent.gameObject.CompareTag ("FatDog") == true)
	                            {
	                                fatDogScript.stateManager(6);
	                                fatDogScript.soundSource = transform.parent.gameObject;
	                            }
	                        }
	                    }
	                    else if (fatDogScript.soundSource == null)
	                    {
	                        Physics.Linecast(transform.parent.position, other.transform.position, out hit);
	                        if (transform.parent.gameObject.CompareTag ("Enemy") == true || transform.parent.gameObject.CompareTag ("FatDog") == true || hit.collider.CompareTag ("Enemy") == true || hit.collider.CompareTag ("FatDog") == true)
	                        {
	                            fatDogScript.stateManager(6);
	                            fatDogScript.soundSource = transform.parent.gameObject;
	                        }
	                    }
	                    //fatDogScript.stateManager(6);
	                    //fatDogScript.soundSource = transform.parent.gameObject;
	                }
				}
			}
        }

    public void setMaxDiameter(float value)
    {
        maxDiameter = value;
    }
}
