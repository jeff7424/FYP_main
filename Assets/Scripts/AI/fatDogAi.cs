using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum enumStatesFatDog
{
	
	patrol = 0,
    idle = 1,  // not needed
    chase = 2, // cone of vision & ring of smell
    alert = 3, // cone of vision & ring of smell
    idleSuspicious = 4, // basic state
	distracted = 5,
	detectSound = 6,
	eatBone = 7
}

public class fatDogAi : MonoBehaviour {
	
    ringOfSmell ringOfSmellScript;
    coneOfVision coneOfVisionScript;
	soundSphere sphereScript;
	RaycastHit hit;
	public Vector3 respawnPosition;
	public Transform target1;
	public Transform currentTarget;
	public Transform lastTarget;
	public Vector3 lastSeenPosition = new Vector3(0,0,0);
	public enumStatesFatDog States;
	GameObject vision;
	GameObject smell;
	GameObject bone;
	GameObject player;
	GameObject newSphere;
	public GameObject sphere;
	public GameObject soundSource;
	GameObject brokenObject;
	public NavMeshAgent agent;
	List<Transform> targets = new List<Transform>();
	public bool eatBone = false;
	public bool distracted = false;
    public bool onWaypoint = false;
    public bool isOnWaypoint;
	public float turnSpeed = 2.0f;
	public float speed = 2;
	//float maxSpeed = 5;
	float maxScale = 20;
	float waypointOffsetMin = -2.05f;
	float waypointOffsetMax = 2.05f;
	float vectorTransformPositionx = 0;
	float vectorTransformPositionz = 0;
	float vectorCurrentTargetx = 0;
	float vectorCurrentTargetz = 0;
	float vectorx;
	float vectorz;
	//Idle Suspicious values
	public float firstDirection;
	public float secondDirection;
	public float thirdDirection;
    public float fourthDirection;
	List<float> directionDegrees = new List<float>();
	GameObject enemyObject;
	//bool rotating = false;
	float rotationStep = 65.0f;
	public float currentAngle = 0;
	public float targetAngle = 0;
	public float angleOffsetMax = 10.0f;
	public float angleOffsetMin = -10.0f;
	bool rotationInProgress = false;
	public bool rotationCompleted = false;
    public float turnTimer;
	int turnCounter = 0;
    public  float rotationDifference = 0;
    // Alert Values for FatDog
    public float firstDirectionAlert;
    public float secondDirectionAlert;
    List<float> directionDegreesAlert = new List<float>();
    public float turnTimerAlert;
    public float defaultTurnTimerAlert;
    bool alertLookingDirectionsSet = false;
    //Distracted values
    bool agentStopped = false;
	public float timer;
	public float idleTimer;    
	public float barkTimer;
	public float escapeTimer;
	public float eatTimer;
	public float defaultEatTimer;
	public float defaultIdleTimer;
	public float defaultBarkTimer;
	public float defaultTimer;
	public float defaultEscapeTimer;
    public float defaultNewTargetTimer;
	public int playerOutOfSight;
	int targetIndex;
	int targetCounter = 0;
	public int areaCounter = 0;
	public float defaultTurnTimer;
    public float newTargetTimer;
    float raycastRange;
    public float defaultRaycastRange;
	public float patrolSpeed;
	public float chaseSpeed;
    public float chaseRange;
	Vector3[] path = new Vector3[0];
	Vector3 currentWaypoint;
	//values if enemy doesn't receive a new waypoint to prevent them from being stuck
	Vector3 worldPositionNow;
	Vector3 worldPositionPast;
    float x;
    float y;
    public float startingAngle;
    [Tooltip("What is the max amount the enemy can look from the starting angle before timer for zeroing starts to tick")]
    public float facingAngle;
    public float angleDiff;
    Vector3 startingVector;
    public bool wasChasing = false;
    Rigidbody dogRigidbody;

	void Start()
	{
		coneOfVisionScript = GetComponentInChildren<coneOfVision>();
		dogRigidbody = GetComponent<Rigidbody>();
		agent = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag("Player");
		ringOfSmellScript = player.GetComponentInChildren<ringOfSmell>();

		raycastRange = GetComponentInChildren<coneOfVision>().startRange; //defaultRaycastRange;
        turnTimerAlert = defaultTurnTimerAlert;
		//respawnPosition = this.transform.position;

		setDirectionsForIdle();
		setTargetWaypoints();
 
		currentTarget = targets[0];
		lastTarget = currentTarget;
		agent.speed = patrolSpeed;
		agent.SetDestination(currentTarget.position);
        stateManager((int)enumStatesFatDog.idle);
		
        //Setting Timers
		timer = defaultTimer;
		eatTimer = defaultEatTimer;
		idleTimer = defaultIdleTimer;
		barkTimer = defaultBarkTimer;
		escapeTimer = defaultEscapeTimer;
		turnTimer = defaultTurnTimer;

        x = transform.right.x * 1.0f;
        y = transform.right.z * 1.0f;
        startingAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        startingVector = transform.forward * 2.0f;
        respawnPosition = transform.position;
	}
	
	void Update()
	{
        x = transform.right.x;
        y = transform.right.z;

        /// Calcumalationen for ze vector differences///
        if (currentTarget != null)
        {
            vectorTransformPositionx = transform.position.x;
            vectorTransformPositionz = transform.position.z;

            vectorCurrentTargetx = currentTarget.position.x;
            vectorCurrentTargetz = currentTarget.position.z;

            vectorx = (vectorTransformPositionx - vectorCurrentTargetx);
            vectorz = (vectorTransformPositionz - vectorCurrentTargetz);
        }
        /// End of Calcumalationen for ze vector difference///

        // Calculation for the angle the enemy is facing right now //
        facingAngle = Mathf.Atan2(transform.right.x, transform.right.z) * Mathf.Rad2Deg;
        currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
        rotationDifference = targetAngle - currentAngle;
        // End of Calculation for the angle the enemy is facing right now //

		dogRigidbody.WakeUp();

		//------------------//
		//Code of the states//
		//------------------//
		switch(States)
		{
			case enumStatesFatDog.patrol:
			{
				//-----------------------------------------------------------------------------------------//
				//patrol, moves from one waypoint to the next waiting for a second before advancing forward//
				//-----------------------------------------------------------------------------------------//

	            if (newTargetTimer >= 0.0f)
	            {
	                agent.velocity = Vector3.zero;
	                newTargetTimer -= Time.deltaTime;
	            }

	                if (vectorx >= waypointOffsetMin && vectorx <= waypointOffsetMax && vectorz >= waypointOffsetMin && vectorz <= waypointOffsetMax)
	                {
	                    agent.Stop();
	                    agentStopped = true;
	                    stateManager((int)enumStatesFatDog.idle);
	                    onWaypoint = true;
	                }
			}
			break;
				
			case enumStatesFatDog.idle:
			{
				//--------------------------------------------------------//
				// idle, look around, without moving towards any waypoints//
				//--------------------------------------------------------//

				dogRigidbody.velocity = Vector3.zero;
				dogRigidbody.angularVelocity = Vector3.zero;  

	            agent.velocity = Vector3.zero;
				dogRigidbody.velocity = Vector3.zero;
	            agent.Stop();

                if (idleTimer <= 0)
                {
                    lastTarget = currentTarget;
                    if (currentTarget != targets[targetCounter])
                    {
                        currentTarget = targets[targetCounter];
                    }
                    
                    idleTimer = defaultIdleTimer;                    
                    stateManager((int)enumStatesFatDog.idleSuspicious);
                }

                idleTimer -= Time.deltaTime;

                if (idleTimer <= 0.0f)
                {
                    idleTimer = 0.0f;
                }
			}
			break;
			
			case enumStatesFatDog.chase:
			{        
	            wasChasing = true;
	            Vector3 direction = (player.transform.position - transform.position).normalized;
	            Physics.Raycast(transform.position, direction, out hit, raycastRange);
	            Debug.DrawRay(transform.position, direction * raycastRange, Color.white);
	            if (escapeTimer > 0)
	            {                
	                if (hit.collider != null)
	                {                    
	                    if (hit.collider.CompareTag ("Player"))
	                    {                       
	                        if (coneOfVisionScript.playerSeen == true)
	                        {
	                            if (currentTarget != player.transform)
	                            {
	                                lastTarget = currentTarget;
	                            }
	                            currentTarget = player.transform;

	                            Vector3 relative = transform.InverseTransformPoint(currentTarget.position);
	                            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;          
	                            transform.Rotate(0, angle * Time.deltaTime * 4.5f, 0);
	                        }

	                        if (barkTimer <= 0.0f)
	                        {
	                            bark();
	                        }
	                        barkTimer -= Time.deltaTime;
	                    }
	                    else
	                    {                        
	                        escapeTimer -= Time.deltaTime;
	                        if (escapeTimer <= 0.0f)
	                        {                         
	                            escapeTimer = 0.0f;
	                            if (currentTarget == player.transform)
	                            {
	                             	currentTarget = lastTarget;
	                            }
	                            escapeTimer = defaultEscapeTimer;                            
	                            coneOfVisionScript.playerSeen = false;
	                            if (turnCounter != 0)
	                            {
	                                turnCounter = 0;
	                            }
	                            stateManager((int)enumStatesFatDog.alert);
	                        }
	                    }
	                }
	                else
	                {                   
						escapeTimer -= Time.deltaTime;
	                    if (escapeTimer <= 0.0f)
	                    {
	                        escapeTimer = 0.0f;
	                        if (currentTarget == player.transform)
	                        {
	                            currentTarget = lastTarget;
	                        }
	                        escapeTimer = defaultEscapeTimer;
	                        coneOfVisionScript.playerSeen = false;
	                        if (turnCounter != 0)
                            {
                                turnCounter = 0;
                            }
							stateManager((int)enumStatesFatDog.alert);
	                    }
	                }
	            }
			}
			break;

        	case enumStatesFatDog.alert:
            {
                agentStopped = true;
                agent.velocity = Vector3.zero;
                agent.Stop();
               
                if (alertLookingDirectionsSet == false)
                {
                    //currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                    firstDirectionAlert = currentAngle + 30;
                    secondDirectionAlert = currentAngle - 30;
                    if (firstDirectionAlert >= 180)
                    {
                        firstDirectionAlert -= 360;
                    }
                    if (secondDirectionAlert <= -180)
                    {
                        secondDirectionAlert += 360;
                    }

                    alertLookingDirectionsSet = true;

                    if (directionDegreesAlert[0] != null)
                    {
                        for (int i = 0; i <= directionDegreesAlert.Count; i++)
                        {
                            directionDegreesAlert.Remove(i);
                        }
                    }
                    directionDegreesAlert.Add(firstDirectionAlert);
                    directionDegreesAlert.Add(secondDirectionAlert);
                    directionDegreesAlert.Add(firstDirectionAlert);
                    directionDegreesAlert.Add(secondDirectionAlert);
                }

                int tempAlertCount = directionDegreesAlert.Count;

                if (turnCounter < 4 && targetAngle != directionDegreesAlert[0])
                {
                    targetAngle = directionDegreesAlert[0];                       
                }
                else if (turnCounter >= 4)
                {
                    turnCounter = 0;
                    alertLookingDirectionsSet = false;
					stateManager((int)enumStatesFatDog.idleSuspicious);
                }                    
                else if (rotationCompleted)
                {
                    directionDegreesAlert.Add(directionDegreesAlert[0]);
                    directionDegreesAlert.Remove(directionDegreesAlert[0]);
                    rotationCompleted = false;
                    turnCounter++;
                    turnTimerAlert += defaultTurnTimerAlert;
                }
                else if (currentAngle != targetAngle)
                {
                    rotateEnemy(targetAngle, rotationStep); //castatastastast
                }
            }
            break;

        	case enumStatesFatDog.idleSuspicious:
            {
                angleDiff = facingAngle - startingAngle;
                if (wasChasing == true)
                {
                    if (angleDiff > angleOffsetMin && angleDiff < angleOffsetMax)
                    {
                        wasChasing = false;                      
                    }

                    if (wasChasing == true)
                    {
                        transform.Rotate(Vector3.up, -angleDiff * Time.deltaTime * 0.75f, 0);                     
                        if (angleOffsetMin < angleDiff && angleDiff < angleOffsetMax)
                        {
                            wasChasing = false; 
                        }
                    }
                }

                if (wasChasing == false)
                {
                    if (agentStopped == false || idleTimer > 0)
                    {
						idleTimer -= Time.deltaTime;
                        agentStopped = true;
                        agent.velocity = Vector3.zero;
                        agent.Stop();
                    }
                    else
                    {
                        //-----------------------------------------------//
                        //Stand on the spot and look at preset directions//
                        //-----------------------------------------------// 

                        float tempAngle = targetAngle - currentAngle;

                        if (coneOfVisionScript.playerSeen == true)
                        {
							stateManager((int)enumStatesFatDog.chase);
                        }

                        if (turnCounter < directionDegrees.Count && targetAngle != directionDegrees[0])
                        {
                            targetAngle = directionDegrees[0];
                        }

                        else if (turnCounter >= directionDegrees.Count)
                        {
                            turnCounter = 0;
                            idleTimer = defaultIdleTimer;
							stateManager((int)enumStatesFatDog.idle);
                        }

                        if (rotationCompleted)
                        {
                            directionDegrees.Add(directionDegrees[0]);
                            directionDegrees.Remove(directionDegrees[0]);
                            rotationCompleted = false;
                            turnCounter++;
                            turnTimer += defaultTurnTimer;
                        }
                        else if(rotationCompleted == false)
                        {
                            rotateEnemy(targetAngle, rotationStep);
                        }
                   
                        if (idleTimer < 0)
                        {
                            idleTimer = 0;
                        }
                    }
                }
            }
            break;

			case enumStatesFatDog.distracted:	
			{
				//-------------------------//
				// Move towards distraction//
				//-------------------------//
	            if (agentStopped)
	            {
	                agent.Resume();
	            }
	            
				distracted = true;
				Vector3 bonedir = (currentTarget.transform.localPosition) - (this.transform.localPosition);
				if (bonedir.x <= 4 && bonedir.x >= -4 && bonedir.z <= 4 && bonedir.z >= -4)
				{
					stateManager((int)enumStatesFatDog.eatBone);
					distracted = false;
					if (!eatBone)
					{
						eatTimer = defaultEatTimer;
					}
					
					eatBone = true;
				}
			}
			break;

			case enumStatesFatDog.detectSound:
			{
                if (soundSource && soundSource.CompareTag ("Bone"))
                {
                    currentTarget = soundSource.transform;
                }
                else
                {
                    currentTarget = lastTarget;
                    if (vectorx >= (waypointOffsetMin) && vectorx <= (waypointOffsetMax) && vectorz >= (waypointOffsetMin) && vectorz <= (waypointOffsetMax))
                    {
						stateManager((int)enumStatesFatDog.patrol);
                    }
                }
            
				//---------------------------------------------//
				// when sound is heard, move towards the source//
				//---------------------------------------------//
	            if (vectorx >= (waypointOffsetMin * 2) && vectorx <= (waypointOffsetMax * 2) && vectorz >= (waypointOffsetMin * 2) && vectorz <= (waypointOffsetMax * 2))
	            {
	                if (soundSource != null || !soundSource.Equals(null))
	                {
	                    Physics.Linecast(this.transform.position, soundSource.transform.position, out hit);
	                   // Debug.DrawLine(this.transform.position, soundSource.transform.position, Color.blue);
	                }
	                //alertTimer = defaultAlertTimer;

	                if (hit.collider != null)
	                {
	                    if (agentStopped == false)
	                    {
	                        agentStopped = true;
	                        agent.Stop();
	                    }
	                    if (hit.collider.CompareTag ("Bone"))
	                    {
	                        //randomPointSelected = false;
							stateManager((int)enumStatesFatDog.eatBone);
	                    }
	                }
	            }                          
			}
			break;
			case enumStatesFatDog.eatBone:
			{
				//------------------------------------------------------------------//
				// holds the enemy still for long enough for the distraction to pass//
				//------------------------------------------------------------------//

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
	                //alertTimer += defaultAlertTimer;

					stateManager((int)enumStatesFatDog.patrol);
	                eatTimer = defaultEatTimer;
	                currentTarget = lastTarget;
	                //currentTarget = alertArea[areaCounter];
	            }

	            if (eatTimer <= 0)
	            {
	                eatTimer = defaultEatTimer;
	                distracted = false;
	                eatBone = false;
	                currentTarget = lastTarget;
	               // currentTarget = alertArea[areaCounter];
	                breakableObject boneScript = bone.GetComponent<breakableObject>();
	                boneScript.destroySelf();
	               // alertTimer += defaultAlertTimer;
					stateManager((int)enumStatesFatDog.patrol);
	            }
				eatTimer -= Time.deltaTime;
	            if (eatTimer <= 0.0f)
	            {
	                eatTimer = 0.0f;
	            }
	            //}
	        }
			break;

		default:
			break;
		}

        { 
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Physics.Raycast(transform.position, direction, out hit, (ringOfSmellScript.radius * 0.48f));
            Debug.DrawRay(transform.position, direction * ringOfSmellScript.radius * 0.48f, Color.yellow);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag (player.GetComponent<Collider>().tag))
                {
                    rotateDogWhileSmelling(player.transform.position);
                }
            }
        }
        
		// 
		if(timer <= 0)
		{
			timer+=defaultTimer;

            if (States != enumStatesFatDog.idleSuspicious && States != enumStatesFatDog.chase)
			{
				if(currentTarget != null && currentTarget.position != transform.position)
				{
                    //agent.Stop();
					agent.SetDestination(currentTarget.position);
                    agent.Resume();
                    if (newTargetTimer <= 0 && onWaypoint == true)
                    {
                        newTargetTimer = defaultNewTargetTimer;
                        onWaypoint = false;
                    }
				}
                else if(currentTarget == null)
                {
                    currentTarget = lastTarget;
                }
                else
                {
                    agent.Stop();
                }
			}
		}
		timer -= Time.deltaTime;
	}

	//======================================================================//
	//	To change the FSM state of the current object						//
	//======================================================================//

	public void stateManager(int value)
	{
		States = (enumStatesFatDog)value;
	}

	//======================================================================//
	//	Set the directions to look at (Only for init)						//
	//======================================================================//

	void setDirectionsForIdle()
	{
		directionDegrees.Add(firstDirection);
		directionDegrees.Add(secondDirection);
		directionDegrees.Add(thirdDirection);
        directionDegrees.Add(fourthDirection);
	}

	//======================================================================//
	//	Set the waypoints (Only for init)									//
	//======================================================================//

	void setTargetWaypoints()
	{
		if (target1 != null) 
		{
			targets.Add(target1);
		}
	}

	//======================================================================//
	//	Barking is called in chase state									//
	//======================================================================//

    void bark()
    {
        newSphere = (GameObject)Instantiate(sphere, this.transform.position, Quaternion.identity);
        newSphere.transform.parent = transform;
		newSphere.tag = "SoundSphere";
        barkTimer = defaultBarkTimer;
        if (newSphere)
        {
            sphereScript = newSphere.GetComponent<soundSphere>();
            sphereScript.setMaxDiameter(maxScale);
        }
    }
	
	//======================================================================//
	//	Rotation of the enemy												//
	//======================================================================//

	void rotateEnemy(float targetDegrees, float rotationStep)
	{
        //if (ringOfSmellScript.smellDetected == false)
        {
            rotationDifference = 0;

            if (turnTimer <= 0)
            {
                if (rotationInProgress == false)
                {
                    //currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                    targetAngle = targetDegrees;//currentAngle + targetDegrees;
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
                                if (currentAngle <= targetAngle || (currentAngle > targetAngle - 180 && currentAngle < 0))
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                    //currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;

                                    if (rotationDifference < 0)
                                    {
                                        rotationDifference = rotationDifference * -1;
                                    }
                                  
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                        //turnTimer += defaultTurnTimer;
                                    }
                                }
                                else //if (currentAngle > targetAngle && turnTimer == 0)
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                    //currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;

                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                    }
                                }
                            }
                            //=============//
                            //Second Sector//
                            //=============//    
                            else if (targetAngle > 90 && targetAngle <= 180)// decide which sector the target is
							{
                                if (targetAngle < currentAngle || (currentAngle <= targetAngle - 180 && currentAngle < 0))
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                    //currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                    if (rotationDifference < 0)
                                    {
                                        rotationDifference = rotationDifference * 1;
                                    }

                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                    }
                                }
                                else //if (currentAngle > targetAngle & targetAngle - 180 > currentAngle)
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                   // currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
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
                                if ((currentAngle >= targetAngle  && currentAngle < 0) || (currentAngle <= 180 + targetAngle  && currentAngle > 0))
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                   // currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                    if (rotationDifference < 0)
                                    {
                                        rotationDifference = rotationDifference * -1;
                                    }
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                    }
                                }
                                else //if (currentAngle < targetAngle && turnTimer == 0)
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                  //  currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                  
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                    }
                                }
                            }
                            //=============//
                            //Fourth Sector//
                            //=============//
                            else if (targetAngle < -90)// decide which sector the target is. 4 different sectors 0-90, 90-180, 0-(-90), (-90)- (-180)
                            {
                                if ((currentAngle >= targetAngle && currentAngle <= 0) ||  (currentAngle <= 180 + targetAngle && currentAngle >= 0))
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * 1);
                                   // currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                    if (rotationDifference < 0)
                                    {
                                        rotationDifference = rotationDifference * -1;
                                    }
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                       // turnTimer += defaultTurnTimer; // *Time.deltaTime;
                                    }
                                }
                                else //if (currentAngle < targetAngle && turnTimer == 0)
                                {
                                    transform.Rotate(Vector3.up * Time.deltaTime * rotationStep * -1);
                                  //  currentAngle = Mathf.Atan2(transform.right.z, transform.right.x) * Mathf.Rad2Deg;
                                    rotationDifference = targetAngle - currentAngle;
                                 
                                    if (currentAngle == targetAngle || angleOffsetMin <= rotationDifference && rotationDifference <= angleOffsetMax)
                                    {
                                        rotationCompleted = true;
                                        rotationInProgress = false;
                                        //turnTimer += defaultTurnTimer; // *Time.deltaTime;
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
                if (turnTimer < 0.0f)
                {
                    turnTimer = 0.0f;
                }
            }
        }
	}

	//======================================================================//
	//	This is to rotate enemy towards a smell before he detects the cause //
	//	of the smell.														//
	//======================================================================//

    public void rotateDogWhileSmelling(Vector3 targetTransformPosition)
    {
            //SeekForSmellSource = true;
            agentStopped = true;
            agent.Stop ();
            Vector3 relative = transform.InverseTransformPoint(targetTransformPosition);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
            transform.Rotate(0, angle * Time.deltaTime * 1.5f, 0);
    }

	//======================================================================//
	//	Reset() to be called in checkPoint.cs to reset the object when  	//
	//	respawn back to checkpoint is called.								//
	//======================================================================//

	public void Reset() 
	{
		// Stop navmesh
		agent.Stop ();
		agent.velocity = Vector3.zero;

		// Reset all the timers
		turnTimerAlert = defaultTurnTimerAlert;

		eatTimer = defaultEatTimer;
		idleTimer = defaultIdleTimer;
		barkTimer = defaultBarkTimer;
		escapeTimer = defaultEscapeTimer;
		turnTimer = defaultTurnTimer;

		// Change the state to 
		stateManager ((int)enumStatesFatDog.idleSuspicious);
		agent.speed = patrolSpeed;
		currentTarget = targets[0];
		lastTarget = currentTarget;
		agent.SetDestination (currentTarget.position);

		// Delete sound source 
		soundSource = null;

		// Reset the position and velocity of the rigidbody
		dogRigidbody.Sleep ();
		dogRigidbody.velocity = Vector3.zero;
		dogRigidbody.angularVelocity = Vector3.zero;
		dogRigidbody.WakeUp ();
		//dogRigidbody.AddForce ();
		transform.position = respawnPosition;
	}
}

//if (ringOfSmellScript.smellDetected == false)
//if (startingAngle > 0)
//{
//    //First we look the border areas in our sector. If the starting angle isn't in the sector 
//    //made with angle we're currently facing and offset, we can assume that enemy is not facing
//    //the right direction anymore and we need to set him into a starting angle.
// 
//
//    //firstly check the area around 180°
//    if(facingAngle + angleOffSet > 180)
//    {
//        float tempAngle = facingAngle + angleOffSet;
//        tempAngle -= 360;
//
//        // check if between current angle and 180° 
//        if ( 180 >= startingAngle  && startingAngle >= facingAngle)
//        { 
//         // enemy is correctly set
//        }
//
//        else if ( -180 <= startingAngle && startingAngle <= tempAngle)
//        {
//            //enemy is correctly set
//        }
//    }
//
//
//    //first check the the area around 0°
//    if(facingAngle - angleOffSet < 0)
//    {
//  
//    }
//
//
//    // this is for the sector 0° - 180° to check if the enemy is facing the right direction.
//    // If they don't then we need to move them to face the right direction                 
//    if (facingAngle < startingAngle && startingAngle < facingAngle + (angleOffSet / 2) || facingAngle - (angleOffSet / 2) < startingAngle && startingAngle < facingAngle)
//    {
//        // Enemy is where it should be, no need to do anything
//    }
//
//    else 
//    {
//      // Enemy is somewhere where it should not be, it needs to turn to face the starting location
//    }
//    //2.
//    if(facingAngle - (angleOffSet/2) < startingAngle && startingAngle < facingAngle)
//    {
//
//    }
//
//}
//
//else if (startingAngle < 0)
//{
//    //1.
//    if (facingAngle + (angleOffSet/2) < startingAngle && startingAngle > facingAngle)
//    {
//
//    }
//    //2.
//    else if(facingAngle > startingAngle && startingAngle > facingAngle - (angleOffSet/2))
//    {
//    
//    }
//}
