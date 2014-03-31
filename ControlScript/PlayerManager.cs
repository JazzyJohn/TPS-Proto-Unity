using UnityEngine;
using System;



public class PlayerManager : MonoBehaviour {

	public Pawn[] avaiblePawn;

	public Pawn[] avaibleBots;

	public GameObject[] ghostsBots;
    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static PlayerManager s_Instance = null;
 
    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static PlayerManager instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance =  FindObjectOfType(typeof (PlayerManager)) as PlayerManager;
            }
 
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("PlayerManager");
                s_Instance = obj.AddComponent(typeof (PlayerManager)) as PlayerManager;
                Debug.Log ("Could not locate an AManager object.  AManager was Generated Automaticly.");
            }
 
            return s_Instance;
        }
    }
 
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
    }
 
    // Add the rest of the code here...
    public Pawn SpawmPlayer(Pawn newPalyerClass) {
		Pawn localPlayer;
		if(Network.connections.Length==0){
			localPlayer =Instantiate(newPalyerClass) as Pawn;
		}else{
			localPlayer =Network.Instantiate(newPalyerClass,Vector3.zero,Quaternion.identity,0) as Pawn;
		}
		return localPlayer;
    }
	

    public Pawn SpawmPlayer(Pawn newPalyerClass, Vector3 position,Quaternion rotation ) {
		Pawn localPlayer;
		if(Network.connections.Length==0){
			localPlayer =Instantiate(newPalyerClass,position,rotation) as Pawn;
		}else{
			localPlayer =Network.Instantiate(newPalyerClass,position,rotation,0) as Pawn;
		}
		return localPlayer;
    }
 
 }
	


