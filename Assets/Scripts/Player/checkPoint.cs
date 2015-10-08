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
	private GameObject[] allBottles;
	private GameObject[] allBones;
	private GameObject[] allSpheres;
	private GameObject[] allClosets;
	private chaseTransition chaseTransScript;
	private enemyPathfinding script;
	private huntingDog hunterScript;
	private fatDogAi fatDogScript;
	private TemporaryMovement playerScript;
	private breakableObject bo;
	private instantiateDestructible destructibleSpawner;
	private instantiateKey keySpawner;
	private DoorTrigger door;
	private hidingThirdPerson closet;

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
		chaseTransScript = GameObject.Find ("BGM").GetComponent<chaseTransition>();	// get chase music transition script
		playerScript = player.GetComponent<TemporaryMovement>();

		checkpointNumber = 0;	// No checkpoint has pass through yet, therefore 0
	}
	
	void OnTriggerEnter(Collider other) // turns the check point on
    {
		// If gameobject tag is checkpoint
		if (other.gameObject.CompareTag ("Checkpoint")) 
		{
			// Increase the checkpointNumber to get which checkpoint has the player pass through
			checkpointNumber++;

			// Deactivate the gameObject of the checkpoint such that player won't collide with the checkpoint again and increase the checkpointNumber
			other.gameObject.SetActive(false);
		}
    }

    void OnCollisionEnter(Collision other) // On collision with an enemy
    {
        if ((other.gameObject.CompareTag ("Enemy") || other.gameObject.CompareTag ("HuntingDog") || other.gameObject.CompareTag ("FatDog")) )
        {
			// Stop the enemy when collided
            if (other.gameObject.CompareTag ("Enemy"))
            {
                other.gameObject.GetComponent<enemyPathfinding>().agent.velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
            else if (other.gameObject.CompareTag ("FatDog"))
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
        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        allHunters = GameObject.FindGameObjectsWithTag("HuntingDog");
        allKeys = GameObject.FindGameObjectsWithTag("Key");
        allDoors = GameObject.FindGameObjectsWithTag("Door");
        allDestructibles = GameObject.FindGameObjectsWithTag("Destructible");
		allBottles = GameObject.FindGameObjectsWithTag ("Bottle");
        allBones = GameObject.FindGameObjectsWithTag("Bone");
        allFatDogs = GameObject.FindGameObjectsWithTag("FatDog");
        allSpheres = GameObject.FindGameObjectsWithTag("SoundSphere");
		allClosets = GameObject.FindGameObjectsWithTag ("Closet");
		//resets BGM.
		chaseTransScript.resetChaseTrans(); 

//        foreach(GameObject hunter in allHunters)
//        {
//            hunterScript = (huntingDog)hunter.GetComponent<huntingDog>();
//            hunterScript.selfDestruct();
//            //Destroy(hunter);
//        }

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
		int i = 0;

//		if (allDestructibles.Length > 0) 
//		{
//			for (i = 0; i < allDestructibles.Length; i++) 
//			{
//				destructibleSpawner = allDestructibles[i].GetComponent<instantiateDestructible> ();
//				destructibleSpawner.checkpoint ();
//			}
//		}

		for (i = 0; i < allBottles.Length; i++)
		{
			if (allBottles[i].activeInHierarchy)
			{
				Debug.Log (allBottles[i]);
			}
		}

		if (allBottles.Length > 0) 
		{
			for (i = 0; i < allBottles.Length; i++)
			{
//				if (allBottles[i].GetComponent<breakableObject>() == null)
//				{
//					Debug.Log (allBottles[i]);
//				}
//				bo = allBottles[i].GetComponent<breakableObject>();
//				bo.brokenPieces.SetActive(false);
//				bo.originalObject.SetActive(true);
//				bo.GetComponent<CapsuleCollider>().enabled = true;
				allBottles[i].GetComponent<breakableObject>().Reset();
			}
		}

		if (allSpheres.Length > 0) 
		{
			for (i = 0; i < allSpheres.Length; i++)
			{
				allSpheres[i].SetActive(false);
			}
		}

		if (allBones.Length > 0) 
		{
			for (i = 0; i < allBones.Length; i++) 
			{
				player.GetComponent<TemporaryMovement> ().reduceBonePlacedNumber ();
				Destroy (allBones[i]);
			}
		}

		if (allHunters.Length > 0) 
		{
			for (i = 0; i < allHunters.Length; i++) 
			{
				hunterScript = allHunters [i].GetComponent<huntingDog> ();
				hunterScript.selfDestruct ();
			}
		}

		if (allFatDogs.Length > 0) 
		{
			for (i = 0; i < allFatDogs.Length; i++) 
			{
				fatDogScript = allFatDogs [i].GetComponent<fatDogAi> ();
				fatDogScript.Reset ();
			}
		}

		if (allEnemies.Length > 0) 
		{
			for (i = 0; i < allEnemies.Length; i++)
			{
				script = (enemyPathfinding)allEnemies[i].GetComponent<enemyPathfinding> ();
				script.agent.Stop ();
				script.agent.velocity = Vector3.zero;
				script.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				script.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
				if (script.respawnPosition != null) {
					script.transform.position = script.respawnPosition;
				}

				script.currentTarget = script.firstTarget;
				script.targetCounter = 0;

				script.agent.speed = script.patrolSpeed;
				script.stateManager (1);
				script.agent.SetDestination (script.currentTarget.position);
				script.newTargetTimer = script.defaultNewTargetTimer;
			}
		}

		if (allKeys.Length > 0) 
		{
			for (i = 0; i < allKeys.Length; i++)
			{
				keySpawner = allKeys[i].GetComponent<instantiateKey> ();
				keySpawner.checkpoint ();
			}
		}

		if (allDoors.Length > 0) 
		{
			for (i = 0; i < allDoors.Length; i++)
			{
				door = allDoors[i].GetComponent<DoorTrigger> ();
				door.checkpoint ();
			}
		}

		if (allClosets.Length > 0) 
		{
			for (i = 0; i < allClosets.Length; i++)
			{
				closet = allClosets[i].GetComponent<hidingThirdPerson>();
				closet.ResetCloset ();
			}
		}

		playerScript.resetCharacter();
		StopAllCoroutines ();
		sendBack = false;
    }
}