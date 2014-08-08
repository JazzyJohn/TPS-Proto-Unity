using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowOnGuiComponent : MonoBehaviour {
	
	public static List<ShowOnGuiComponent> allShowOnGui  = new List<ShowOnGuiComponent>();
	
	public int team =0;
	
	public Transform myTransform;
	
	public string baseTitle;

	private string title;
	
	public string spriteName;
	
	public void Awake(){
		myTransform = transform;
		allShowOnGui.Add(this);
	}
	void OnDestroy(){
		allShowOnGui.Remove(this);
	}
	
	public void SetTitle(string text){
		title = baseTitle +" " +text;
	}
	public void SetTitle(string text,int team){
		title = baseTitle +" " +text;
		this.team =team;
	}
	public string GetTitle(){
		return title;
	}


}