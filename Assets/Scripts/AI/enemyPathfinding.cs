﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum enumStates
{	
	patrol = 0,
	idle = 1,
	chase = 2,
	alert = 3,
	idleSuspicious = 4,
	distracted = 5,
	detectSound = 6,
	eatBone = 7,
    smell = 8
}

public class enemyPathfinding : MonoBehaviour
{
    //Detection variables//

    //smell detection
    //float maxDistance = 2.0f;
    //bool dodging = false;
    ringOfSmell ringOfSmellScript;
    GameObject bone;    
    //rotation after smelling values
    public Transform tempSmellPosition; 
    //These variables are for the enemies to use when they smell a bone
    float maxRange = 1.5f;
    Vector3 soundSourcePos;
    Transform tempWaypointPos;
    //end of rotation after smelling values
    //sound detection
    soundSphere sphereScript;
    GameObject newSphere;
    public GameObject sphere;
    GameObject brokenObject;
    //vision detection
    coneOfVision coneOfVisionScript;
    //end of Detection variables
    RaycastHit hit;
	[HideInInspector]
    public Vector3 respawnPosition;
    //Pathfinding variables
    [Tooltip("Waypoint to go during patrol")]
    public Transform target1;
    [Tooltip("Waypoint to go during patrol")]
    public Transform target2;
    [Tooltip("Waypoint to go during patrol")]
    public Transform target3;
    [Tooltip("Waypoint to go during patrol")]
    public Transform target4;
    [Tooltip("Waypoint the enemy is heading right now")]
    public Transform currentTarget;
    [Tooltip("Waypoint the enemy was heading moment ago")]
    public Transform lastTarget;
    [Tooltip("The first waypoint enemy will head, used for checkpoint")]
    public Transform firstTarget;
    float maxScale = 20;
    float waypointOffsetMin = -2.05f;
    float waypointOffsetMax = 2.05f;
    float vectorTransformPositionx = 0;   
    float vectorTransformPositionz = 0;
    float vectorCurrentTargetx = 0;
    float vectorCurrentTargetz = 0;
    float vectorx;
    float vectorz;	
	Vector3[] path = new Vector3[0];
    Vector3 currentWaypoint;
    Vector3 firstWaypoint;
    public float rotationSpeed;
    //End of Pathfinding variables
    [Tooltip("FOR DEBUG The state where enemy is right now")]
    public enumStates States;
    [Tooltip("Enemy speed for non chase state")]
    public float patrolSpeed;
    [Tooltip("Enemy speed for chase state")]
    public float chaseSpeed;
    [Tooltip("if player gets outside of this range enemy goes to alert")]
    public float chaseRange;
    //Idle Suspicious variables
    float rotationDifference = 0;
    [Tooltip("from -180° to 180°. Used on Idle suspicious")]
    public float firstDirection;    // -These are used to determine where the opponen will look when it
                                    // reaches the waypoint.
                                    // Insert integer to set the angle between -180 and 180.
    [Tooltip("from -180° to 180°. Used on Idle suspicious")]
    public float secondDirection;  
    [Tooltip("from -180° to 180°. Used on Idle suspicious")]
    public float thirdDirection;    
    List<float> directionDegrees = new List<float>();
    GameObject enemyObject;    
    float rotationStep = 65.0f;             //-Enemies turning speed
    [Tooltip("By how much can the enemy miss their intended direction in angles")]
    public float angleOffsetMax = 10.0f;    // -These values are used to prevent the Unity from missing
    [Tooltip("By how much can the enemy miss their intended direction in angles")]
    public float angleOffsetMin = -10.0f;   // the right angle during updates.
    [Tooltip("How long will the enemy wait between turning from one direction to another")]
    public float turnTimer = 100.0f;        // -This is used to determine how long the enemy will sit idling between turning from a single angle to another.
    float currentTargetDirection;           
    int turnCounter = 0;
    //bool rotating = false;
    bool rotationInProgress = false;       
    // end of Idle Suspicious variables
    //Timers
    int tempcounters = 0;
    float timer;
	public float idleTimer;
	float barkTimer;
    float escapeTimer;
    float dodgeTimer;
    [Tooltip("How long will enemy dodge another enemy")]
	public float defaultDodgeTimer;
    [Tooltip("How long the enemy will eat a bone")]
	public float defaultEatTimer;
    [Tooltip("How long the enemy will idle when waypoint is reached")]
	public float defaultIdleTimer;
    [Tooltip("How long the enemy will wait before barking")]
	public float defaultBarkTimer;
    [Tooltip("How long the enemy will stay in alert state")]
	public float defaultAlertTimer;
    [Tooltip("How long the player needs to be out of sight for enemies to return into alert state")]
	public float defaultEscapeTimer;
    [Tooltip("Time between turns during suspicious state")]
    public float defaultTurnTimer;
    [Tooltip("How often enemies will ensure that they have a target to go for")]
    public float defaultNewTargetTimer; 
    [Tooltip("How long enemy will wait before turning towards the smell")]
    public float defaultTurnTowardsSmellTimer;
    [Tooltip("How long enemy can stand still before checking if he's stuck")]
    public float defaultAgentNotMovingTimer;
    [Tooltip("How long until Alert Waypoints can be organized again by the same enemy")]
	public float organizeAlertWaypointsTimer;
    //end of Timers  
    //Charge variables
    float chargeTimer;
    [Tooltip("Time 'til the enemy 'charges'")]
    public float defaultChargeTimer;
    [Tooltip("Range from where the enemy can 'charge'")]
    public float chargeRange;
    Vector3 enemyRotation;
    //end of Charge variables
    //If enemies get stuck variables
    float tempPosX = 0;
    float tempPosZ = 0;
    bool visited = false;
    //end of If enemies get stuck variables
    //Alert waypoint organization variables
    List<Transform> usedWaypoints = new List<Transform>();
    List<Vector3> waypointLocations = new List<Vector3>();
    List<Transform> tempAlertWaypoints = new List<Transform>();
    float closestWaypointValue = 0;
    //int currentWaypointIndex = 0;
    float waypointLocationValue = 0;
    Vector3 waypointDifference;
    Vector3 _soundSourceValue;
    //end of Alert waypoint organization variables
    //Misc variables
    Collider playerCollider;
    Animator patrolAnim;
    bool randomPointSelected = false;
    public float animationSpeedModifier;
    //This is for Animator guy to see enemies actual speeds, it uses normal update atm.
    //It can be changed to FixedUpdate if it gives better results
   Vector3 currentMove;
   Vector3 previousPosition;
    float smellTimer = 3.0f;
    [Tooltip("To show enemy's current speed")]
    public float currentSpeed;
    //[HideInInspector]
    public bool isPatrolling = true;
    [HideInInspector]
     public GameObject player;
   // [HideInInspector]
     public GameObject soundSource;
   // [HideInInspector]
     public float alertTimer;
   // [HideInInspector]
    public float eatTimer;
   // [HideInInspector]
	public float defaultTimer;
   // [HideInInspector]
    public int areaCounter = 0;
   // [HideInInspector]
    public float turnTowardsSmellTimer;
  //  [HideInInspector]
    public int targetCounter = 0;
   // [HideInInspector]
    public float newTargetTimer;
   // [HideInInspector]
    public float agentNotMovingTimer;
   // [HideInInspector]
    public float currentAngle = 0;
   // [HideInInspector]
    public float targetAngle = 0;
   // [HideInInspector]
    public bool isPaired = false;
  //  [HideInInspector]
    public bool eatBone = false;
   // [HideInInspector]
    public bool distracted = false;
   // [HideInInspector]
    public bool onWaypoint = false;
  //  [HideInInspector]
    public bool continueRotation = false;
    [HideInInspector]
    public GameObject visionRotator;
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public List<Transform> targets = new List<Transform>();
    [HideInInspector]
    public List<Transform> alertArea = new List<Transform>();
   // [HideInInspector]
    public bool isOnWaypoint = false;
   // [HideInInspector]
    public bool rotationCompleted = false;
  //  [HideInInspector]
    public bool SeekForSmellSource = false;
   // [HideInInspector]
    public bool agentStopped = false;
   // [HideInInspector]
    public Vector3 _soundSource;
    //public float deceleration = 60f;
    //public float acceleration = 0.0f;
    NavMeshPath navPath;
	Rigidbody rb;
    //end of Misc variables

    void Start()
    {
        respawnPosition = this.transform.position;
        dodgeTimer = defaultDodgeTimer;
        visionRotator = GameObject.FindGameObjectWithTag("VisionRotator");        
        player = GameObject.FindGameObjectWithTag("Player");
        ringOfSmellScript = player.GetComponentInChildren<ringOfSmell>();
        coneOfVisionScript = GetComponentInChildren<coneOfVision>();
        patrolAnim = gameObject.GetComponent<Animator>();
		
        //Setting waypoints and Navigation Mesh agent
        setDirectionsForIdle();
        setTargetWaypoints();
        currentTarget = targets[0];
        lastTarget = currentTarget;
        firstTarget = currentTarget;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.SetDestination(currentTarget.position);
        //end of Setting waypoints and Navigation Mesh agent

        //Setting Timers
        chargeTimer = defaultChargeTimer;
        timer = defaultTimer;
        eatTimer = defaultEatTimer;
        barkTimer = defaultBarkTimer;
        alertTimer = defaultAlertTimer;
        escapeTimer = defaultEscapeTimer;
        turnTimer = defaultTurnTimer;
        turnTowardsSmellTimer = defaultTurnTowardsSmellTimer;
        agentNotMovingTimer = defaultAgentNotMovingTimer;
        newTargetTimer = defaultNewTargetTimer;
        //end of Setting Timers

        //Misc 
        playerCollider = player.GetComponent<Collider>();
		rb = GetComponent<Rigidbody> ();
        //end of Misc
    }
    void Update()
    {
        //Velocity meter for the animator guy to see enemies actual speed
        currentMove = transform.position - previousPosition;
        currentSpeed = currentMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
        patrolAnim.speed = agent.speed * animationSpeedModifier;
        updateAnimator();

        //Tried to make enemies be able to dodge each other when pahtfinding
        //Physics.Raycast(transform.position,transform.forward * 0.05f,out hit);
        //if (hit.collider.tag == "enemy" || hit.collider.tag == "fatDog" || hit.collider.tag == "huntingDog")
        //{
        //    dodge(hit.collider);
        //}
        //if (dodgeTimer <= 0 && dodging == true)
        //{
        //    dodging = false;
        //    currentTarget = lastTarget;
        //}

        // To prevent opponent from sleeping
		rb.WakeUp();

        // FSM
        switch (States)
        {
			// Patrol, moves from one waypoint to the next waiting for a second before advancing forward
            case enumStates.patrol:
            {
                if(!isPatrolling)
                {
                    isPatrolling = true;
                }
                if (newTargetTimer >= 0)
                {                       
                    agent.velocity = Vector3.zero;
                    newTargetTimer -= Time.deltaTime; 
                }
                else
                {
                    patrolAnim.SetBool("patrolRun", false);
                    if (agentStopped == true && isPaired)
                    {
                        agentStopped = false;
                        agent.Resume();
                    }

                    //Check if the player is within offset range from the current target
                    if (vectorx >= waypointOffsetMin && vectorx <= waypointOffsetMax && vectorz >= waypointOffsetMin && vectorz <= waypointOffsetMax)
                    {                            
                        stateManager((int)enumStates.idle);
                        onWaypoint = true;
                        if (agentStopped == false)
                        {
                            agentStopped = true;
                            agent.velocity = Vector3.zero;
							rb.angularVelocity = Vector3.zero;
							rb.velocity = Vector3.zero;
                            agent.Stop();
                            //isOnWaypoint = true;
                            // patrolAnim.SetBool("patrolWalk", false);
                            patrolAnim.SetBool("patrolRun", false);
                        }
                    }
			
                    if (SeekForSmellSource)
                    {
                        tempSmellPosition = player.transform;
                        stateManager((int)enumStates.smell);
                    }
                }
            }
            break;

			// Idle, look around, without moving towards waypoints
            case enumStates.idle:
            {
                patrolAnim.SetBool("patrolRun", false);
                if (agentStopped == false)
                {
                    agentStopped = true;
                    agent.velocity = Vector3.zero;
					rb.velocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
                    agent.Stop();
                }
                //Check if the player is within offset range from the current target
                if (vectorx >= waypointOffsetMin && vectorx <= waypointOffsetMax && vectorz >= waypointOffsetMin && vectorz <= waypointOffsetMax)
                {                       
                    if (idleTimer <= 0)
                    {
                        if (currentTarget.gameObject.CompareTag ("Bone") == false)
                        {
                            lastTarget = currentTarget;
                        }
                        if (isPaired == true)
                        {
                            idleTimer = defaultIdleTimer;
                            isOnWaypoint = true;
                        }
                        else if (isPaired == false)
                        {
                            idleTimer = defaultIdleTimer;
                            currentTarget = targets[targetCounter];
                            stateManager(0);
                            targetCounter++;
                            if (targetCounter >= targets.Count)
                            {
                                targetCounter = 0;
                            }
                        }
                        agent.speed = patrolSpeed;
                    }
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0.0f)
                    {
                        idleTimer = 0.0f;
                    }
                }
                //We assume enemy is not in a waypoint
                else
                {
                    patrolAnim.SetBool("patrolRun", false);
                    stateManager((int)enumStates.patrol);
                }

                if (SeekForSmellSource)
                {
                    tempSmellPosition = player.transform;
                    stateManager((int)enumStates.smell);
                }
            }
			break;

			// chase the Player constantly searching for a waypoint at the Player position//
            case enumStates.chase:
            {
                if (isPatrolling)
                {
                    isPatrolling = false;
                }
                if(onWaypoint == true)
                {
                    onWaypoint = false;
                }
                patrolAnim.SetBool("patrolRun", true);

                navPath = new NavMeshPath();
                agent.CalculatePath(currentTarget.position, navPath);
                if (navPath.status == NavMeshPathStatus.PathPartial)
                {
                    agent.autoBraking = false;
                    enemyRotation = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    transform.LookAt(enemyRotation);
                    transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, chaseSpeed * Time.deltaTime);

//                    chargeTimer -= Time.deltaTime;
//                    if (chargeTimer <= 0.0f)
//                    {
//                        agent.autoBraking = true;
//                        chargeTimer = defaultChargeTimer;
//                    }
                }

                if (vectorx < chargeRange || vectorz < chargeRange)
                {
                    agent.autoBraking = false;
                    enemyRotation = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                    transform.LookAt(enemyRotation);
                    transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, chaseSpeed * Time.deltaTime);
				}
                    chargeTimer -= Time.deltaTime;
                    if (chargeTimer <= 0.0f)
                    {
                        agent.autoBraking = true;
                        chargeTimer = defaultChargeTimer;
                    }
//                }
//                else
//                {
//                    chargeTimer -= Time.deltaTime;
//                    if (chargeTimer <= 0.0f)
//                    {
//                        agent.autoBraking = true;
//                        chargeTimer = defaultChargeTimer;
//                    }
//                }

                // Bark while chasing
                if (barkTimer < 0)
                {
                    newSphere = (GameObject)Instantiate(sphere, this.transform.position, Quaternion.identity);
                    newSphere.transform.parent = transform;
                    newSphere.tag = "Sound";
                    barkTimer = defaultBarkTimer;
                    if (newSphere)
                    {
                        sphereScript = newSphere.GetComponent<soundSphere>();
                        sphereScript.setMaxDiameter(maxScale);
                    }
                }
                barkTimer -= Time.deltaTime;

                // Escape from chase
                Physics.Linecast(this.transform.position, player.transform.position, out hit);
                Debug.DrawLine(this.transform.position, player.transform.position, Color.red);
                if (hit.collider.CompareTag (playerCollider.tag) == false)
                {
                    if (vectorx >= chaseRange || vectorz >= chaseRange)
                    {
                        agent.speed = patrolSpeed;
                        if (alertArea[areaCounter] != null)
                        {
                            currentTarget = alertArea[areaCounter];
                        }
                        areaCounter++;
						if (areaCounter > alertArea.Count - 1)
                        {
                            areaCounter = 0;
                        }
                        alertTimer = defaultAlertTimer;
                       // organizeAlertWaypoints();
                        stateManager((int)enumStates.alert);
                    }
                    else if (escapeTimer <= 0)
                    {
                        agent.speed = patrolSpeed;
                        if (alertArea[areaCounter] != null)
                        {
                            currentTarget = alertArea[areaCounter];
                        }

                        areaCounter++;
						if (areaCounter > alertArea.Count - 1)
                        {
                            areaCounter = 0;
                        }
                        alertTimer = defaultAlertTimer;
                        //organizeAlertWaypoints();
					stateManager((int)enumStates.alert);
                    }
                    escapeTimer -= Time.deltaTime;
                }
                else
                {
                    agent.speed = chaseSpeed;
                    if (currentTarget != player.transform && currentTarget.CompareTag ("Bone") == false)
                    {
                        lastTarget = currentTarget;
                    }
                    currentTarget = player.transform;
                }
            }
            break;
			//Look around a room by moving from waypoint to waypoint//
            case enumStates.alert:
			{
	            if (isPatrolling)
	            {
	                isPatrolling = false;
	            }
	            patrolAnim.SetBool("patrolRun", false);
	            if (distracted)
	            {
					stateManager((int)enumStates.distracted);
	            }
	            if (onWaypoint == true)
	            {
	                onWaypoint = false;
	            }
	            if (agentStopped == true)
	            {
	                agent.velocity = Vector3.zero;
	                agentStopped = false;
	                agent.Resume();
	            }
	            if (alertTimer == 0 || alertTimer < 0)
	            {
	                if (lastTarget != null)
	                {
	                    currentTarget = lastTarget;
						stateManager((int)enumStates.idleSuspicious);
	                }
	            }
	            //Check if the player is within offset range from the current target
	            if (vectorx >= waypointOffsetMin && vectorx <= waypointOffsetMax && vectorz >= waypointOffsetMin && vectorz <= waypointOffsetMax)
	            {
	                if (timer <= 0 && (!distracted))
	                {
	                    if (currentTarget != null && currentTarget.gameObject.CompareTag ("Bone") == false)
	                    {
	                        lastTarget = currentTarget;
	                    }
	                    //if(alertArea[areaCounter] == null)
	                    //{
	                    //    //areaCounter = 0;
	                    //}
	                    if (alertArea[areaCounter] != null)
	                    {
	                        currentTarget = alertArea[areaCounter];
	                    }

	                    areaCounter++;
						if (areaCounter > alertArea.Count - 1)
	                    {
	                        areaCounter = 0;
	                    }
	                    if (tempcounters < 6)
	                    {
	                        if (turnCounter != 0)
	                        {
	                            turnCounter = 0;
	                        }
	                        if (idleTimer != defaultIdleTimer)
	                        {
	                            idleTimer = defaultIdleTimer;
	                        }
	                        tempcounters++;
							stateManager((int)enumStates.idleSuspicious);
	                    }
	                }
	                else
	                {
	                    alertTimer -= Time.deltaTime;
	                    if (alertTimer <= 0.0f)
	                    {
	                        alertTimer = 0.0f;
	                    }
	                }

	            }
	            else
	            {
	                alertTimer -= Time.deltaTime;
	                if (alertTimer <= 0.0f)
	                {
	                    alertTimer = 0.0f;
	                }
	            }

	            if (SeekForSmellSource)
	            {
	                tempSmellPosition = player.transform; 
					stateManager((int)enumStates.smell);
	            }
			}
            break;

            case enumStates.idleSuspicious:
            {
                //-----------------------------------------------//
                //Stand on the spot and look at preset directions//
                //-----------------------------------------------//
                if (isPatrolling)
                {
                    isPatrolling = false;
                }
                patrolAnim.SetBool("patrolRun", false);
                if (agentStopped == false)
                {
                    agent.velocity = Vector3.zero;
                    agentStopped = true;
					rb.angularVelocity = Vector3.zero;
					rb.velocity = Vector3.zero;
                    agent.Stop();
                }
                if (coneOfVisionScript.playerSeen == true)
                {
                    agentStopped = false;
                    agent.Resume();
					stateManager((int)enumStates.chase);
                }
                if (alertTimer > 0.0f)
                {
                    alertTimer -= Time.deltaTime;
                }

                if (alertTimer <= 0.0f)
                {
                    alertTimer = 0.0f;
//                }
//                if (alertTimer <= 0)
//                {
                    agent.speed = patrolSpeed;
                    turnCounter = 0;
                    agentStopped = false;
                    agent.Resume();
					stateManager((int)enumStates.patrol);
                }
                if (turnCounter < 3)
                {
                    currentTargetDirection = directionDegrees[0];
                    rotateEnemy(currentTargetDirection, rotationStep);

                    if (rotationCompleted)
                    {
                        directionDegrees.Add(directionDegrees[0]);
                        directionDegrees.Remove(directionDegrees[0]);
                        rotationCompleted = false;
                        turnCounter++;
                        turnTimer += defaultTurnTimer * Time.deltaTime;
                    }

                }

                if (turnCounter > 2)
                {
                    alertTimer = defaultAlertTimer;
                    turnCounter = 0;
                    agentStopped = false;
                    agent.Resume();
					stateManager((int)enumStates.alert);
                }

                idleTimer -= Time.deltaTime;
                if (idleTimer < 0.0f)
                {
                    idleTimer = 0.0f;
                }
                
            }
			break;

			// Move towards distraction
            case enumStates.distracted:
            {
                if (isPatrolling)
                {
                    isPatrolling = false;
                }
                patrolAnim.SetBool("patrolRun", false);
                distracted = true;
                Vector3 bonedir = (currentTarget.transform.localPosition) - (this.transform.localPosition);
                if (bonedir.x <= 4 && bonedir.x >= -4 && bonedir.z <= 4 && bonedir.z >= -4)
                {
                    stateManager(7);
                    distracted = false;
                    if (!eatBone)
                    {
                        eatTimer = defaultEatTimer;
                    }

                    eatBone = true;
                }
            }
            break;

            case enumStates.detectSound:
            {
				if (soundSource != null) 
				{
                    if (soundSource.CompareTag ("Bone"))
                    {                        
                        if (isPatrolling)
                        {
                            isPatrolling = false;
                        }
                        navPath = new NavMeshPath();
                        NavMesh.CalculatePath(transform.position, soundSource.transform.position, NavMesh.AllAreas, navPath);
                        if (navPath != null)
                        {
                            organizeAlertWaypoints();

                            patrolAnim.SetBool("patrolRun", false);

                            if (soundSource && soundSource.CompareTag ("Bone") == false && randomPointSelected == false)
                            {
                                if (RandomPoint(soundSource.transform.position, maxRange, out soundSourcePos))
                                {
                                    randomPointSelected = true;
                                    Debug.DrawRay(soundSourcePos, Vector3.up, Color.blue, 5.0f);
                                    tempWaypointPos = soundSource.transform;
                                    currentTarget = tempWaypointPos;//soundSourcePos;
                                }
                            }
                            else if (soundSource && soundSource.CompareTag ("Bone"))
                            {
                                currentTarget = soundSource.transform;
                            }
                            // agent.SetDestination(currentTarget.transform.position);

                            // When sound is heard, move towards the source// 

                            // Check if the enemy is within offset range from the current target
                            if (vectorx >= (waypointOffsetMin) && vectorx <= (waypointOffsetMax) && vectorz >= (waypointOffsetMin) && vectorz <= (waypointOffsetMax))
                            {
                                if (soundSource != null || !soundSource.Equals(null))
                                {
                                    //Physics.Linecast(this.transform.position, soundSource.transform.position, out hit);
                                    //Debug.DrawLine(this.transform.position, soundSource.transform.position, Color.blue);
                                }
                                alertTimer = defaultAlertTimer;

                                if (soundSource)//(hit.collider != null)
                                {
                                    if (agentStopped == false)
                                    {
                                        agent.velocity = Vector3.zero;
                                        agentStopped = true;
                                        agent.Stop();
										rb.angularVelocity = Vector3.zero;
										rb.velocity = Vector3.zero;

                                    }
                                    if (soundSource.CompareTag ("Bone"))//(hit.collider.tag == "bone")
                                    {
                                        randomPointSelected = false;
										stateManager((int)enumStates.eatBone);
                                    }
                                    else
                                    {
                                        randomPointSelected = false;
										stateManager((int)enumStates.alert);
                                    }
                                }
                                else
                                {
                                    randomPointSelected = false;
									stateManager((int)enumStates.alert);
                                }
                            }
                            else if (soundSource == null)
                            {
								stateManager((int)enumStates.alert);
                            }
                        }
                        else
                        {
							stateManager((int)enumStates.alert);
                        }

                    }
                    else if (soundSource.CompareTag ("Bone"))
                    {
                       // if (smellDetectedBefore)
                       // {
                            // continue towards the soundsource
                            // but change target if the enemy 
                     //   }
                       // else
                      //  { 
                           // pick this as a new sound
                     //   }
                    }
				}
				else
				{
					stateManager ((int)enumStates.idle);
				}
            }
            break;

			// holds the enemy still for long enough for the distraction to pass
            case enumStates.eatBone:
            {
                if (isPatrolling)
                {
                    isPatrolling = false;
                }
                patrolAnim.SetBool("patrolRun", false);
                if (soundSource != null && soundSource.CompareTag ("Bone"))
                {
                    bone = soundSource;
                }                        

            //if (hit.collider.tag == "bone")
            //{
                eatBone = true;
                if (!bone)
                {
                    eatBone = true;
                    alertTimer += defaultAlertTimer;
                    stateManager((int)enumStates.alert);
                    eatTimer = defaultEatTimer;
                    currentTarget = alertArea[areaCounter];
                }

                if (eatTimer <= 0)
                {
                    eatTimer = defaultEatTimer;
                    distracted = false;
                    eatBone = false;

                    currentTarget = alertArea[areaCounter];
                    breakableObject boneScript = bone.GetComponent<breakableObject>();
                    boneScript.destroySelf();
                    //Destroy(bone);
                    alertTimer += defaultAlertTimer;
					stateManager((int)enumStates.alert);
                }
                eatTimer -= Time.deltaTime;
                if (eatTimer <= 0.0f)
                {
                    eatTimer = 0.0f;
                }
            //}
            }
            
            break;

			// Smell if player smell collides with it
            case enumStates.smell:
            {
                if (smellTimer > 0.0f)
                {
                    smellTimer -= Time.deltaTime;
                    if (smellTimer <= 0.0f)
                    {
                        smellTimer = 180;
						stateManager((int)enumStates.idle);
                    }

                    patrolAnim.SetBool("patrolRun", false);

                    // Turns enemy towards player's last known location//
                    SeekForSmellSource = true;
                    agentStopped = true;
                    agent.velocity = Vector3.zero;
                    agent.Stop();
					rb.angularVelocity = Vector3.zero;
					rb.velocity = Vector3.zero;
                    if (tempSmellPosition != null)
                    {            
						// Rotate the enemy
                        Vector3 relative = transform.InverseTransformPoint(tempSmellPosition.position);
                        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                        transform.Rotate(0, angle * Time.deltaTime * rotationSpeed, 0);
						
						// If reach the angle
                        if (angle < 5.0f && angle > -5.0f)
                        {
	                        SeekForSmellSource = false;
							stateManager((int)enumStates.idle);
						}
						else
						{
							
						}
                    }
                    //else
                    //{
                    //    smellTimer = 180;
                    //    agentStopped = false;
                    //    agent.Resume();
                    //    stateManager(1);
                    //}
               	}
				else
				{
				    smellTimer = 180;
					stateManager((int)enumStates.idle);
				}
			}
            break;

            default:
                break;
        }

        // Calculating the distance between game object's target and this game object
        if (currentTarget != null)
        {
            // Vector calculations used multiple times during the update
            vectorTransformPositionx = transform.position.x;
            vectorTransformPositionz = transform.position.z;

            vectorCurrentTargetx = currentTarget.position.x;
            vectorCurrentTargetz = currentTarget.position.z;

            vectorx = (vectorTransformPositionx - vectorCurrentTargetx);
            vectorz = (vectorTransformPositionz - vectorCurrentTargetz);
            if (vectorz < 0)
            {
                vectorz *= -1;
            }
            if (vectorx < 0)
            {
                vectorx *= -1;
            }
        }

        // Checking if player is near enough to be smelled
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Physics.Raycast(transform.position, direction, out hit, (ringOfSmellScript.radius));
        Debug.DrawRay(transform.position, direction * ringOfSmellScript.radius, Color.yellow);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag (player.GetComponent<Collider>().tag))
            {
                checkContinuousSmelling(player.transform.position);
            }
            else
            {
                turnTowardsSmellTimer = defaultTurnTowardsSmellTimer;
            }
        }
        else
        {
            turnTowardsSmellTimer = defaultTurnTowardsSmellTimer;
        }
    }
    
    void LateUpdate()
    {
        //Just to make sure enemies will have a target
        if (timer <= 0)
        {
            timer += defaultTimer;

            if (States != enumStates.idleSuspicious)
            {
                if (currentTarget != null && !currentTarget.Equals(null))
                {
                    agent.SetDestination(currentTarget.position);
                    if (newTargetTimer <= 0 && onWaypoint == true)
                    {
                        newTargetTimer = defaultNewTargetTimer;
                        onWaypoint = false;
                    }
                }
                else
                {
                    currentTarget = lastTarget;
                }
            }            
        }
        timer -= Time.deltaTime;
        //if(dodgeTimer > 0)
        //{
        //    dodgeTimer--;
        //}

		// To decide what player should do when he's not moving in the end of an update.
        if (agentStopped == true)
        {
            if (States == enumStates.patrol)
            {                
                agentStopped = false;
                agent.Resume(); 
            }

            else if (States == enumStates.chase || States == enumStates.alert)
            {                
                agentNotMovingTimer -= Time.deltaTime;
                if (agentNotMovingTimer <= 0.0f)
                {
                    agentStopped = false;
                    agent.Resume();
                    agentNotMovingTimer = defaultAgentNotMovingTimer;
                }
                else if (agentStopped == false)
                {
                    agentNotMovingTimer = defaultAgentNotMovingTimer;
                }
            }
            else if (States == enumStates.idle)
            {                
                agentStopped = true;
                agent.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.velocity = Vector3.zero;
                agent.Stop();
            }           
        }

        // To make sure even if the enemies lose their target for some reason
        // they will recover and start to move again. 
        agentNotMovingTimer -= Time.deltaTime;
        if (agentNotMovingTimer <= 0.0f)
        {
//            agentNotMovingTimer = 0;
//        }
//        if (agentNotMovingTimer == 0)
//        {                       
            if (visited == false)
            {               
                tempPosX = vectorx;
                tempPosZ = vectorz;
                visited = true;
                agentNotMovingTimer = defaultAgentNotMovingTimer;
            }
            else
            {
                //Check if the player is within offset range from the current target
                if (tempPosX == vectorx || tempPosX >= (vectorx - waypointOffsetMin) || tempPosX <= (vectorx - waypointOffsetMax) && tempPosZ == vectorz || tempPosX >= (vectorz - waypointOffsetMin) || tempPosZ <= (vectorz - waypointOffsetMax))
                {                    
                    agent.SetDestination(currentTarget.position);

                    agentNotMovingTimer = defaultAgentNotMovingTimer;
                }                
                visited = false;
            }

//            agentStopped = true;
//            agent.Stop();
//            agentStopped = false;
//            agent.Resume();
        }
    }

	//======================================================================//
	//	Call this function to change state									//
	//======================================================================//
    public void stateManager(int value)
    {
        States = (enumStates)value;
    }

	//======================================================================//
	//	Sets directions for idle (Only called in Start() for init)			//
	//======================================================================//
    void setDirectionsForIdle()
    {
        directionDegrees.Add(firstDirection);
        directionDegrees.Add(secondDirection);
        directionDegrees.Add(thirdDirection);
    }

	//======================================================================//
	//	Sets waypoints (Only called in Start() for init)					//
	//======================================================================//
    void setTargetWaypoints()
    {
        // Last edited by Marc
        if (target1 != null)
        {
            targets.Add(target1);
        }
        if (target2 != null)
        {
            targets.Add(target2);
        }
        if (target3 != null)
        {
            targets.Add(target3);
        }
        if (target4 != null)
        {
            targets.Add(target4);
        }
        //end of Last edited by Marc
    }

	//======================================================================//
	//	Rotation of enemy													//
	//======================================================================//
    void rotateEnemy(float targetDegrees, float rotationStep)
    {
        rotationDifference = 0;

        if (turnTimer <= 0)
        {
            if (rotationInProgress == false)
            {
                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                targetAngle = targetDegrees;
                rotationInProgress = true;
            }
            else if (rotationInProgress)
            {
                if (turnTimer == 0 && rotationDifference >= 0)
                {
                    if (targetAngle <= 180 && targetAngle >= 0) //decide which side the target is. 0-180 left, 0 - (-180)
                    {
                        //=============//
                        // First Sector//
                        //=============//
                        if (targetAngle <= 90 && targetAngle >= 0)// decide which sector the target is. 4 different sectors 0-90, 90-180, 0-(-90), (-90)- (-180)
                        {

                            if (currentAngle <= targetAngle && currentAngle > targetAngle - 180)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1); // Vector3.up * Time.deltaTime * rotationStep * -1
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;

                                if (rotationDifference < 0)
                                {
                                    rotationDifference = rotationDifference * -1;
                                }

                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                            else //if (currentAngle > targetAngle && turnTimer == 0)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1); //Vector3.up * Time.deltaTime * rotationStep * 1
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                }
                            }
                        }

                        //=============//
                        //Second Sector//
                        //=============//
                        else if (targetAngle > 90 && targetAngle <= 180)// decide which sector the target is
                        {
                            if (currentAngle > targetAngle || currentAngle <= targetAngle - 180)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (rotationDifference < 0)
                                {
                                    rotationDifference = rotationDifference * -1;
                                }


                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                            else
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                        }
                    }

                    else if (targetAngle < 0 && targetAngle > -180)  //decide which side the target is
                    {
                        //=============//
                        //Third Sector //
                        //=============//
                        if (targetAngle >= -90)// decide which sector the target is. 4 different sectors 0-90, 90-180, 0-(-90), (-90)- (-180)
                        {
                            if (currentAngle >= targetAngle && currentAngle <= 180 + targetAngle)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (rotationDifference < 0)
                                {
                                    rotationDifference = rotationDifference * -1;
                                }
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                            else //if (currentAngle < targetAngle && turnTimer == 0)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                        }
                        //=============//
                        //Fourth Sector//
                        //=============//
                        else if (targetAngle < -90)// decide which sector the target is. 4 different sectors 0-90, 90-180, 0-(-90), (-90)- (-180)
                        {
                            if (currentAngle >= targetAngle && currentAngle <= 180 + targetAngle)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (rotationDifference < 0)
                                {
                                    rotationDifference = rotationDifference * -1;
                                }
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                            else //if (currentAngle < targetAngle && turnTimer == 0)
                            {
                                transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                rotationDifference = targetAngle - currentAngle;
                                if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                {
                                    rotationCompleted = true;
                                    rotationInProgress = false;
                                    turnTimer += defaultTurnTimer;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0.0f)
            {
                turnTimer = 0.0f;
            }
        }
    }

	//======================================================================//
	//	Sets an area from a room the enemy is in for the alert-state		//
	//======================================================================//
    public void organizeAlertWaypoints()
    {
		// float lowestValue;
		List<Transform> toBeWaypointOrder = new List<Transform>();
		Vector3 thisWaypoint = new Vector3(0,0,0);
		List<float> usedValues = new List<float>();
		List<Vector3> usedVector = new List<Vector3>();
		List<Vector3> orderForWaypoints = new List<Vector3>();
		List<Transform> tempUsedWaypoints = new List<Transform>();
		tempUsedWaypoints.Clear();
		int counter = 0;

        if (organizeAlertWaypointsTimer <= 0)
        {
            for (int zi = 0; zi < alertArea.Count; zi++)
            {
                // goes through every alertArea waypoint
                tempAlertWaypoints.Clear();
                waypointLocations.Clear();
                
                for (int i = 0; i < alertArea.Count; i++)
                {
                    if (alertArea[i] != null /*|| tempAlertWaypoints[i] != null*/ && !tempUsedWaypoints.Contains(alertArea[i]))
                    {
                        tempAlertWaypoints.Add(alertArea[i]);
                        if(!usedWaypoints.Contains(alertArea[i]))
                        {
							waypointLocations.Add(tempAlertWaypoints[i].transform.position);
                        }
                    }
                }

                // calculate the length of the vector we're going to set as the closest one
                Vector3 closestWaypoint = new Vector3(waypointLocations[0].x, waypointLocations[0].y, waypointLocations[0].z);
                Vector3 _soundSourceValue = new Vector3( 0, 0, 0 );

                if(zi == 0)
                {
                    _soundSourceValue = new Vector3(_soundSource.x, _soundSource.y, _soundSource.z);
                }
                   
                else
                {
                    _soundSourceValue = new Vector3(toBeWaypointOrder[zi - 1].transform.position.x, toBeWaypointOrder[zi - 1].transform.position.y, toBeWaypointOrder[zi - 1].transform.position.z);
                }

                Vector3 tempVec = new Vector3(closestWaypoint.x - _soundSourceValue.x, closestWaypoint.y - _soundSourceValue.y, closestWaypoint.z - _soundSourceValue.z);
                tempVec = new Vector3(tempVec.x * tempVec.x, tempVec.y * tempVec.y, tempVec.z * tempVec.z);

                closestWaypointValue = tempVec.x + tempVec.y + tempVec.z;
                closestWaypointValue = Mathf.Sqrt(closestWaypointValue);

                // Run the check if it's really the closest waypoint we're looking for
                for (int i = 0; i < waypointLocations.Count; i++)
                {                    
                    if(!usedVector.Contains(waypointLocations[i]))
                    {                            
                        // run the same kind of calculations as above
                        Vector3 possiblyClosest = new Vector3(waypointLocations[i].x, waypointLocations[i].y, waypointLocations[i].z);
                        possiblyClosest = new Vector3(waypointLocations[i].x - _soundSourceValue.x, waypointLocations[i].y - _soundSourceValue.y, waypointLocations[i].z - _soundSourceValue.z);
                        possiblyClosest = new Vector3(possiblyClosest.x * possiblyClosest.x, possiblyClosest.y * possiblyClosest.y, possiblyClosest.z * possiblyClosest.z);

                        waypointLocationValue = possiblyClosest.x + possiblyClosest.y + possiblyClosest.z;
                        waypointLocationValue = Mathf.Sqrt(waypointLocationValue);                         

                        if (waypointLocationValue <= closestWaypointValue)
                        {                               
                            counter = i;
                            closestWaypointValue = waypointLocationValue;
                            thisWaypoint = waypointLocations[i];                                                  
                        }                          
                    }
                }
             
                if (alertArea[counter].transform.position == thisWaypoint)
                {                       
                    toBeWaypointOrder.Add(alertArea[counter]);
                    usedWaypoints.Add(alertArea[counter]);
                }
                else
                {
                    for (int i = 0; i < alertArea.Count; i++)
                    {
                        if (alertArea[i].transform.position == thisWaypoint)
                        {
                            toBeWaypointOrder.Add(alertArea[i]);
                            usedWaypoints.Add(alertArea[i]);
                        }
                    }
                }
            }

            alertArea.Clear();
            for (int i = 0; i < toBeWaypointOrder.Count; i++)
            {
                alertArea.Add(toBeWaypointOrder[i]);
            }
            organizeAlertWaypointsTimer = 900;
        }
        else
        {
            organizeAlertWaypointsTimer -= Time.deltaTime;
            if (organizeAlertWaypointsTimer <= 0.0f)
            {
                organizeAlertWaypointsTimer = 0.0f;
            }
        }
    }
	
	//======================================================================//
	//	If enemy enters new room with diff alert area, set new waypoints	//
	//======================================================================//
    public void setAlertArea(GameObject area)
    {
        Component[] transforms;
        alertArea.Clear();
        transforms = area.GetComponentsInChildren<Transform>();

        foreach (Transform alert in transforms)
        {
            if (alert.CompareTag ("Waypoint"))
            {
                alertArea.Add(alert);
            }
        }
    }
	
	//======================================================================//
	//	Rotate enemy when player stay too close to the enemy			  	//
	//======================================================================//
    public void rotateDogWhileSmelling(Vector3 targetTransformPosition)
    {
        if (States != enumStates.smell)
        {
            if (ringOfSmellScript.smellingPlayer)
            {
                SeekForSmellSource = true;
                agentStopped = true;
                agent.velocity = Vector3.zero;
                agent.Stop();
				rb.angularVelocity = Vector3.zero;
				rb.velocity = Vector3.zero;
                Vector3 relative = transform.InverseTransformPoint(targetTransformPosition);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                if (angle >= 0)
                {
                    angle++;
                }
                else if (angle < 0)
                {
                    angle--;
                }

                transform.Rotate(0, angle * Time.deltaTime * 1.5f, 0);
                if (angle < 5.0f && angle > -5.0f)
                {
                    SeekForSmellSource = false;
                }
            }
        }
    }
	
	//======================================================================//
	//	If player stays within smelling range enemies will turn to look for //
	//	the smell source													//
	//======================================================================//
	public void checkContinuousSmelling(Vector3 targetTransformPosition)
	{
	    if (States != enumStates.smell)
	    {
	        turnTowardsSmellTimer -= Time.deltaTime;
	        if (turnTowardsSmellTimer <= 0.0f)
	        {
	            turnTowardsSmellTimer = 0.0f;
	            SeekForSmellSource = true;
	            rotateDogWhileSmelling(targetTransformPosition);
	        }   
	    } 
	}

	//======================================================================//
	//	Prevent enemies from being unable to find a path to destructible   	//
	//	object by taking a random point around the object for enemy to		//
	//	navigate to.														//
	//======================================================================//
	bool RandomPoint(Vector3 center, float range, out Vector3 result)
	{
		Vector3 randomPoint = center + Random.insideUnitSphere * range;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
		{
			result = hit.position;
			return true;
		}
		result = Vector3.zero;
		return false;
	}

	//======================================================================//
	//	Updates the animation speed	(DON'T CALL IN UPDATE ONLY CALL WHEN   	//
	//	CURRENT SPEED CHANGES!!)											//
	//======================================================================//
	void updateAnimator()
	{
		patrolAnim.SetFloat ("patrolMovement", currentSpeed);
	}
}