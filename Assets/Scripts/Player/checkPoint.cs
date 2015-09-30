/* 
 * To put inside player's gameObject.
 * respawnPosition = variable that needs to be added in the enemy script.
 * An empty gameObject set at the position of the check point has to be added to the scene.
 * A trigger has to be added to the scene to turn the check point on with a box collider (trigger set to on) and tag == "checkPoint".
*/

using UnityEngine;
using System.Collections;

public class checkPoint: MonoBehaviour
{
	public bool sendBack;

	private GameObject player;
    private GameObject[] allEnemies; // needed to reset enemies' positions
    private GameObject[] allHunters; // Hunters need to be destroyed on player death
    private GameObject[] allFatDogs;
	private GameObject[] allKeys;
	private GameObject[] allDoors;
	private GameObject[] allDestructibles;
	private GameObject[] allBones;
	private GameObject[] allSpheres;
	private GameObject[] allClosets;
	private Rigidbody rb;
	private chaseTransition chaseTransScript;
	private enemyPathfinding script;
	private huntingDog hunterScript;
	private fatDogAi fatDogScript;
	private TemporaryMovement playerScript;
	private breakableObject bo;

	// Working on new checkpoint system with more flexibility
	// For checkpointPosition, please insert startingPoint in the first column in the array
	// *NOTE: Insert position, not the trigger 
	[Tooltip("Insert the start position and the checkpoints position here. Start position is a must to prevent errors")]
	public GameObject[] checkpointPosition;	// Store all the checkpoint spawning positions into this array
	private int checkpointNumber;			// Use as index to get from the checkpointPosition array

    void Start()
    {	
		sendBack = false;

		player = this.gameObject;
		rb = GetComponent<Rigidbody> ();
		chaseTransScript = GameObject.Find ("BGM").GetComponent<chaseTransition>();	// get chase music transition script
		playerScript = player.GetComponent<TemporaryMovement>();

		checkpointNumber = 0;	// No checkpoint has pass through yet, therefore 0
	}
	
	void OnTriggerEnter(Collider other) // turns the check point on
    {
		// If gameobject tag is checkpoint
		if (other.gameObject.CompareTag ("checkPoint")) 
		{
			// Increase the checkpointNumber to get which checkpoint has the player pass through
			checkpointNumber++;

			// Deactivate the gameObject of the checkpoint such that player won't collide with the checkpoint again and increase the checkpointNumber
			other.gameObject.SetActive(false);
		}
    }

    void OnCollisionEnter(Collision other) // On collision with an enemy
    {
        if ((other.gameObject.CompareTag ("enemy") || other.gameObject.CompareTag ("huntingDog") || other.gameObject.CompareTag ("fatDog")) )
        {
			// Stop the enemy when collided
            if (other.gameObject.CompareTag ("enemy"))
            {
                other.gameObject.GetComponent<enemyPathfinding>().agent.velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            else if (other.gameObject.CompareTag ("fatDog"))
            {
                other.gameObject.GetComponent<fatDogAi>().agent.velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            this.transform.position = checkpointPosition[checkpointNumber].transform.position;
            resetLevel();
        }
    }
	
    void resetLevel()
    {
		//==============================================//
		//	Change for each to for loop to reduce GC	//
		//==============================================//
        sendBack = true;

		// Get all the objects from the scene using tag (Might have performance impact, if have time find a better solution)
        allEnemies = GameObject.FindGameObjectsWithTag("enemy");
        allHunters = GameObject.FindGameObjectsWithTag("huntingDog");
        allKeys = GameObject.FindGameObjectsWithTag("key");
        allDoors = GameObject.FindGameObjectsWithTag("door");
        allDestructibles = GameObject.FindGameObjectsWithTag("destructible");
        allBones = GameObject.FindGameObjectsWithTag("bone");
        allFatDogs = GameObject.FindGameObjectsWithTag("fatDog");
        allSpheres = GameObject.FindGameObjectsWithTag("soundSphere");
		allClosets = GameObject.FindGameObjectsWithTag ("Closet");
		//resets BGM.
		chaseTransScript.resetChaseTrans(); 

//        foreach(GameObject hunter in allHunters)
//        {
//            hunterScript = (huntingDog)hunter.GetComponent<huntingDog>();
//            hunterScript.selfDestruct();
//            //Destroy(hunter);
//        }
		if (allHunters.Length > 0) 
		{
			for (int i = 0; i < allHunters.Length; i++) 
			{
				hunterScript = allHunters [i].GetComponent<huntingDog> ();
				hunterScript.selfDestruct ();
			}
		}
//        foreach(GameObject fatDog in allFatDogs)
//        {
//            fatDogScript = (fatDogAi)fatDog.GetComponent<fatDogAi>();
////            fatDogScript.agent.Stop();
////            fatDogScript.agent.velocity = Vector3.zero;
////            fatDogScript.GetComponent<Rigidbody>().velocity = Vector3.zero;
////            fatDogScript.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
////            if (fatDogScript.respawnPosition != null)
////            {
////                fatDogScript.transform.position = fatDogScript.respawnPosition;
////            }
////            fatDogScript.stateManager(4);
//			// Reset func contains everything above
//			fatDogScript.Reset ();
//        }
		foreach (GameObject sphere in allSpheres)
		{
			Destroy(sphere);
		}
		for (int i = 0; i < allFatDogs.Length; i++) 
		{
			fatDogScript = allFatDogs[i].GetComponent<fatDogAi>();
			fatDogScript.Reset ();
		}
        foreach (GameObject enemy in allEnemies)
        {
			script = (enemyPathfinding)enemy.GetComponent<enemyPathfinding>();
            script.agent.Stop();
            script.agent.velocity = Vector3.zero;
            script.GetComponent<Rigidbody>().velocity = Vector3.zero;
            script.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            if (script.respawnPosition != null)
            {
                enemy.transform.position = script.respawnPosition;
            }

            script.currentTarget = script.firstTarget;
            script.targetCounter = 0;

            script.agent.speed = script.patrolSpeed;
            script.stateManager(1);
            script.agent.SetDestination(script.currentTarget.position);
            script.newTargetTimer = script.defaultNewTargetTimer;
        }

        foreach(GameObject key in allKeys)
        {
            instantiateKey Key = key.GetComponent<instantiateKey>();
            Key.checkpoint();
        }

        foreach(GameObject bone in allBones)
        {
            //bo = bone.gameObject.GetComponent<breakableObject>();
			player.GetComponent<TemporaryMovement>().reduceBonePlacedNumber ();
            Destroy(bone);
        }

        foreach (GameObject destructible in allDestructibles)
        {
            instantiateDestructible DES = destructible.GetComponent<instantiateDestructible>();
            DES.checkpoint();
        }

        foreach (GameObject door in allDoors)
        {
            DoorTrigger dt = door.GetComponent<DoorTrigger>();
            dt.checkpoint();
        }       
		foreach (GameObject closet in allClosets) 
		{
			closet.GetComponent<hidingThirdPerson>().ResetCloset();
		}
		playerScript.resetCharacter();
		StopAllCoroutines ();
		sendBack = false;
    }
}