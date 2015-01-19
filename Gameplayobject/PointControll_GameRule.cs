using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PointControll_GameRule : PVPGameRule {
	public int pointsToGoal;//How much points team should invade to win.
	private AssaultPoint[] cPoints;



/*	public int getPoints(int team)//how much points have team.
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
    */
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
		for(int i=0;i<teamScore.Length;i++){
			if(pointsToGoal<=teamScore[i]){
				return true;
			}
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		cPoints = FindObjectsOfType(typeof(AssaultPoint)) as AssaultPoint[];
	//	StartCoroutine("Tick");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/*private IEnumerator Tick()
	{
		while (true)
		{
			
			int aTeam =getPoints(1);
			int bTeam =getPoints(2);
			if(aTeam>bTeam){
				teamScore[0]++;
			}
			if(aTeam<bTeam){
				teamScore[1]++;
			}
			float tick = Mathf.Abs(aTeam-bTeam);
			if(tick!=0){
				tick = 1.0f/tick;
			}else{
				tick=1.0f;
			}
			yield return new WaitForSeconds(tick);
			//Debug.Log("Work");
		}
	}*/
}
