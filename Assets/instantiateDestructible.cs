using UnityEngine;
using System.Collections;

public class instantiateDestructible : MonoBehaviour {
    public GameObject destructible;
    GameObject newDestructible;

	void Start () 
    {
        newDestructible = (GameObject)Instantiate(destructible, this.transform.localPosition, Quaternion.identity);
        newDestructible.transform.parent = transform;
	}

    public void checkpoint()
    {
        Destroy(newDestructible);
		newDestructible = (GameObject)Instantiate(destructible, this.transform.localPosition, Quaternion.identity);
        newDestructible.transform.parent = transform;
    }
}
