using UnityEngine;
using System.Collections;

public class startMovie : MonoBehaviour {

    public string movieFolder;
    public int nextScene;
	// Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag ("Player"))
        {
            PlayerPrefs.SetInt("Scene", nextScene);
            PlayerPrefs.SetString("Movie", movieFolder);
            PlayerPrefs.Save();
            Application.LoadLevel(1);
        }
    }
}
