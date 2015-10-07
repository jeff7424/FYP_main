using UnityEngine;
using System.Collections;

public class Sniffing : MonoBehaviour
{
    AudioSource audioSource;
    enemyPathfinding enemyPathfindingScript;

    void Start()
    {
		audioSource = GetComponent<AudioSource>();
        enemyPathfindingScript = this.transform.parent.GetComponent<enemyPathfinding>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player") && !enemyPathfindingScript.States.Equals(enumStates.chase))
        {
			audioSource.Play();
            //print("OnTriggerEnter");
        }
    }

    void OnTriggerStay(Collider other)
    {
		if (other.gameObject.CompareTag ("Player"))
        {
			if (!audioSource.isPlaying) audioSource.Play();
            //print("OnTriggerStay");
        }
    }

    void OnTriggerExit(Collider other)
    {
		if (other.gameObject.CompareTag ("Player"))
        {
			if (audioSource.isPlaying) audioSource.Stop();
            //print("OnTriggerExit");
        }
    }
}