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
	public bool checkPointActivated = false; // if the check point has been reached or not
	public bool checkPointActivated_2 = false;
	public bool sendBack;
    public GameObject checkPointPosition; // Position of the check point
    public GameObject checkPointPosition_2; // Position of the check point
	public GameObject player;
    public Transform startPosition;
    
    private string currentLevel; // the current level
    private GameObject[] allEnemies; // needed to reset enemies' positions
    private GameObject[] allHunters; // Hunters need to be destroyed on player death
    private GameObject[] allFatDogs;
	private GameObject[] allKeys;
	private GameObject[] allDoors;
	private GameObject[] allDestructibles;
	private GameObject[] allBones;
	private GameObject[] allSpheres;
	private Rigidbody rb;
	private chaseTransition chaseTransScript;
	private enemyPathfinding script;
	private huntingDog hunterScript;
	private fatDogAi fatDogScript;
	private TemporaryMovement playerScript;
	private breakableObject bo;


    void Start()
    {	
		sendBack = false;
        currentLevel = Application.loadedLevelName; // get current level name
		chaseTransScript = GameObject.Find ("BGM").GetComponent<chaseTransition>();//get chase music transition script
		rb = GetComponent<Rigidbody> ();
	}

    void OnTriggerEnter(Collider other) // turns the check point on
    {
        if (other.gameObject.name == "checkPoint_Trigger")
        {
            this.checkPointActivated = true;
        }
        else if (other.gameObject.name == "checkPoint_Trigger_2")
        {
            this.checkPointActivated_2 = true;
        }
    }

    void OnCollisionEnter(Collision other) // On collision with an enemy
    {
        if ((other.gameObject.CompareTag ("enemy") || other.gameObject.CompareTag ("huntingDog") || other.gameObject.CompareTag ("fatDog")) )
        {
			// if check point has not been reached
			if ( checkPointActivated == false && checkPointActivated_2 == false )
			{
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

	            this.transform.position = startPosition.transform.position;
	            resetLevel();
	            //Application.LoadLevel(currentLevel);
			}
			else if (checkPointActivated == true || checkPointActivated_2 == true)
			{
				if (other.gameObject.CompareTag ("enemy"))
				{
					other.gameObject.GetComponent<enemyPathfinding>().agent.velocity = Vector3.zero;
				}
				if (checkPointActivated_2)
				{
					this.transform.position = checkPointPosition_2.transform.position;
				}
				if (checkPointActivated)
				{
					this.transform.position = checkPointPosition.transform.position;
				} 
				resetLevel();
			}
        }
    }

    //public static bool isNull(System.Object aObj)
    //{
    //    return aObj == null || aObj.Equals(null);
    //}

    void resetLevel()
    {
        sendBack = true;
        playerScript = player.GetComponent<TemporaryMovement>();
        playerScript.resetKeys();

		// Get all the objects from the scene using tag (Might have performance impact, if have time find a better solution)
        allEnemies = GameObject.FindGameObjectsWithTag("enemy");
        allHunters = GameObject.FindGameObjectsWithTag("huntingDog");
        allKeys = GameObject.FindGameObjectsWithTag("key");
        allDoors = GameObject.FindGameObjectsWithTag("door");
        allDestructibles = GameObject.FindGameObjectsWithTag("destructible");
        allBones = GameObject.FindGameObjectsWithTag("bone");
        allFatDogs = GameObject.FindGameObjectsWithTag("fatDog");
        allSpheres = GameObject.FindGameObjectsWithTag("soundSphere");
		//resets BGM.
		chaseTransScript.resetChaseTrans(); 

        foreach(GameObject hunter in allHunters)
        {
            hunterScript = (huntingDog)hunter.GetComponent<huntingDog>();
            hunterScript.selfDestruct();
            //Destroy(hunter);
        }
//		for (int i = 0; i < allHunters.Length; i++) 
//		{
//			allHunters[i].GetComponent<huntingDog>().selfDestruct();
//		}
        foreach(GameObject fatDog in allFatDogs)
        {
            fatDogScript = (fatDogAi)fatDog.GetComponent<fatDogAi>();
//            fatDogScript.agent.Stop();
//            fatDogScript.agent.velocity = Vector3.zero;
//            fatDogScript.GetComponent<Rigidbody>().velocity = Vector3.zero;
//            fatDogScript.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
//            if (fatDogScript.respawnPosition != null)
//            {
//                fatDogScript.transform.position = fatDogScript.respawnPosition;
//            }
//            fatDogScript.stateManager(4);
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
            player.GetComponent<TemporaryMovement>().bonesPlaced--;
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
        foreach (GameObject sphere in allSpheres)
        {
            Destroy(sphere);
        }

		playerScript.rb.velocity = Vector3.zero;
		playerScript.rb.angularVelocity = Vector3.zero;

		sendBack = false;
    }
}