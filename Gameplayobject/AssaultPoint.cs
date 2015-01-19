using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;

public class AssaultPoint : MonoBehaviour {
	
	public int id;
	
	public AssaultPoint[] lockedBy;
	
	public float scorePoint;
	
	public float needToScore;
	
	public int owner=0;
	
	public int peopleCnt = 0;
	
    public int teamConquering =0;
	
	public AssaultPointModel model = new AssaultPointModel();
	
	public ShowOnGuiComponent guiElement;
	
	private int[] teamCnt = new int[2]{0,0};
	
	private bool send = true;
	
	public void Init(){
		model.id =id;
		model.lockedBy = new ArrayList();
		for(int i=0;i<lockedBy.Length;i++){
			model.lockedBy.Add(lockedBy[i]);
		}
	}
	
	
	void OnTriggerStay(Collider other) {
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
					teamConquering= i;
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
		return model;	

	}
	
	public void NetUpdate(AssaultPointModel model){
        needToScore = model.needPoint;
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
		guiElement.SetTitle(scorePoint +"/" + needToScore);
		guiElement.team =owner;
	
	}
}
