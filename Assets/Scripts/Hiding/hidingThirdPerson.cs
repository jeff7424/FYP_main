using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class hidingThirdPerson : MonoBehaviour {

	public bool isHiding;
	public bool isPaused;
	public bool isEntered;
    public Transform character;
    public Transform prevPosition;
    public Transform hidingPosition;
	public Transform cameraPosition;
    public TemporaryMovement playerScript;
	public GameObject showHiding;

	private ringOfSmell ros;
	private thirdPersonCamera characterCamera;
	//private checkPoint cp;

	void Start () 
	{
		isHiding = false;
		isPaused = false;
		isEntered = false;

		//cp = character.GetComponent<checkPoint>();
		ros = character.FindChild ("Smell ring FBX").GetComponent<ringOfSmell>();
		playerScript = character.GetComponent<TemporaryMovement>();
		characterCamera = playerScript.mainCam.GetComponent<thirdPersonCamera> ();
		//cameraPosition.localRotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
		//Debug.Log (cameraPosition.eulerAngles.y);
		showHiding.SetActive(false);
	}

    void OnTriggerStay(Collider catType)
    {	
		if (catType.CompareTag ("Player"))
		{	
			isEntered = true;
			//keyboardCheckToEnter.SetActive(true);
			//controllerCheckToEnter.SetActive(true);

			if (Input.GetButtonDown ("Interact"))
			{
				if (isHiding == false)
				{
					if (Input.GetButtonDown("Interact"))
					{
						StartCoroutine(Wait());
					}
				}
				else if (isHiding == true) 
				{
					if (Input.GetButtonDown("Interact"))
					{
						StartCoroutine (Delayed ());
					}
				} 
			}
		}
    }

	void OnTriggerExit(Collider cat)
	{	
		if (cat.CompareTag ("Player")) 
		{
			isEntered = false;
		}
		//keyboardCheckToEnter.SetActive(false);
		//controllerCheckToEnter.SetActive(false);
	}

//	void Update ()
//    { 
//        if (isHiding == true) 
//		{
//			if (Input.GetButtonDown("Interact"))
//			{
//				StartCoroutine (Delayed ());
//
//				isHiding = false;
//				isPaused = false;            
//                if(ros.disguised == true)
//            	ros.isNotDisguised("htp");
//                playerScript.playerHidden = false;
//			}
//		} 
//	}

	public void ResetCloset()
	{
		isHiding = false;
		isPaused = false;
		isEntered = false;
		if(ros.disguised == true)
			ros.isNotDisguised("htp");
		showHiding.SetActive (false);
		playerScript.playerHidden = false;
	}

    IEnumerator Wait()
	{
        yield return new WaitForSeconds(0.05f);

		isPaused = true;
        isHiding = true;

        if(ros.disguised == false)
        	ros.isDisguised("htp");

		showHiding.SetActive(true);
		character.transform.position = hidingPosition.transform.position;
		characterCamera.SetHidingPosition(cameraPosition.position);

		characterCamera.SetHidingRotation (cameraPosition.eulerAngles.y);
		characterCamera.transform.localRotation = Quaternion.Euler(0, transform.parent.eulerAngles.y, 0);
		playerScript.playerHidden = true;
		//keyboardCheckToEnter.SetActive(false);
		//keyboardCheckToExit.SetActive(true);

		//controllerCheckToEnter.SetActive(false);
		//controllerCheckToExit.SetActive(true);
	}
	
	IEnumerator Delayed()
	{
        yield return new WaitForSeconds(0.05f);
		
		isHiding = false;
		isPaused = false;

		if(ros.disguised == true)
			ros.isNotDisguised("htp");

		showHiding.SetActive(false);
		character.transform.position = prevPosition.transform.position;
		characterCamera.ResetHidingPosition ();
		//characterCamera.ResetHidingRotation ();
		playerScript.playerHidden = false;

		//controllerCheckToExit.SetActive(false);
		//keyboardCheckToExit.SetActive(false);
	}
}
