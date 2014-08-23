using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using nstuff.juggerfall.extension.models;

public class ConquestPoint : MonoBehaviour {
	public float deltaScorePoint;
	
	public int scorePoint;
	
	public int needToScore;
	
	public FoxView foxView;
	
	public  int owner=0;
	
	public ConquestPointModel model = new ConquestPointModel();
	
	public ShowOnGuiComponent guiElement;
		
	void Awake(){
		guiElement= GetComponent<ShowOnGuiComponent>();
		foxView  = GetComponent<FoxView>();
        guiElement.SetTitle(scorePoint.ToString("0"), owner);
	}
	
	void OnTriggerStay(Collider other) {
		Pawn pawn  =other.GetComponent<Pawn>();
		if(pawn!=null&&!pawn.isAi&&foxView.isMine){
			if(pawn.team==1){
				deltaScorePoint+= Time.deltaTime;
				
			}else{
				deltaScorePoint-= Time.deltaTime;
			}
		
		}
	 
	 
	}
	void Update(){
		if(Mathf.Abs(deltaScorePoint)>=1&&foxView.isMine){
	
			if(Mathf.Abs(scorePoint)<needToScore){
			
				scorePoint+=(int)deltaScorePoint;
			}else{
				if(owner==0){
					if(scorePoint<0){
						owner = 2;
					}else{
						owner =1;
					}
                    PointCoquested();
				}
			}
			if(owner ==1&&scorePoint<0){
                PointNetral();
				
			}
			if(owner ==2&&scorePoint>0){
                PointNetral();
				
			}
			UpdateModel();
			foxView.UpdateConquestPoint(model);
			deltaScorePoint=0;	
			guiElement.SetTitle(scorePoint.ToString("0"),owner);
		}
	
	}
	void UpdateModel(){
		model.id = foxView.viewID;
		model.owner = owner;
		model.scorePoint = scorePoint;
		guiElement.SetTitle(scorePoint.ToString("0"),owner);
	}
	
	public void UpdateFromModel(ConquestPointModel model){
		scorePoint=	model.scorePoint;
		if(model.owner!=owner){
			owner=model.owner;
			if(model.owner==0){
                PointNetral();
			}else{
                PointCoquested();
			}			
		}
	}
	
	public void PointNetral(){
		owner=0;
        SendMessage("StopUse", owner,SendMessageOptions.DontRequireReceiver);
	}
    public void PointCoquested()
    {
		SendMessage("StartUse", owner,SendMessageOptions.DontRequireReceiver);
	}


}