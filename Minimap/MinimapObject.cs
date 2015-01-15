using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MinimapManager{

	public static List<MinimapObject> allMinimapObject = new List<MinimapObject>();
	
	public enum MODE{
		NONE,
		TEAMMATE,
		ALLPALYER,
		ALLANDROTATION,
		ALLANDHP
	}
	
	public static MODE mode;
}


public class MinimapObject : MonoBehaviour {


	
	public enum TYPE{
		PAWN,
		TARGET,
		INFO
	}
	
	
	public TYPE type;
	
	public Pawn pawn;
	
	public int team;
	
	public Transform myTransform;
	
	public void Awake(){
		MinimapManager.allMinimapObject.Add(this);
		myTransform= transform;
		switch(type){
			case TYPE.PAWN:
				pawn= GetComponent<Pawn>();
			break;	
		}
	}

	public void OnRemove(){
		MinimapManager.allMinimapObject.Remove(this);
	
	}
	
	public bool SeeMe(int team){
		switch(type){
			case TYPE.PAWN:
				switch(MinimapManager.mode){
					case MinimapManager.MODE.NONE:
						return false;
					
					break;
					case MinimapManager.MODE.TEAMMATE:
						return pawn.team==team;
					
					break;
					default:
						return true;
					break;
				
				}
			break;	
			case TYPE.TARGET:
				return this.team ==team;
			break;
			case TYPE.INFO:
				return true;
			break;
			default:
			return false;
			break;
		}
	}
	
	public Vector3 RelativePosition(Vector3 position){
		return myTransform.position - position;
	}
	
	public Quaternion RelativeDirection(){
		switch(type){
			case TYPE.PAWN:
				switch(MinimapManager.mode){
					case MinimapManager.MODE.ALLANDHP:
					case MinimapManager.MODE.ALLANDROTATION:
						return myTransform.rotation;					
					break;					
					default:
						return  Quaternion.identity;
					break;
				
				}
			break;	
			default:
            return Quaternion.identity;
			break;
		}
	
	}
	
	public String AddInfo(){
		switch(type){
			case TYPE.PAWN:
				switch(MinimapManager.mode){
					case MinimapManager.MODE.ALLANDHP:
						return pawn.health.ToString("0");
					
					break;
					
					default:
						return "";
					break;
				
				}
			break;	
			default:
			return "";
			break;
		}
		
	}
}