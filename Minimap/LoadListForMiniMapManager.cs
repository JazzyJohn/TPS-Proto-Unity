using UnityEngine;
using System.Collections;

public class LoadListForMiniMapManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		MinimapManager.IndexItem.Clear();
		MinimapManager.IndexItem.AddRange(NJGMapOnGUI.instance.mapItemTypes);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
