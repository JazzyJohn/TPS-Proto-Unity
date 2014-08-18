using UnityEngine;
using System.Collections;

public class AIConscious : MonoBehaviour {
	
	public AIAction aiAction;
	
	public float Waiting = 0;
	
	void Update()
	{

	}
	
	public void SwichAction (AIAction NewAction)
	{
		aiAction = NewAction;
	}
	
}