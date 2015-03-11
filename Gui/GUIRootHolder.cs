using UnityEngine;
using System.Collections;

public class GUIRootHolder : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<GUIRootHolder>().Length > 1)
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
