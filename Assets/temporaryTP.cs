using UnityEngine;
using System.Collections;

public class temporaryTP : MonoBehaviour 
{
    public GameObject tp;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            other.transform.position = tp.transform.position;
        }
    }
}
