using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class hidingThirdPerson : MonoBehaviour {

	private ringOfSmell ros;

	private checkPoint cp;

    public Transform character;
    public Transform prevPosition;
    public Transform hidingPosition;
    public TemporaryMovement playerScript;

	public GameObject showHiding;

    public bool isHiding;
	public bool isPaused;

	public bool isEntered = false;

	void Start () 
	{
        //isHiding = false;
        //isPaused = false;
		
		cp = GameObject.Find ("Char_Cat").GetComponent<checkPoint>();
		ros = GameObject.Find ("Smell ring FBX").GetComponent<ringOfSmell>();
        playerScript = GameObject.Find("Char_Cat").GetComponent<TemporaryMovement>();

		showHiding.SetActive(false);
	}

    void OnTriggerStay(Collider catType)
    {	
		if (catType.CompareTag ("player"))
		{	

			isEntered = true;
			//keyboardCheckToEnter.SetActive(true);
			//controllerCheckToEnter.SetActive(true);
			
			if (isHiding == false)
			{
				showHiding.SetActive(false);

				if (Input.GetButtonDown("Interact"))
				{
					character.transform.position = hidingPosition.transform.position;
                    playerScript.playerHidden = true;
					StartCoroutine(Wait());
				}
			}
		}
    }

	void OnTriggerExit(Collider cat)
	{	
		if (cat.CompareTag ("player")) 
		{
			isEntered = false;
		}
		//keyboardCheckToEnter.SetActive(false);
		//controllerCheckToEnter.SetActive(false);
	}

	void Update ()
    { 

        if (isHiding == true) 
		{
			if (Input.GetButtonDown("Interact"))
			{
				StartCoroutine (Delayed ());

				isHiding = false;
				isPaused = false;            
                if(ros.disguised == true)
            	ros.isNotDisguised("htp");
                playerScript.playerHidden = false;

		
			}
		} 

//		if (cp.sendBack == true) 
//        {
//			isHiding = false;
//			isPaused = false;    
//			
//			//checkToEnter.enabled = false;
//			//checkToExit.enabled = false;
//
//			cp.sendBack = false;
//		}
	}

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
		//keyboardCheckToEnter.SetActive(false);
		//keyboardCheckToExit.SetActive(true);

		//controllerCheckToEnter.SetActive(false);
		//controllerCheckToExit.SetActive(true);
        
	}
	
	IEnumerator Delayed()
	{
        yield return new WaitForSeconds(0.05f);

        character.transform.position = prevPosition.transform.position;
		//controllerCheckToExit.SetActive(false);
		//keyboardCheckToExit.SetActive(false);
		
   
	}

}
