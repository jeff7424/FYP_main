// This script goes on the button that has to be pressed by the player or an enemy to open a door

using UnityEngine;
using System.Collections;

public class pushButton : MonoBehaviour
{
    public bool buttonActivated = false;
	[Tooltip("Countdown timer before the button is deactivated")]
	public float defaultTimer;

    float timer;
    Animator anim;

    void Start()
    {
        timer = defaultTimer;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
		// Check if button is activated
		if (buttonActivated == true) 
		{
			// Change animation to button down and start countdown
			GetComponentInParent<Animator> ().SetBool ("isDown", true);
			timer -= Time.deltaTime;

			// If timer reaches 0 deactivate button and reset timer
			if (timer <= 0) {
				buttonActivated = false;
				timer = defaultTimer;
			}
		} 
		else if (buttonActivated == false) 
		{
			// Change animation to button up
			GetComponentInParent<Animator> ().SetBool ("isDown", false);
		}
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("Enemy") || other.gameObject.CompareTag ("FatDog"))
        {
            buttonActivated = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
		if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("Enemy") || other.gameObject.CompareTag ("FatDog"))
        {
            timer = defaultTimer;
            GetComponentInParent<Animator>().SetBool("isDown", false); 
        }
    }
}