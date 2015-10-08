using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TemporaryMovement : MonoBehaviour
{
	public bool thirdPersonControls;
	public float magnMultiplier;
	public float movementSpeed;
	public float sprintModifier;
	public float origMovementSpeed;
	public float jumpHeight;
	public float duration = 0.2f;
	public float defaultBoneCooldown;
	public float boneSpawnTimer;
	public float defaultBoneSpawnTimer;
	private float durationOfSpriteAnimationBone;
	public float grav;
	private float durationOfSpriteAnimationBag;
	public float throwForce;
	public string movieFolder;
	private int bonesPlaced;
	
	RaycastHit hit;
	
	public int maxBonesPlaced;
	public int maxBones;
	public int bones = 2;
	public int bags;
	
	[HideInInspector]
	public Rigidbody rb;
	
	Animator catAnim;
	
	public GameObject bone;
	public GameObject bagOfAir;
	GameObject boneSpawner;
	GameObject newBone;
	GameObject newBagOfAir;

	public int maxTraps;
	public int trapPlaced;
	public GameObject trap;
	
	public bool isGrounded;
	private bool isEsc;    
	bool smellHidden;
	bool disguisedAsDog;
	public bool playerHidden;
	
	public List<GameObject> enemies = new List<GameObject>();
	
	ringOfSmell ring;
	
	public bool onLadder;
	
	public Vector3 movement;
	
	public Image boneCoolDown;
	public Image bagCoolDown;
	public Image boneBackground;
	public Image bagBackground;
	
	public AnimationClip spriteAnimationBone;
	public AnimationClip spriteAnimationBag;
	public int numberOfKeys;
	
	public int numberOfFishes;
	public int numberOfRescue;
	
	public int[] keyPossessed = new int[4];
	private float joystickPressure;
	private float horizontal;
	private float vertical;
	private float sprintSpeed;
	private float boneCooldown;
	private float trapCooldown;
	private float m_GroundCheckDistance;
	private float m_OrigGroundCheckDistance;
	private Vector3 look;
	public Camera mainCam;
	
	IEnumerator spriteBoneTimer()
	{
		boneCoolDown.GetComponent<Animator>().enabled = true;
		
		yield return new WaitForSeconds(durationOfSpriteAnimationBone);
		
		boneCoolDown.GetComponent<Animator> ().playbackTime = 0.0f;
		boneCoolDown.GetComponent<Animator>().enabled = false;
	}
	
	IEnumerator spriteBagTimer()
	{
		bagCoolDown.GetComponent<Animator>().enabled = true;
		
		yield return new WaitForSeconds(durationOfSpriteAnimationBag);
		
		bagCoolDown.GetComponent<Animator>().enabled = false;
	}
	
	void Start()
	{
		//Physics.gravity = new Vector3(0.0f, -grav, 0.0f);
		
		//boneCoolDown.GetComponent<Animator>().enabled = false;
		//bagCoolDown.GetComponent<Animator>().enabled = false;
		
		//bagCoolDown.enabled = false;
		boneSpawnTimer = defaultBoneSpawnTimer;
		m_GroundCheckDistance = 0.6f;
		rb = GetComponent<Rigidbody>();
		catAnim = GetComponent<Animator>();
		origMovementSpeed = movementSpeed;
		sprintSpeed = movementSpeed * sprintModifier;
		ring = GetComponentInChildren<ringOfSmell>();
		boneSpawner = GameObject.FindGameObjectWithTag("BoneSpawner");
		durationOfSpriteAnimationBone = spriteAnimationBone.length;
		durationOfSpriteAnimationBag = spriteAnimationBag.length;
		isEsc = !isEsc;
		//mainCam = Camera.main;
		
		//numberOfBones = GetComponent<Text>().text--;
	}
	
	void FixedUpdate()
	{
		if(boneCooldown > 0 )
		{
			boneCooldown -= Time.deltaTime;
		}
		bags = inventory.inventoryArray[0];
		sprint();
		updateAnimator();
		
		checkGroundStatus();
		
		if (playerHidden == false) 
		{
			horizontal = Input.GetAxis ("Horizontal");
			vertical = Input.GetAxis ("Vertical");
		}
		
		if (thirdPersonControls == false) 
		{
			movement = new Vector3 (1, 0, 1) * vertical + new Vector3 (1, 0, -1) * horizontal;  
			look = new Vector3(-1, 0, 1) * vertical + new Vector3(1, 0, 1) * horizontal;
			rb.MovePosition(transform.position + movement.normalized * (movementSpeed + movement.magnitude) * Time.deltaTime);
			transform.LookAt(transform.position + look, Vector3.up);
		} 
		else 
		{
			if (horizontal != 0 || vertical != 0) 
			{
				movement = new Vector3 (vertical, 0, -horizontal);
				
				// Calculate the direction to move to
				Vector3 v = (mainCam.transform.forward * vertical);
				Vector3 h = (mainCam.transform.right * horizontal);
				// Set y to 0 because we don't want to add the y to the velocity of the character
				v.y = 0.0f;
				h.y = 0.0f;
				
				rb.MovePosition (transform.position + (v.normalized + h.normalized) * (movementSpeed + movement.magnitude) * Time.deltaTime);
				
				// Calculate the rotation of the character
				Vector3 lookV = (mainCam.transform.right * vertical);
				Vector3 lookH = (mainCam.transform.forward * horizontal);
				lookH.y = 0.0f;
				
				transform.LookAt(transform.position - lookV + lookH, Vector3.up);
			}
		}
		
		
		if ((Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("Fire3")) && bones > 0 && bonesPlaced < maxBonesPlaced && boneCooldown <= 0) // BONE
		{
			if(playerHidden == false)
			{
				boneCoolDown.enabled = true;
				//bagCoolDown.enabled = false;
				
				boneBackground.enabled = true;
				//bagBackground.enabled = false;
				
				boneBackground.CrossFadeAlpha(1.0f, duration, true);
				// bagBackground.CrossFadeAlpha(0.0f, duration, true);
				
				StartCoroutine(spriteBoneTimer());
				bones--;
				bonesPlaced++;
				
				Vector3 direction = (boneSpawner.transform.position - this.transform.position).normalized;
				Physics.Raycast(transform.position, direction, out hit, 1.3f);
				Debug.DrawRay(transform.position, direction * 1.3f, Color.blue, 2.0f);
				if (hit.collider != null)
				{
					if (hit.collider.CompareTag ("Wall"))
					{
						Vector3 vectorBetweenWallPlayer = hit.point - this.transform.position;
						float distanceWallPlayer = vectorBetweenWallPlayer.magnitude;
						vectorBetweenWallPlayer = vectorBetweenWallPlayer.normalized;
						Debug.DrawRay(transform.position, vectorBetweenWallPlayer * distanceWallPlayer, Color.red, 5.0f);
						boneSpawner.transform.position = this.transform.position + (distanceWallPlayer * vectorBetweenWallPlayer);
					}
				}
				
				else
				{
					Vector3 length = (boneSpawner.transform.position - this.transform.position).normalized;
					boneSpawner.transform.position = this.transform.position + (length * 1.3f);
				}
				
				newBone = (GameObject)Instantiate(bone, boneSpawner.transform.position, Quaternion.identity);
				boneCooldown = defaultBoneCooldown;
			}
		}

		//PutTrap
		if (Input.GetKeyDown (KeyCode.R) && trapPlaced < maxTraps) {
			if (playerHidden == false) {
				Vector3 direction = (boneSpawner.transform.position - transform.position).normalized;
				Instantiate (trap, boneSpawner.transform.position, Quaternion.identity);
				trapPlaced++;
			}
		}

		if (Input.GetKeyDown(KeyCode.Y) /*&& bags > 0*/) // BAG
		{
			boneCoolDown.enabled = false;
			bagCoolDown.enabled = true;
			
			boneBackground.enabled = false;
			bagBackground.enabled = true;
			
			boneBackground.CrossFadeAlpha(0.0f, duration, true);
			bagBackground.CrossFadeAlpha(1.0f, duration, true);
			
			StartCoroutine(spriteBagTimer());
			
			bags--;
			newBagOfAir = (GameObject)Instantiate(bagOfAir, boneSpawner.transform.position, Quaternion.identity);
		}
		
		if (Input.GetKeyDown(KeyCode.G)) // DISGUIUSED AS A DOG
		{
			if (!disguisedAsDog)
			{
				disguisedAsDog = true;
				disGuiseAsDog();
			}
			else if (disguisedAsDog)
			{
				disguisedAsDog = false;
				disGuiseAsDog();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.H)) // CANNOT SMELL
		{
			if (smellHidden)
			{
				smellHidden = false;
				ring.isNotDisguised("tempMove");
			}
			else if (!smellHidden)
			{
				smellHidden = true;
				ring.isDisguised("tempMove");
			}
		}
		
		if (boneSpawnTimer <= 0)
		{
			if (bones < maxBones)
			{
				bones++;
				boneSpawnTimer = defaultBoneSpawnTimer;
			}
		}
		else if (boneSpawnTimer > 0)
		{
			boneSpawnTimer -= Time.deltaTime;
		}
	}
	
	void Update()
	{
		//checks if character is grounded
		if (isGrounded)
		{
			catAnim.SetBool("isOnGround", true);
			catAnim.speed = 1;
			if (Input.GetButtonDown("Jump"))
			{
				rb.AddForce(Vector3.up * (jumpHeight * 100)); // *100 is just here so that we don't have to enter scary values in the inspector
			} 
		}
		else
		{
			//catAnim.speed = 0.1f;
			if (onLadder == false) 
				catAnim.SetBool("isOnGround", false);
			rb.AddForce(Vector3.down * (grav / 10)); // /10 is just here so that we don't have to enter scary values in the inspector
		}
		
		if (onLadder == true && catAnim.GetBool ("isOnClimbing") == false) 
		{
			catAnim.SetBool ("isOnGround", true);
			catAnim.SetBool ("isClimbing", true);
		} 
		else if (onLadder == true && catAnim.GetBool ("isOnClimbing") == true)
		{
			if (movement.magnitude > 0.5f)
			{
				catAnim.speed = movement.magnitude;
			}
		}
	}
	
	void sprint()
	{
		if (isGrounded)
		{
			// WALK / Sprint ACCORDING JOYSTICK PRESSURE
			/*
            if (movement.magnitude > 1.6)
            {
                catAnim.SetBool("isSprinting", true);
                movementSpeed = sprintSpeed;
            }

            else if (horizontal > joystickPressure || horizontal < -joystickPressure || vertical > joystickPressure || vertical < -joystickPressure || movementSpeed > 5)
            {
                catAnim.SetBool("isSprinting", true);
                movementSpeed = sprintSpeed;
            }

            else
            {
                catAnim.SetBool("isSprinting", false);
                movementSpeed = origMovementSpeed;
            }
            */
			// WALK / Sprint WITH THE USE OF A BUTTON
			
			if (Input.GetButton("Sprint") && movement.magnitude > 0.1)
			{
				catAnim.SetBool("isSprinting", true);
				movementSpeed = sprintSpeed;
			}
			else
			{
				catAnim.SetBool("isSprinting", false);
				movementSpeed = origMovementSpeed;
			}
		}
	}
	
	void updateAnimator()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		
		catAnim.SetFloat("hSpeed", horizontal);
		catAnim.SetFloat("vSpeed", vertical);
	}
	
	// uses raycast to check if player is grounded
	void checkGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
		#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.transform.parent != null)
		{
			if ((Input.GetKeyDown(KeyCode.Return) && other.transform.parent.CompareTag ("Ball") || other.transform.parent.CompareTag ("Cube")))
			{
				other.transform.parent.GetComponent<Rigidbody>().AddForce(Vector3.forward * throwForce, ForceMode.Force);
				other.transform.GetComponentInParent<breakableObject>().ObjectFalling();
			}
			
			if (other.transform.parent.CompareTag ("Trap"))
			{
				
			}
		}
	}
	
	void OnCollisionEnter(Collision col)
	{
		if (col.collider.CompareTag ("Trap"))
		{
			breakableObject trap = col.collider.transform.GetComponent<breakableObject>();
			trap.makeSound = true;
		}
	}
	
	
	void OnTriggerEnter(Collider coll)
	{
		if (coll.gameObject.CompareTag ("Ladder"))
		{
			if (Vector3.Distance(this.transform.position, coll.transform.position) < 1.5f)
			{
				catAnim.SetBool("isClimbing", true);
				onLadder = true;
			}
		}
	}
	
	void OnTriggerExit(Collider coll)
	{
		if (coll.gameObject.CompareTag ("Ladder"))
		{
			onLadder = false;
			catAnim.SetBool("isClimbing", false);
		}
	}
	
	
	void disGuiseAsDog()
	{
		GameObject[] patrolEnemy;
		GameObject[] hunterEnemy;
		enemies.Clear();
		patrolEnemy = GameObject.FindGameObjectsWithTag("Enemy");
		hunterEnemy = GameObject.FindGameObjectsWithTag("HuntingDog");
		
		foreach (GameObject enemy in patrolEnemy)
		{
			enemies.Add(enemy);
		}
		
		foreach (GameObject enemy in hunterEnemy)
		{
			enemies.Add(enemy);
		}
		
		if (disguisedAsDog)
		{
			foreach (GameObject enemy in enemies)
			{
				coneOfVision cone = enemy.GetComponentInChildren<coneOfVision>();
				cone.isDisguised();
			}
		}
		else if (!disguisedAsDog)
		{
			foreach (GameObject enemy in enemies)
			{
				coneOfVision cone = enemy.GetComponentInChildren<coneOfVision>();
				cone.isNotDisguised();
			}
		}
	}
	
	public void reduceBonePlacedNumber()
	{
		if (bonesPlaced > 0) 
		{
			bonesPlaced--;
		}
	}

	public void reduceTrapPlacedNumber()
	{
		if (trapPlaced > 0) 
		{
			trapPlaced--;
		}
	}

	public void resetKeys()
	{
		keyPossessed = new int[4];
		numberOfKeys = 0;
	}
	
	public void resetFishes()
	{
		numberOfFishes = 0;
	}
	
	public void resetCharacter()
	{
		// Reset player's velocity
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		
		playerHidden = false;
		
		//Clear Fishes
		//resetFishes ();
		
		// Clear keys
		resetKeys ();

		//ShowNumberOfFishes
		Debug.Log (numberOfFishes);
		Debug.Log (numberOfRescue);

		// Reset ring
		ring.isNotDisguised ("tempMove");
	}
}