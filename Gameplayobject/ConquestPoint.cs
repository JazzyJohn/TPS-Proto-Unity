using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConquestPoint : MonoBehaviour {
	public float deltaScorePoint;
	
	public int scorePoint;
	
	public int needToScore;
	
	public FoxView foxview;
	
	public  int owner=0;
	
	public ConquestPointModel model = new ConquestPointModel();
	
	public ShowOnGuiComponent guiElement;
		
	void Awake(){
		guiElement= GetComponent<ShowOnGuiComponent>();
		foxView  = GetComponent<FoxView>();
	}
	
	void OnTriggerStay(Collider other) {
		Pawn pawn  =other.GetComponent<Pawn>();
		if(pawn!=null&&!pawn.IsAi&&foxView.isMine){
			if(pawm.team==1){
				deltaScorePoint+= Time.deltaTime;
				
			}else{
				deltaScorePoint-= Time.deltaTime;
			}
		
		}
	 
	 
	}
	void Update(){
		if(Mathf.Abs(deltaScorePoint)>=1&&foxView.isMine){
	
			if(Mathf.abs(scorePoint)<needToScore){
			
				scorePoint+=deltaScorePoint;
			}else{
				if(owner==0){
					if(scorePoint<0){
						owner = 2;
					}else{
						owner =1;
					}
					StartUse();
				}
			}
			if(owner ==1&&scorePoint<0){
					StopUse();
				
			}
			if(owner ==2&&scorePoint>0){
					StopUse();
				
			}
			UpdateModel();
			foxView.UpdateConquestPoint(model);
			deltaScorePoint=0;	
			guiElement.SetTitle(scorePoint.Tostring(0),owner);
		}
	
	}
	void UpdateModel(){
		model.id = foxFiew.id;
		model.owner = owner;
		model.scorePoint = scorePoint;
		guiElement.SetTitle(scorePoint.Tostring(0),owner);
	}
	
	public void UpdateFromModel(ConquestPointModel model){
		scorePoint=	model.scorePoint;
		if(model.owner!=owner){
			owner=model.owner;
			if(model.owner==0){
				StopUse();
			}else{
				StartUse();
			}			
		}
	}
	
	public void StopUse(){
		owner=0;
		SendMessage("StopUse", SendMessageOptions.DontRequireReceiver);
	}
	public void StartUse(){
		SendMessage("StartUse", SendMessageOptions.DontRequireReceiver);
	}


}