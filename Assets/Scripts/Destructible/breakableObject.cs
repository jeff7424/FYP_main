using UnityEngine;
using System.Collections;
//-----------------------------------------------------//
// A destroyable object, creates a sound when destroyed//
//-----------------------------------------------------//
public class breakableObject: MonoBehaviour 
{
    public bool makeSound = false;
    public float timer;
	public float defaultTimer = 1.0f;
    public float expireTimer = 10.0f;
    public float boneRadius;
    public float ballRadius;
    public float cubeRadius;
	public float maxScale;
	public GameObject originalObject;
	public GameObject brokenPieces;
	public GameObject Sphere;
	public GameObject brokenSphere;
	public GameObject brokenCube;
	public TemporaryMovement playerMovement;

	private bool isGrounded;
	private float m_GroundCheckDistance;
	private GameObject newSphere;
	private GameObject brokenObject;
	private GameObject bone;
	private soundSphere sphereScript;
	private sfxPlayer SFX;
	private Rigidbody rb;

	void Start () 
    {
        //---------------------------------------------------//
        // set the volume of the sound sphere for this object//
        //---------------------------------------------------//
        if (this.gameObject.CompareTag ("Bottle")) 
		{
			maxScale = 15.0f;
			if (originalObject != null && brokenPieces != null)
			{
				originalObject.SetActive(true);
				brokenPieces.SetActive (false);
			}
		} 
		else if (this.gameObject.CompareTag ("Glass")) 
		{
			maxScale = 40.0f;
		} 
		else if (this.gameObject.CompareTag ("Jar")) 
		{
			maxScale = 50.0f;
		}

		timer = defaultTimer;

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<TemporaryMovement>();

		SFX = GameObject.Find ("SFX").GetComponent<sfxPlayer> ();

        m_GroundCheckDistance = 0.6f;
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
        }
		newSphere = (GameObject)Instantiate (Sphere, this.transform.position, Quaternion.identity);
		newSphere.transform.parent = transform.parent;
		newSphere.SetActive (false);
		rb = GetComponent<Rigidbody> ();
	}

	void Update () 
    {
        //---------------------------------------------//
        // expands the sound sphere until maximum range//
        //---------------------------------------------//

//        if (this.gameObject.CompareTag ("Bone")) {
//			if (timer <= 0.0f) {
//				makeSound = false;
//				timer += defaultTimer;
//				newSphere = (GameObject)Instantiate (Sphere, this.transform.localPosition, Quaternion.identity);
//				newSphere.transform.parent = transform;
//				newSphere.tag = "Bone";
//				sphereScript = newSphere.GetComponent<soundSphere> ();
//				sphereScript.setMaxDiameter (boneRadius);
//				expireTimer--;
//			}
//			timer -= Time.deltaTime;
//		} else if (this.gameObject.CompareTag ("BagOfAir")) {
//			if (timer <= 0) {
//				makeSound = false;
//				timer += defaultTimer;
//				newSphere = (GameObject)Instantiate (Sphere, this.transform.localPosition, Quaternion.identity);
//				newSphere.transform.parent = transform;
//				sphereScript = newSphere.GetComponent<soundSphere> ();
//				sphereScript.setMaxDiameter (boneRadius);
//				expireTimer--;
//			}
//			timer -= Time.deltaTime;
//		}
		if (gameObject.CompareTag ("Bone") || gameObject.CompareTag ("BagOfAir")) {
			if (timer <= 0.0f) {
				makeSound = false;
				timer += defaultTimer;
				if (newSphere == null) {
					newSphere = (GameObject)Instantiate (Sphere, this.transform.localPosition, Quaternion.identity);
				} else {
					newSphere.SetActive (true);
				}
				newSphere.transform.parent = transform;
				if (gameObject.CompareTag ("Bone")) {
					newSphere.tag = "Bone";
				}
				sphereScript = newSphere.GetComponent<soundSphere> ();
				sphereScript.setMaxDiameter (boneRadius);
				expireTimer--;
			}
			timer -= Time.deltaTime;
		} 
	}

    void LateUpdate()
    {
        if (expireTimer <= 0)
        {
            destroySelf();
        }
    }

	void FixedUpdate()
	{
	    // uses raycast to check if bone is grounded
	    if (this.gameObject.CompareTag ("Bone"))
	    {
	        checkGroundStatus();
	        if (isGrounded)
	        {
	            rb.useGravity = false;
	            rb.velocity = Vector3.zero;
	            rb.angularVelocity = Vector3.zero;
	        }
		}
	}
    
    void OnCollisionEnter(Collision Other)
    {
        //----------------------------------------------------------//
        // When object falls to the ground it creates a sound sphere//
        //----------------------------------------------------------//
        if (Other.gameObject.CompareTag ("Player")) {
			rb.AddRelativeForce (playerMovement.transform.forward * playerMovement.movementSpeed, ForceMode.Force);
			//makeSound = true;
		} else {
			if (originalObject != null && brokenPieces != null) {
				Debug.Log (rb.velocity.magnitude);
				if (rb.velocity.magnitude > 0.5f) {
					brokenPieces.transform.position = originalObject.transform.position;
					brokenPieces.transform.rotation = originalObject.transform.rotation;
					originalObject.SetActive (false);
					brokenPieces.SetActive (true);
					makeSound = true;
				}
			}
		}
//        if (this.transform.position.y <= 0.5f && makeSound == true)
//        {
//            //makeSound = false;
//            {
//                objectBreaking();
//				SFX.playGlassBreak();
//            }
//        }
    }

	void OnCollisionStay(Collision other)
	{
		if (other.gameObject.CompareTag ("Player") == false)
		{
			if (originalObject != null && brokenPieces != null)
			{
				Debug.Log (other.gameObject);
				Debug.Log (rb.velocity.magnitude);
				if (rb.velocity.magnitude > 0.5f)
				{
					brokenPieces.transform.position = originalObject.transform.position;
					brokenPieces.transform.rotation = originalObject.transform.rotation;
					originalObject.SetActive(false);
					brokenPieces.SetActive (true);
					makeSound = true;
				}
				if (makeSound == true)
				{
					objectBreaking();
				}
			}
		}
	}

    public void ObjectFalling()
    {    
        makeSound = true;
    }

    public void destroySelf()
    {
        if(gameObject.CompareTag ("Bone"))
        {
			playerMovement.reduceBonePlacedNumber();
        }
        Destroy(gameObject);
    }

    public void objectBreaking()
    {
        if (this.gameObject.CompareTag ("Bottle") || this.gameObject.CompareTag ("Jar"))
        {
            //brokenObject = (GameObject)Instantiate(brokenSphere, this.transform.position, Quaternion.identity);
            //newSphere = (GameObject)Instantiate(Sphere, this.transform.position, Quaternion.identity);
			if (newSphere.activeInHierarchy == false)
			{
				newSphere.SetActive(true);
			}
			//newSphere.transform.parent = brokenPieces.transform;
            sphereScript = newSphere.GetComponent<soundSphere>();
            sphereScript.setMaxDiameter(maxScale);
            destroySelf();
        }
        else if (this.gameObject.CompareTag ("Glass"))
        {
            //brokenObject = (GameObject)Instantiate(brokenCube, this.transform.localPosition, Quaternion.identity);
            //newSphere = (GameObject)Instantiate(Sphere, this.transform.position, Quaternion.identity);
			if (newSphere.activeInHierarchy == false)
			{
				newSphere.SetActive(true);
			}
			//newSphere.transform.parent = brokenPieces.transform;
            sphereScript = newSphere.GetComponent<soundSphere>();
            sphereScript.setMaxDiameter(maxScale);
            destroySelf();
        }

//        if (this.gameObject.CompareTag ("jar"))
//        {
//            brokenObject = (GameObject)Instantiate(brokenSphere, this.transform.position, Quaternion.identity);
//            newSphere = (GameObject)Instantiate(Sphere, this.transform.position, Quaternion.identity);
//            newSphere.transform.parent = brokenObject.transform;
//            sphereScript = newSphere.GetComponent<soundSphere>();
//            sphereScript.setMaxDiameter(maxScale);
//            destroySelf();
//        }
    }

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
}