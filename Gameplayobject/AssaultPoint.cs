using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;

public class AssaultPoint : MonoBehaviour {
	
	public int id;
	
	public int startOwner;
	
	public float aiRadius;
	
	public Transform[] aiRoamPoint;
	[HideInInspector] 
	public Transform myTransform;
	
	public AssaultPoint[] lockedByOneTeam;
	
	public AssaultPoint[] lockedBySecondTeam;
	
	[HideInInspector] 
	public float scorePoint;
	
	public float needToScore;
	[HideInInspector] 
	public int owner=0;
	[HideInInspector] 
	public int peopleCnt = 0;
	[HideInInspector] 
    public int teamConquering =0;
	[HideInInspector] 
	public AssaultPointModel model = new AssaultPointModel();
	
	public PointOnGuiComponent guiElement;
	
	private int[] teamCnt = new int[2]{0,0};
	
	private bool send = true;
	
	private bool firstTime = true;

	public void Init(){
		myTransform = transform;
		owner =startOwner;
        if (startOwner != 0)
        {
            scorePoint = needToScore;
        }
		model.id =id;
		model.lockedByOneTeam = new ArrayList();
		for(int i=0;i<lockedByOneTeam.Length;i++){
			model.lockedByOneTeam.Add(lockedByOneTeam[i]);
		}
		model.lockedBySecondTeam = new ArrayList();
		for(int i=0;i<lockedBySecondTeam.Length;i++){
			model.lockedBySecondTeam.Add(lockedBySecondTeam[i]);
		}
	}
	public Transform GetRoamTarget(){
		return aiRoamPoint[(int)(UnityEngine.Random.value * aiRoamPoint.Length)];
	
	}
	
	void OnTriggerStay(Collider other) {
        //Debug.Log(other.name);
        if (other.CompareTag("Player")){
			Pawn pawn =other.GetComponent<Pawn>();
			if(pawn!=null&& !pawn.isDead&&pawn.team!=0){
				teamCnt[pawn.team-1]++;
			}
		}       
    }
	
	void FixedUpdate(){
		int newPeopleCnt=0;
		teamConquering=0;
		for(int i=0;i<teamCnt.Length;i++){
			if(teamCnt[i]>0){
                
				if(newPeopleCnt>0){
					newPeopleCnt=0;
					teamConquering=0;
					break;
				}else{
					newPeopleCnt=teamCnt[i];
					teamConquering= i+1;
				}
			}
			teamCnt[i] =0;
		
		}

		if(peopleCnt!=newPeopleCnt){
			send= true;
		}
		peopleCnt= newPeopleCnt;
		
	}
	
	public bool NeedUpdate(){
		return send;	

	}
	public AssaultPointModel GetModel(){
		model.needPoint = needToScore;
		model.owner = owner;
		model.people = peopleCnt;
		model.teamConquering = teamConquering;
		send=false;
		if(!firstTime){
			model.lockedByOneTeam = null;
			model.lockedBySecondTeam = null;
		}
		firstTime = false;
		return model;	

	}
	
	public void NetUpdate(AssaultPointModel model){
        needToScore = model.needPoint;
        scorePoint = model.scorePoint;
		if(owner!=model.owner){
			GameRule.instance.PointChangeOwner(model.owner);
		}
		owner =model.owner;
        peopleCnt = model.people;
		if(teamConquering!=model.teamConquering){
			GameRule.instance.PointChangeConquare(model.teamConquering);
		}
		teamConquering = model.teamConquering;
      

	}
	public void Update(){
        guiElement.SetTitle(scorePoint + "/" + needToScore, owner,teamConquering);
	
	
	}
}
