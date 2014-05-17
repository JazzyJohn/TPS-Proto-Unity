using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Player))]
[RequireComponent (typeof(PointControll_GameRule))]

public class AssaultPoint : MonoBehaviour {
	private List<Player> invaders = new List<Player> ();
	public int[] points = new int[2];
	private int currentOwner;
	private float nextTick;

	//times when points will be disappear
	private float timeToCountdownRed;
	private float timeToCountdownBlue;

	public float timeToCountdown;
	public int pointsToGoal;//how much points team should earn for own this point.
	public int owner;//team who owning this point at start

	// Use this for initialization
	void Start () {
		for (int i=0; i<2; i++)
			points[i] = 0;

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
			if (invaders [i] && invaders [i].team == 1)
			iRet++;

		return iRet;
	}

	public int countBlueInvaders()
	{
		int iRet = 0;
		for (int i=0; i<invaders.Count; i++)
			if (invaders [i] && invaders [i].team == 2)
				iRet++;
		
		return iRet;
	}

	public void clearScores()
	{
		points [0] = points [1] = 0;
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

		for (int i=0; i<invaders.Count; i++) 
		{
			if(!invaders[i]) continue;

			if(invaders[i].team == 1)
			{
				addPointRed+=1;
				//TODO: get player class and add extra points
			}
			else if(invaders[i].team == 2)
			{
				addPointBlue+=1;
				//TODO: get player class and add extra points
			}
		}

		addPointBlue = Mathf.Min (addPointBlue, 5);//maximum 5 points per 0.5s
		addPointRed = Mathf.Min (addPointRed, 5);//maximum 5 points per 0.5s

		points [0] += addPointRed;
		points [1] += addPointBlue;
	
		Debug.Log ("Times: " + timeToCountdownRed + " .. " + timeToCountdownBlue);
		//If nobody staying on the point in red/blue command, the process of invade will be decreasing every 0.5s by 2 points.
		if (countRedInvaders () == 0 && (timeToCountdownRed < Time.time))
			points [0] -= 2;
		if (countBlueInvaders () == 0 && (timeToCountdownBlue < Time.time))
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

		if (team == 0)
			timeToCountdownRed = Time.time + timeToCountdown;
		else if (team == 1)
			timeToCountdownBlue = Time.time + timeToCountdown;

		Debug.Log ("Team " + team + " Invaded this point");
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
