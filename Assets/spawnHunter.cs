using UnityEngine;
using System.Collections;

public class spawnHunter : MonoBehaviour {
    public GameObject huntingDog;
    public int spawnedHunters;
    public bool spawnhunter;
    public Transform spawnLocation;
	public GameObject alertArea;
	private GameObject newDog;
	private Animator doorAnimator;

	void Start () 
	{
        if(spawnLocation == null)
        {
            spawnLocation = this.transform;
        }
		doorAnimator = GetComponentInParent<Animator> ();
	}

    void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag ("soundSphere"))
        {
            if (other.transform.parent.CompareTag ("bone") == false)
            {
                if (spawnedHunters < 1)
                {
                    newDog = (GameObject)Instantiate(huntingDog, spawnLocation.transform.position, Quaternion.identity);
					if (alertArea != null)
					{
						newDog.GetComponent<huntingDog>().setAlertArea(alertArea);
					}
                    newDog.transform.parent = transform;
                    spawnedHunters++;
					doorAnimator.SetBool("DoorOpen", true);
                }
            }
        }
	}

	public void CloseDoor() 
	{
		doorAnimator.SetBool ("DoorOpen", false);
	}
}
