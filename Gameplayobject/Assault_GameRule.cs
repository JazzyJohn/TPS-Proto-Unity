using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Assault_GameRule : PVPGameRule {
	public int pointsToGoal;//How much points team should invade to win.
	private AssaultPoint[] cPoints;
	
	
	
	public int getPoints(int team)//how much points have team.
	{
		int iRet = 0;
		foreach (AssaultPoint aPoint in cPoints) 
			if(aPoint.getCurrentOwner() == team)
				iRet++;
		
		return iRet;
	}
	
	public void clearPoints()
	{
		foreach (AssaultPoint aPoint in cPoints) 
			aPoint.clearScores ();
	}
	
	public void redTeamWon()
	{
		//TODO: make won.
	}
	
	public void blueTeamWon()
	{
		//TODO: make won.
	}
	
	public void pointHasBeenTaken(AssaultPoint pnt,int team)
	{
		//TODO: event for point invasion.
	}
	
	public void pointHasBeenTaken(TFAssaultPoint pnt,int team)
	{
		//TODO: event for point invasion.
	}
	
	//the game will be ended when any team will take $pointsToGoal$ points.
	public override bool IsGameEnded()
	{
		if (getPoints (1) >= pointsToGoal) 
		{
			redTeamWon ();
			return true;
		}
		else if(getPoints(2) >= pointsToGoal)
		{
			blueTeamWon();
			return true;
		}
		
		return false;
	}
	
	// Use this for initialization
	void Start () {
		cPoints = FindObjectsOfType(typeof(AssaultPoint)) as AssaultPoint[];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
