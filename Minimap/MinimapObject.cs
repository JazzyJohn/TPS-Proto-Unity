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

	public static bool MainPlayerSpawn = false;

	public static MODE mode;

	public static Pawn MainPawn;

	public static void SetStatus(bool spawn)
	{
		MiniMap.GetComponent<GUIAnchor>().uiCamera.enabled = spawn;
		if(spawn)
		{
			foreach(MinimapObject obj in allMinimapObject)
			{
				if(!obj.main)
				{
					obj.MainPawn = MainPawn;
					
					if(obj.MainPawn.team == obj.team)
						obj.ThisIndex = obj.IndexItem.IndexOf("Frend");
					else
						obj.ThisIndex = obj.IndexItem.IndexOf("Enemy");
					
					obj.Select = true;
					obj.Item.type = obj.ThisIndex;
				}
			}
		}
		else
		{
			foreach(MinimapObject obj in allMinimapObject)
			{					
				obj.ThisIndex = 0;
				
				obj.Select = false;
				obj.Item.type = obj.ThisIndex;
			}
		}
	}

	public static UIMiniMapOnGUI MiniMap; 
}


public class MinimapObject : MonoBehaviour {


	
	public enum TYPE{
		PAWN,
		TARGET,
		INFO
	}

	Player player;

	public List<string> IndexItem;

	public TYPE type;
	
	public Pawn pawn;

	public NJGMapItem Item;
	
	public int team;

	public bool Select = false;
	
	Transform myTransform;

	public bool main = false;

	public int ThisIndex;

	public Pawn MainPawn;

	public bool MainPawnSpawn;

	public void Awake()
	{
		MinimapManager.allMinimapObject.Add(this);
		myTransform= transform;
		switch(type){
		case TYPE.PAWN:
			pawn= GetComponent<Pawn>();
			break;	
		}
	}

	public void Start()
	{
		if(!Transform.FindObjectOfType<UIMiniMapOnGUI>())
		{
			this.enabled = false;
			Item.enabled = false;
			return;
		}

		if(!MinimapManager.MiniMap)
			MinimapManager.MiniMap = Transform.FindObjectOfType<UIMiniMapOnGUI>();
		
		if(!Item)
			Item = gameObject.AddComponent<NJGMapItem>();
		Item.type = 0;

		IndexItem.AddRange(NJGMapOnGUI.instance.mapItemTypes);

		switch(type)
		{
		case TYPE.PAWN:
			team = pawn.team;

			MainPawn = Player.localPlayer.GetActivePawn();
			if(MainPawn == pawn)
			{
				main = true;
				MinimapManager.MainPawn = pawn;
				MinimapManager.MainPlayerSpawn = true;
				MinimapManager.MiniMap.target = transform;
				ThisIndex = IndexItem.IndexOf("Player");
				Item.type = ThisIndex;
				MinimapManager.SetStatus(true);
			}
			else if(MinimapManager.MainPlayerSpawn && !main)
			{
				MainPawn = MinimapManager.MainPawn;

				if(MainPawn.team == team)
					ThisIndex = IndexItem.IndexOf("Frend");
				else
					ThisIndex = IndexItem.IndexOf("Enemy");

				Select = true;
			}
			break;
		}
	}

	void OnDestroy()
	{
		if(main)
		{
			MinimapManager.MainPlayerSpawn = false;
			MinimapManager.SetStatus(false);
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

	public void FixedUpdate()
	{
		switch(type)
		{
		case TYPE.PAWN:
			MainPawnSpawn = MinimapManager.MainPlayerSpawn;

			if(pawn.isDead)
				ThisIndex = IndexItem.IndexOf("Dead");
			break;
		}

		if(ThisIndex != Item.type)
		{
			Item.type = ThisIndex;
		}
	}
}