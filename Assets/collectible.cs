using UnityEngine;
using System.Collections;

public class collectible : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            Destroy(this.gameObject, 0.1f);
        }
    }
}
