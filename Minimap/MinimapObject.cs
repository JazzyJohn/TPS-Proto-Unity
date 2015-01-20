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

	public static Camera UICam;

	public static void SetStatus(bool spawn)
	{
		if(spawn)
		{
			foreach(MinimapObject obj in allMinimapObject)
			{
				switch(obj.type)
				{
				case MinimapObject.TYPE.PAWN:
					if(!obj.main)
					{
						obj.GetNewStatus = true;
					}
					break;
				}
			}
			MinimapManager.UICam.enabled = true;
		}
		else
		{
			foreach(MinimapObject obj in allMinimapObject)
			{					
				obj.ThisIndex = 0;
				obj.Item.type = obj.ThisIndex;
			}
		}
	}

	public static UIMiniMapOnGUI MiniMap; 

	public static List<string> IndexItem = new List<string>();

	public static bool needUpdateStatus = false;
}


public class MinimapObject : MonoBehaviour {

	public enum TYPE{
		PAWN,
		TARGET,
		INFO
	}

	Player player;

	public TYPE type;
	
	public Pawn pawn;

	public NJGMapItem Item;
	
	public int team;

	Transform myTransform;

	[HideInInspector] public bool main = false;

	public int ThisIndex;

	public bool MainPawnSpawn;

	[HideInInspector] public bool GetNewStatus = false;

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
		else if(!MinimapManager.MiniMap)
			MinimapManager.MiniMap = Transform.FindObjectOfType<UIMiniMapOnGUI>();
		
		if(!Item)
			Item = gameObject.AddComponent<NJGMapItem>();

		Item.type = 0;



		switch(type)
		{
		case TYPE.PAWN:
			team = pawn.team;

			if(Player.localPlayer.GetActivePawn() == pawn)
			{
				if(!MinimapManager.UICam)
					MinimapManager.UICam = MinimapManager.MiniMap.GetComponent<GUIAnchor>().uiCamera;
				MinimapManager.mode = MinimapManager.MODE.ALLPALYER;

				main = true;
				MinimapManager.MainPawn = pawn;
				MinimapManager.MainPlayerSpawn = true;
				MinimapManager.MiniMap.target = transform;
				ThisIndex = MinimapManager.IndexItem.IndexOf("Player");
				Item.type = ThisIndex;

				StartCoroutine(TimeUpdateStatus());

				MinimapManager.SetStatus(true);
			}
			else if(MinimapManager.MainPlayerSpawn && !main)
			{
				GetStatus();
			}
			break;
		}
	}

	IEnumerator TimeUpdateStatus()
	{
		MinimapManager.needUpdateStatus = true;
		yield return new WaitForEndOfFrame(); //немного идуского кода, но писать for ради 5 кадров...
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		MinimapManager.needUpdateStatus = false;
	}

	void OnDestroy()
	{
		if(main)
		{
			MinimapManager.UICam.enabled = false;
			MinimapManager.MainPlayerSpawn = false;
			MinimapManager.SetStatus(false);
			main = false;
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

	void GetStatus()
	{
		switch(type)
		{
		case TYPE.PAWN:
			switch(MinimapManager.mode)
			{
			case MinimapManager.MODE.NONE:
				ThisIndex = 0;
				break;
			case MinimapManager.MODE.TEAMMATE:
				if(MinimapManager.MainPawn.team == team)
					ThisIndex = MinimapManager.IndexItem.IndexOf("Frend");
				else
					ThisIndex = 0;
				break;
			case MinimapManager.MODE.ALLPALYER:
				if(MinimapManager.MainPawn.team == team)
					ThisIndex = MinimapManager.IndexItem.IndexOf("Frend");
				else
					ThisIndex = MinimapManager.IndexItem.IndexOf("Enemy");
				break;
			case MinimapManager.MODE.ALLANDROTATION:
				if(MinimapManager.MainPawn.team == team)
					ThisIndex = MinimapManager.IndexItem.IndexOf("FrendR");
				else
					ThisIndex = MinimapManager.IndexItem.IndexOf("EnemyR");
				break;
			}
			break;
		}
		Item.newColorGet(ThisIndex);
	}

	public void Update()
	{
		if(ThisIndex != Item.type)
		{
			Item.type = ThisIndex;
		}

		switch(type)
		{
		case TYPE.PAWN:
			MainPawnSpawn = MinimapManager.MainPlayerSpawn;

			if(GetNewStatus)
			{
				GetNewStatus = false;
				GetStatus();
			}

			if(pawn.isDead)
			{
				ThisIndex = MinimapManager.IndexItem.IndexOf("Dead");
				if(main)
				{
					main = false;
					MinimapManager.UICam.enabled = false;
					MinimapManager.MainPlayerSpawn = false;
					MinimapManager.SetStatus(false);
				}
			}
			break;
		}
	}
}