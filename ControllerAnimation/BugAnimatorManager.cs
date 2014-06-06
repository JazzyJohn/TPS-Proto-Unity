using UnityEngine;
using System.Collections;


public class BugAnimatorManager : AnimationManager{
	
	public BugPawn owner;

	protected void Awake()
    {
		owner = transform.parent.GetComponent<BugPawn>();
		base.Awake();
	}

	public void TurnOnAim(){
		owner.ToggleAim(true);
	
	}
	public void TurnOfAim(){
		owner.ToggleAim(false);
	}





}