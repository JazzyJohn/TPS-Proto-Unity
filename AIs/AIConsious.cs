using UnityEngine;
using System.Collections;

public class AIConscious : MonoBehaviour {
	
	AIAction aiAction;

	void Update()
	{
		if (AIAction != null) aiAction.AIUpdate ();
	}
	
	public void SwichAction (AIAction NewAction)
	{
		aiAction = NewAction;
	}
	
}