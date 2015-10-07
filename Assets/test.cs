using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{
    public Renderer[] props;

    void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.CompareTag ("Player"))
        {
            foreach (Renderer prop in props)
            {
                prop.enabled = false;
            }
        }
    }

    void OnTriggerExit()
    {
        foreach (Renderer prop in props)
        {
            prop.enabled = true;
        }
     }
}
