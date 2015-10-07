using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            Destroy(this.gameObject, 0.1f);
        }
    }
}
