using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Player))]
[RequireComponent (typeof(PointControll_GameRule))]

public class TFAssaultPoint : MonoBehaviour {
	private List<Player> invaders = new List<Player> ();

	public bool canBeReinvaded = false;//can this point be invaded by another team.
	public int[] points = new int[2];
	public int pointsForOnePlayer = 1;//how much points bring one player
	public int maxPointsAdded = 3;//maximum 3 points can be added 
	public bool canBeInvadedWithEnemy = false;

	private int currentOwner;
	private float nextTick;
	private bool invaded;

	//times when points will be disappear
	private float timeToCountdownRed;
	private float timeToCountdownBlue;
	
	public float timeToCountdown;
	public int pointsToGoal;//how much points team should earn for own this point.
	public int owner = 1;//team who owning this point at start
	
	// Use this for initialization
	void Start () {
		for (int i=0; i<2; i++)
			points[i] = 0;

		invaded = false;
		setCurrentOwner (owner);
		nextTick = Time.time;
		timeToCountdownRed = timeToCountdownBlue = Time.time;
	}
	
	public int getCurrentOwner()//team witch own this point
	{
		return currentOwner;
	}
	public void setCurrentOwner(int team)
	{
		currentOwner = team;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))	
		{
			Player pl = other.GetComponent<Pawn>().player;
			
			if(!pl)
				return;
			
			invaders.Add(pl);
			Debug.Log( pl.team);
			Debug.Log ("Somebody touch this");
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))	
		{
			Player pl = other.GetComponent<Pawn>().player;
			
			if(!pl)
				return;
			
			invaders.Remove(pl);
			Debug.Log(pl.team);
			Debug.Log ("Somebody untouch this");
		}
	}
	
	public int countRedInvaders()
	{
		int iRet = 0;
		for (int i=0; i<invaders.Count; i++)
			if (invaders [i] && invaders [i].team == 1 && !invaders[i].IsDead())
				iRet++;
		
		return iRet;
	}
	
	public int countBlueInvaders()
	{
		int iRet = 0;
		for (int i=0; i<invaders.Count; i++)
			if (invaders [i] && invaders [i].team == 2 && !invaders[i].IsDead())
				iRet++;
		
		return iRet;
	}
	
	public void clearScores()
	{
		points [0] = points [1] = 0;
		invaded = false;
		setCurrentOwner (owner);
	}
	
	public int getPoints(int team)
	{
		return points [team-1];
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Time.time < nextTick)
			return;
		
		nextTick = Time.time + 0.5f;//ticking every 0.5s
		
		int addPointRed = 0;
		int addPointBlue = 0;
		bool invadable = true;

		for (int i=0; i<invaders.Count; i++) 
		{
			if(!invaders[i]) continue;

			if(invaders[i].team == getCurrentOwner() && !invaders[i].IsDead())//at least one
				invadable = false;

			if(invaders[i].team == 1 && (getCurrentOwner() != invaders[i].team))
			{
				addPointRed+=pointsForOnePlayer;
				//TODO: get player class and add extra points
			}
			else if(invaders[i].team == 2 && (getCurrentOwner() != invaders[i].team))
			{
				addPointBlue+=pointsForOnePlayer;
				//TODO: get player class and add extra points
			}
		}

		if (!invadable && !canBeInvadedWithEnemy)
			return;

		addPointBlue = Mathf.Min (addPointBlue, maxPointsAdded);//maximum $maxPointsAdded points per 0.5s
		addPointRed = Mathf.Min (addPointRed, maxPointsAdded);//maximum $maxPointsAdded points per 0.5s
		
		points [0] += addPointRed;
		points [1] += addPointBlue;

		Debug.Log ("Times: " + timeToCountdownRed + " .. " + timeToCountdownBlue);

		if (!canBeReinvaded && invaded)
				return;

		//If nobody staying on the point in red/blue command, the process of invade will be decreasing every 0.5s by 2 points.
		if (countRedInvaders () == 0 && (getCurrentOwner() != 2))
			points [0] -= 2;
		if (countBlueInvaders () == 0 && (getCurrentOwner() != 1))
			points [1] -= 2;
		
		points [0] = Mathf.Clamp (points [0], 0,pointsToGoal);
		points [1] = Mathf.Clamp (points [1], 0,pointsToGoal);
		
		if (points [0] >= pointsToGoal)
			teamInvadePoint (1);//red.
		else if (points [1] >= pointsToGoal)
			teamInvadePoint (2);//blue
		
	}
	
	void teamInvadePoint(int team)
	{
		if (getCurrentOwner () == team)
			return;
		
		team--;
		
		if (team < 0 || team > 1)
			return;
		
		points [1 - team] = 0;//clear enemy points

		Debug.Log ("Team " + team + " Invaded TF point");
		setCurrentOwner(team + 1);
		
		PointControll_GameRule gameRules = FindObjectOfType<PointControll_GameRule>();
		if (!gameRules)
		{	
			Debug.LogError("No PointControll_GameRule");
			return;
		}
		
		gameRules.pointHasBeenTaken (this, team + 1);
	}
}
